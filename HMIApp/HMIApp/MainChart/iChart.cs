namespace HMIApp.MainChart
{
    public interface iChart
    {
        void Run();
        void WriteSpecifiedValueFromReference();
        void CreatePlot();
        void ReadActualValueFromDBChart_Simplified(string filepath);
    }
}
