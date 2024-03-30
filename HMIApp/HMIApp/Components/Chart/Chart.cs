//osobny DB i tagZone do odczytywania danych do wykresu z PLC
//Wykres ScottPlott
using OpenTK.Graphics;
using ScottPlot;
using ScottPlot.Plottables;

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

        public void Run()
        {

            double[] dataX = { 100, 105, 120, 144, 150 };
            double[] dataY = { 100, 400, 900, 5000, 12200 };
            double[] dataXForceMin = { 100, 150 };
            double[] dataYForceMin = { 1200,1200 };
            double[] dataXForceMax = { 144, 155 };
            double[] dataYForceMax = { 12200, 12200 };
            double[] dataXRectangle = { 120, 120,144, 144 };
            double[] dataYRectangle = { 0,15000, 15000,0 };
            Form1._Form1.formsPlot1.Plot.XLabel("Pozycja [mm]");
            Form1._Form1.formsPlot1.Plot.YLabel("Siła [N]");
            //Glowny plot
            var mainplot = Form1._Form1.formsPlot1.Plot.Add.Scatter(dataX, dataY);
            mainplot.Color = Colors.Red;
            mainplot.LineStyle.Width = 3;
            //Plot sila minimalna
            var fmin = Form1._Form1.formsPlot1.Plot.Add.Scatter(dataXForceMin, dataYForceMin);
            fmin.Color = Colors.Black;
            fmin.LineStyle.Pattern = LinePattern.Dashed;
            fmin.LineStyle.Width = 1;
            //Plot sila max
            var fmax = Form1._Form1.formsPlot1.Plot.Add.Scatter(dataXForceMax, dataYForceMax);
            fmax.Color = Colors.Black;
            fmax.LineStyle.Pattern = LinePattern.Dashed;
            fmax.LineStyle.Width = 1;
            //Wyrysowanie prostokąta czytania siły
            var hs = Form1._Form1.formsPlot1.Plot.Add.HorizontalSpan(120, 144);
            hs.LineStyle.Pattern = LinePattern.Dashed;
            hs.LineStyle.Width = 1;
            hs.FillStyle.Color = Colors.Blue.WithOpacity(0.2f);
            Form1._Form1.formsPlot1.Plot.Title("Wykres siły");
            Form1._Form1.formsPlot1.Refresh();
        }

        public void ChartDataX()
        {
        }

        public void ChartDataY()
        {
           
        }
    }
}
