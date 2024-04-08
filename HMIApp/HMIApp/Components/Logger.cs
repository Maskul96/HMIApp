using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace HMIApp.Components
{
    public class Logger
    {
        public TextBox _logTextBox;
        public Form1 obj;
        public Logger(Form1 obj)
        {
            this.obj = obj;
        }

        public Logger(TextBox logTextBox)
        {
            _logTextBox = logTextBox;
        }

        public void Log(string message)
        {
            string formattedMessage = $"{DateTime.Now}: {message}\n";



        }
    }
}
