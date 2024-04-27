using System.Windows.Forms;

namespace HMIApp
{
    public interface iApp
    {
        bool RunInitPLC();
        void listBox1_DrawItem(object sender, DrawItemEventArgs e);
        void ClosePLCConnection();
        void ReadActualValueFromDBReferenceOrProcessData(string filepath);
        void ReadAlarmsFromDB(string filepath);

        void ReadIOFromDB(string filepath);
        void WriteToDB(string valuetoWrite, string DataTypeofTag, int filenameIndex);

        void ReadActualValueFromDBChart_Simplified(string filepath);
        void CreatePlot();
        void CreateStaticPlot();

        bool GetBit(byte b, int bitNumber);

        void WriteSpecifiedValueFromReference();
         int ClearPlot { get; set; } 
         int ForceMin { get; set; }
         int ForceMax { get; set; }
         byte StartChart { get; set; }
         double ActValX { get; set; }
         double ActValY { get; set; }

    }
}
