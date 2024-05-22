using System.Windows.Forms;

namespace HMIApp
{
    public interface IApp
    {
        bool RunInitPLC();
        void listBoxWarningsView_DrawItem(object sender, DrawItemEventArgs e);
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
        //brak metody MakeList - w interfejsie nie implementujemy metod statycznych

    }
}
