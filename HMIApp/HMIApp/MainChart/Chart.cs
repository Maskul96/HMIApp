
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

        #region Zmienne do DBka do odczytu 
        public int DBread_position;
        public string DBread_NumberOfDB;
        public int DBread_StartDB;
        public int DBread_EndDB;
        public int DBread_NrOfByteinDB;

        public byte StartChart; //z PLC żądanie rozpoczęcia generowania wykresu
        public int Counter; //Counter przychodzi z PLC za kazdym razem jak PLC odczyta nową daną z inną pozycją i siłą
        public bool EndOfMeasuring;
        public int index;
        public int ForceMin;
        public int ForceMax;
        public double ActValX;
        public double ActValY;
        public double offset = 2.0;

        //Przepisanie wartosci z referencji
        public double FastMovement;
        public double StartReading;
        public double EndReading;
        public double EndPoint;
        #endregion

        public Form1 obj;
        CSVReader CSVReader = new CSVReader();
        App app = new App();
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000000);

        public void Run()
        {
                CreatePlot();
                Form1._Form1.formsPlot1.Refresh();
        }

        //metoda do odczytywania danych do wykresu uproszczone odczytywanie - Metoda wywołana w Form1 w Timerze co 100ms do odczytu
        public void ReadActualValueFromDBChart_Simplified(string filepath)
        {
            var dbtags = CSVReader.DBStructure(filepath);

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBread_position = dbTag.TagName.IndexOf('.') - 2;
                DBread_NumberOfDB = dbTag.TagName.Substring(2, DBread_position);
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych + ustalenie dlugosci tablicy
                if (dbtags.First() == dbTag)
                {
                    DBread_StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    //Zabezpieczenie wyjscia poza index tablicy
                    DBread_EndDB = dbTag.NumberOfByteInDB + dbTag.LengthDataType;
                }
            }

            byte[] DB = new byte[DBread_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBread_NumberOfDB), DBread_StartDB, DBread_EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                switch (dbTag.DataTypeOfTag.ToUpper())
                {
                    case "BYTE":
                        DBread_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        if (dbTag.TagName == "DB665.Measure")
                            StartChart = DB[DBread_NrOfByteinDB];
                        break;
                    case "INT":
                        DBread_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        if (dbTag.TagName == "DB665.Counter")
                        {
                            Counter = libnodave.getS16from(DB, DBread_NrOfByteinDB);
                        }
                        else if (dbTag.TagName == "DB665.ForceMin")
                        {
                            ForceMin = libnodave.getS16from(DB, DBread_NrOfByteinDB);
                        }
                        else if (dbTag.TagName == "DB665.ForceMax")
                        {
                            ForceMax = libnodave.getS16from(DB, DBread_NrOfByteinDB);
                        }
                        break;
                    case "REAL":
                        DBread_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        if (dbTag.TagName == "DB665.ActValX")
                        {
                            ActValX = libnodave.getFloatfrom(DB, DBread_NrOfByteinDB);
                        }
                        else if (dbTag.TagName == "DB665.ActValY")
                        {
                            ActValY = libnodave.getFloatfrom(DB, DBread_NrOfByteinDB);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        public void CreatePlot()
        {

            double[] dataX = new double[500];
            double[] dataY = new double[500];

            if (StartChart == 1)
            {
                index = 0;
                EndOfMeasuring = false;
                for (int i = 0; i < 500; i++)
                {
                    dataX[i] = ActValX;
                    dataY[i] = ActValY;
                    index += i;
                    if (ActValX >= EndReading)
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
                    int[] dataYForceMin = { ForceMin, ForceMin };
                    double[] dataXForceMax = { EndReading + offset, EndPoint + offset };
                    int[] dataYForceMax = { ForceMax, ForceMax };


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
            Form1._Form1.formsPlot1.Plot.XLabel("Pozycja [mm]");
            Form1._Form1.formsPlot1.Plot.YLabel("Siła [N]");
            //Wyrysowanie prostokąta czytania siły
            var hs = Form1._Form1.formsPlot1.Plot.Add.HorizontalSpan(StartReading, EndReading);
            hs.LineStyle.Pattern = LinePattern.Dashed;
            hs.LineStyle.Width = 1;
            hs.FillStyle.Color = Colors.Blue.WithOpacity(0.2f);
            Form1._Form1.formsPlot1.Plot.Title("Wykres siły");
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
