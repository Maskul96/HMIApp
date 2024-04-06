using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace HMIApp.Components.RFIDCardReader
{
    public class SerialPortReader : iSerialPortReader
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
            _serialPort.PortName = "COM1";
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }


        public void Run()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Close()
        {
            if(_serialPort.IsOpen)  _serialPort.Close();  
        }

        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string receivedData = sp.ReadLine();

            Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text = receivedData;
        }
    }

    
}
