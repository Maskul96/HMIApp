//osobny DB i tagZone do odczytywania danych do wykresu z PLC
//Wykres ScottPlott
namespace HMIApp.Components.Chart
{
    public class Chart
    {
        public Chart()
        {
            
        }
        public Chart(Form1 obj)
        {

            this.obj = obj;

        }
        public Form1 obj;
    }
}
