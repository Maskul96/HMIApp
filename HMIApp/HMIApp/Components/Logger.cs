using System;
using System.Collections.Generic;

namespace HMIApp.Components
{
    public class Logger
    {
        public Form1 obj;
        public Logger(Form1 obj)
        {
            this.obj = obj;
        }

        public Logger()
        {
        }


        //Logować tutaj różne błędy z aplikacji wraz z errorami z komunikacji z PLC
        public void LogMessage(string message = "")
        {

             Form1._Form1.textBox_StuatusyAplikacji.AppendText(message + Environment.NewLine);
           
            if (Form1._Form1.textBox_StuatusyAplikacji.Lines.Length > 100)
            {
                Form1._Form1.textBox_StuatusyAplikacji.Clear();
            }
        }
    }
}
