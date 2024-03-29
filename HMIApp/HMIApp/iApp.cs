﻿using System.Windows.Forms;

namespace HMIApp
{
    public interface iApp
    {
        void RunInitPLC();
        ListViewItem MakeList(string Alarm, string Alarm1, string Alarm2);
        void listBox1_DrawItem(object sender, DrawItemEventArgs e);
        void ClosePLCConnection();
        void ReadActualValueFromDB(string filepath);
        void ReadAlarmsFromDB(string filepath);

        void ReadIOFromDB(string filepath);
        bool GetBit(byte b, int bitNumber);
        void WriteToDB(string valuetoWrite, string DataTypeofTag, int filenameIndex);

    }
}
