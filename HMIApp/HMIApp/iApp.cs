using System.Windows.Forms;

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

        void ReadActualValueFromDBChart_Simplified(string filepath);

         int Counter { get; set; } //Counter przychodzi z PLC za kazdym razem jak PLC odczyta nową daną z inną pozycją i siłą
         int ForceMin { get; set; }
         int ForceMax { get; set; }
         byte StartChart { get; set; }
         double ActValX { get; set; }
         double ActValY { get; set; }

    }
}
