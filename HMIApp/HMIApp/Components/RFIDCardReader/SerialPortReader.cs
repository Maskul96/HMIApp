using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Documents;

namespace HMIApp.Components.RFIDCardReader
{
    public class SerialPortReader : ISerialPortReader
    {
        public Form1 obj;
        private SerialPort _serialPort;

        public SerialPortReader(Form1 obj)
        {
            this.obj = obj;
        }
        public SerialPortReader()
        {
            
        }

        public void InitializeSerialPort()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM5";
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }

        public void Run()
        {
            try
            {
                _serialPort.Open();
            }
            catch(Exception)
            {
                MessageBox.Show("Nie można otworzyć portu szeregowego - sprawdź czy Czytnik RFID jest wpięty w poprawny port COM");
            }                     
        }

        public void Close()
        {
            if(_serialPort.IsOpen)  _serialPort.Close();  
        }

        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            string receivedData = sp.ReadExisting();
            // Poniżej procedura obsługi zdarzeń tworzy nowy wątek i uruchamia metodę ReadSerialPort - z dokumentacji metody Invoke
            var threadParameters = new ThreadStart(delegate { Form1._Form1.ReadSerialPort(receivedData); });
            var thread2 = new Thread(threadParameters);
            thread2.Start();
        }
    }   
}
