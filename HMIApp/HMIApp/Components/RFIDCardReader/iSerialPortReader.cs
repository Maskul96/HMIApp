
using System.IO.Ports;
using Windows.ApplicationModel.VoiceCommands;

namespace HMIApp.Components.RFIDCardReader
{
    public interface iSerialPortReader
    {
        void Run();
        void Close();
        void InitializeSerialPort();

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e);
    }
}
