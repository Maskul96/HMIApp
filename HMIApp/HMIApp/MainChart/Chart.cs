
using HMIApp.Components.CSVReader;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics;
using ScottPlot;
using ScottPlot.Plottables;
using System.Collections;
using System.Linq;
using System;
using Microsoft.Identity.Client;
//namespace nie nazywac tak samo jak klasa czyli np HMIApp.Chart i klasa Chart
namespace HMIApp.MainChart
{

    public class Chart : iChart
    {
        public Chart()
        {

        }
        public Chart(Form1 obj)
        {

            this.obj = obj;

        }



        public bool EndOfMeasuring;
        public int index;
        public double offset = 2.0;

        //Przepisanie wartosci z referencji
        public double FastMovement;
        public double StartReading;
        public double EndReading;
        public double EndPoint;


        public Form1 obj;
        App app = new App();


        public void Run()
        {
            app.ReadActualValueFromDBChart_Simplified("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_4.csv");
                CreatePlot();
                Form1._Form1.formsPlot1.Refresh();
        }

        public void CreatePlot()
        {

            double[] dataX = new double[500];
            double[] dataY = new double[500];

            if (app.StartChart == 1)
            {
                index = 0;
                EndOfMeasuring = false;
                for (int i = 0; i < 500; i++)
                {
                    dataX[i] = app.ActValX;
                    dataY[i] = app.ActValY;
                    index += i;
                    if (app.ActValX >= EndReading)
                    {
                        EndOfMeasuring = true;
                        break;
                    }
                }

                if (EndOfMeasuring)
                {
                    //uzupelnienie komorek ktore mogly zostac nie zapelnione przez dane z PLC
                    for (int i = index + 1; i <= 500; i++)
                    {
                        dataX[i] = dataX[i - 1];
                        dataY[i] = dataY[i - 1];
                    }


                    double[] dataXForceMin = { FastMovement, EndPoint };
                    int[] dataYForceMin = { app.ForceMin, app.ForceMin };
                    double[] dataXForceMax = { EndReading + offset, EndPoint + offset };
                    int[] dataYForceMax = { app.ForceMax, app.ForceMax };


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


                    EndOfMeasuring = false;
                }
            }
            WriteSpecifiedValueFromReference();
            Form1._Form1.formsPlot1.Refresh();

        }

        public void CreateStaticPlot()
        {
            Form1._Form1.formsPlot1.Plot.XLabel("Pozycja [mm]");
            Form1._Form1.formsPlot1.Plot.YLabel("Siła [N]");
            //Wyrysowanie prostokąta czytania siły
            var hs = Form1._Form1.formsPlot1.Plot.Add.HorizontalSpan(StartReading, EndReading);
            var hs1 = Form1._Form1.formsPlot1.Plot.Add.Scatter(FastMovement, 0);
            hs1.Color = Colors.White;
            var hs2 = Form1._Form1.formsPlot1.Plot.Add.Scatter(EndReading, 0);
            hs2.Color = Colors.White;
            hs.LineStyle.Pattern = LinePattern.Dashed;
            hs.LineStyle.Width = 1;
            hs.FillStyle.Color = Colors.Blue.WithOpacity(0.2f);
            Form1._Form1.formsPlot1.Plot.Title("Wykres siły");
            WriteSpecifiedValueFromReference();
            Form1._Form1.formsPlot1.Refresh();
        }

        //Ponizsza metoda do wywolania dopiero jak zaczyta sie jakakolwiek referencja
        public void WriteSpecifiedValueFromReference()
        {
            FastMovement = Convert.ToDouble(Form1._Form1.DB666Tag4.Text);
            StartReading = Convert.ToDouble(Form1._Form1.DB666Tag5.Text);
            EndReading = Convert.ToDouble(Form1._Form1.DB666Tag6.Text);
            EndPoint = Convert.ToDouble(Form1._Form1.DB666Tag15.Text);
        }
    }
}
