using HMIApp.Components.CSVReader;
using ScottPlot;
using System;
using System.Buffers;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;



namespace HMIApp
{
    //Klasa do obsługi komunikacji z PLC oraz odczytu/zapisu danych z DB i do rysowania wykresu
    public class App : iApp
    {
        public Form1 obj;

        private int[] DB_Position = new int[8];
        private string[] DB_NumberOfDB = new string[8];
        private string[] DB_DataTypeOfTag = new string[8];
        private int[] DB_NrOfByteinDB = new int[8];
        private int[] DB_NrOfBitinByte = new int[8];
        private int[] DB_LengthofDataType = new int[8];
        private string[] DB_TagName = new string[8];
        private int[] DB_StartDB = new int[8];
        private int[] DB_EndDB = new int[8];
        #region Zmienne do DBka do odczytu wykresu
        private int DBread_position;
        private string DBread_NumberOfDB;
        private int DBread_StartDB;
        private int DBread_EndDB;
        private int DBread_NrOfByteinDB;
        public int ClearPlot { get; set; }
        public int ForceMin { get; set; }
        public int ForceMax { get; set; }
        public byte StartChart { get; set; }
        public double ActValX { get; set; }
        public double ActValY { get; set; }

        public bool EndOfMeasuring;
        public int index10;
        public double offset = 2.0;

        //Przepisanie wartosci z referencji
        public double FastMovement;
        public double StartReading;
        public double EndReading;
        public double EndPoint;
        public int ForceMaxfromRef;
        #endregion

        #region Zmienne do DBka do odczytu 
        private int DBRead_position;
        private string DBRead_NumberOfDB;
        private int DBRead_position1;
        private int DBRead_StartDB;
        private int DBRead_EndDB;
        private int DBRead_NrOfByteinDB;
        private int DBRead_NrOfBitinByte;
        private string DBRead_TagName;
        #endregion

        #region Zmienne do DBka do zapisu
        private int DBWrite_position;
        private string DBWrite_NumberOfDB;
        private string DBWrite_DataTypeofTag;
        private int DBWrite_LengthofDataType;
        private int DBWrite_NrOfByteinDB;
        private int DBWrite_NrOfBitinByte;
        #endregion

        #region Zmienne do DBka do odczytu Alarmów i komunikatow
        private int DBReadAlarm_position;
        private string DBReadAlarm_NumberOfDB;
        private int DBReadAlarm_position1;
        private string DBReadAlarm_NameofTagWithoutNumberofDB;
        private int DBReadAlarm_StartDB;
        private int DBReadAlarm_EndDB;
        private int DBReadAlarm_NrOfByteinDB;
        private int DBReadAlarm_NrOfBitinByte;
        private int DBReadAlarm_LengthOfDataType;
        private string DBReadAlarm_AlarmName;
        private string DBReadAlarm_TagName;
        private string DBReadAlarm_DataTypeofTag;
        private int NrOfMessage;
        private int[] index = new int[8];
        private int[] index1 = new int[8];
        private bool[] blockade = new bool[8];
        private ListViewItem[] item = new ListViewItem[8];
        private ListViewItem[] itemArchive = new ListViewItem[8];
        #endregion

        #region Zmienne do DBka do odczytu IO's
        private int DBReadIO_position;
        private string DBReadIO_NumberOfDB;
        private int DBReadIO_position1;
        private string DBReadIO_NameofTagWithoutNumberofDB;
        private int DBReadIO_StartDB;
        private int DBReadIO_EndDB;
        private int DBReadIO_NrOfByteinDB;
        private int DBReadIO_NrOfBitinByte;
        private int DBReadIO_LengthOfDataType;
        private string DBReadIO_AlarmName;
        private string DBReadIO_TagName;
        private string DBReadIO_DataTypeofTag;
        #endregion

        public App()
        {

        }
        //Ponizej konstruktor z parametrem obj do przekazywania obiektow z Form1 do wnętrza klasy
        public App(Form1 obj)
        {
            this.obj = obj;
        }


        //Stworzenie obiektu z konfiguracja sterownika
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000);

        //Stworzenie obiektu CSVReader do odczytu z pliku
        CSVReader CSVReader = new CSVReader();

        //Metoda do tworzenia alarmów z trzema tekstami (data, PLCTag, Nazwa) w listview 
        private ListViewItem MakeList(string Alarm, string Alarm1, string Alarm2)
        {
            return new ListViewItem(new[] { Alarm, Alarm1, Alarm2 });
        }

        //Metoda uruchamiająca komunikacje z PLC
        public void RunInitPLC()
        {
            PLC.Init();
        }

        public void ClosePLCConnection()
        {
            PLC.Close();
        }

        //Metoda wyciagająca dany bit z całego Byte'a
        private bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        //Kasowanie zakladki Dane po uzyciu metody usuniecia referencji z bazy danych z klasy DataBase.cs
        public void ClearAllValueInForm1(string filepath)
        {
            var dbtags = CSVReader.DBStructure(filepath);

            foreach (var dbTag in dbtags)
            {
                DBRead_position1 = dbTag.TagName.IndexOf(".");
                DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                TextBox txt;
                CheckBox chk;
                switch (dbTag.DataTypeOfTag.ToUpper())
                {

                    case "BOOL":
                        chk = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as CheckBox;
                        chk.Checked = false;
                        break;
                    default:
                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        txt.Text = "";
                        break;
                }
            }
        }

        //metoda do odczytywania danych do wykresu uproszczone odczytywanie - Metoda wywołana w Form1 w Timerze co 10ms do odczytu
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
                        if (dbTag.TagName == "DB665.ClearPlot")
                        {
                            ClearPlot = libnodave.getS16from(DB, DBread_NrOfByteinDB);
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

        //Tworzenie glownego wykresu
        double[] ActX = new double[1000];
        double[] ActY = new double[1000];
        public void CreatePlot()
        {
            ReadActualValueFromDBChart_Simplified("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_4.csv");
            Form1._Form1.formsPlot1.Plot.Axes.SetLimits(FastMovement, EndPoint + 2.5, -500, ForceMaxfromRef);
            if (StartChart == 0)
            {
                Form1._Form1.formsPlot1.Refresh();
                EndOfMeasuring = false;
                index10 = 0;
            }
            if (ClearPlot == 1)
            {
                Form1._Form1.formsPlot1.Plot.Clear();
                CreateStaticPlot();
            }
            WriteSpecifiedValueFromReference();
            if (StartChart == 1 && EndOfMeasuring == false && ActValX > FastMovement + 10.0)
            {

                ActX[index10] = ActValX;
                ActY[index10] = ActValY;
                index10 += 1;


                if (ActValX >= EndPoint && EndOfMeasuring == false)
                {
                    for (int i = index10; i < 1000; i++)
                    {
                        ActX[i] = ActX[i - 1];
                        ActY[i] = ActY[i - 1];
                    }

                    EndOfMeasuring = true;
                    double[] dataXForceMin = { FastMovement, EndPoint };
                    int[] dataYForceMin = { ForceMin, ForceMin };
                    double[] dataXForceMax = { EndReading, EndPoint };
                    int[] dataYForceMax = { ForceMax, ForceMax };

                    //Plot sila minimalna
                    var fmin = Form1._Form1.formsPlot1.Plot.Add.Scatter(dataXForceMin, dataYForceMin);
                    fmin.Color = Colors.Black;
                    fmin.LineStyle.Pattern = LinePattern.Dashed;
                    fmin.LineStyle.Width = 1;
                    ////Plot sila max
                    var fmax = Form1._Form1.formsPlot1.Plot.Add.Scatter(dataXForceMax, dataYForceMax);
                    fmax.Color = Colors.Black;
                    fmax.LineStyle.Pattern = LinePattern.Dashed;
                    fmax.LineStyle.Width = 1;

                    var mainplot = Form1._Form1.formsPlot1.Plot.Add.Scatter(ActX, ActY);
                    mainplot.Color = Colors.Red;
                    mainplot.LineStyle.Width = 2;
                    //Wrzucenie tekstu na wykres z dokladnymi odczytami punktow i siły
                    Form1._Form1.formsPlot1.Plot.Add.Text($"({EndPoint},{ForceMax})", EndPoint, ForceMax);
                    Form1._Form1.formsPlot1.Plot.Add.Text($"({StartReading},{ForceMin})", StartReading, ForceMin);
                    Form1._Form1.formsPlot1.Refresh();
                }
            }

        }

        //Tworzenie prostokata czytania sily
        public void CreateStaticPlot()
        {
            WriteSpecifiedValueFromReference();
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
            Form1._Form1.formsPlot1.Refresh();
        }

        //Ponizsza metoda do wywolania dopiero jak zaczyta sie jakakolwiek referencja
        public void WriteSpecifiedValueFromReference()
        {
            FastMovement = Convert.ToDouble(Form1._Form1.DB666Tag4.Text);
            StartReading = Convert.ToDouble(Form1._Form1.DB666Tag5.Text);
            EndReading = Convert.ToDouble(Form1._Form1.DB666Tag6.Text);
            EndPoint = Convert.ToDouble(Form1._Form1.DB666Tag15.Text);
            ForceMaxfromRef = Convert.ToInt16(Form1._Form1.DB666Tag18.Text);

        }

        //Odczyt z pliku CSV i od razu odczyt danych z DBka Referencji 
        public void ReadActualValueFromDBReferenceOrProcessData(string filepath)
        {
            var dbtags = CSVReader.DBStructure(filepath);

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBRead_position = dbTag.TagName.IndexOf('.') - 2;
                DBRead_NumberOfDB = dbTag.TagName.Substring(2, DBRead_position);
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych + ustalenie dlugosci tablicy
                if (dbtags.First() == dbTag)
                {
                    DBRead_StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    //Zabezpieczenie wyjscia poza index tablicy
                    DBRead_EndDB = dbTag.NumberOfByteInDB + dbTag.LengthDataType;
                }
            }

            byte[] DB = new byte[DBRead_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBRead_NumberOfDB), DBRead_StartDB, DBRead_EndDB, DB);


            foreach (var dbTag in dbtags)
            {
                //sprobowac uproscic tego switcha szczegolnie z wyszukiwaniem kontrolek - zrobic na wzor tego c ozrobilem z zamiana kropki na przecinek
                switch (dbTag.DataTypeOfTag.ToUpper())
                {

                    case "BOOL":
                        //Read BOOL               
                        DBRead_NrOfBitinByte = dbTag.NumberOfBitInByte;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        bool[] values = new bool[8];
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        TextBox txt;
                        switch (DBRead_NrOfBitinByte)
                        {
                            case 0:
                                values[0] = GetBit(DB[DBRead_NrOfByteinDB], 0);
                                //Wyszukanie TextBoxa po jego nazwie
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[0])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 1:
                                values[1] = GetBit(DB[DBRead_NrOfByteinDB], 1);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[1])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 2:
                                values[2] = GetBit(DB[DBRead_NrOfByteinDB], 2);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[2])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 3:
                                values[3] = GetBit(DB[DBRead_NrOfByteinDB], 3);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[3])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 4:
                                values[4] = GetBit(DB[DBRead_NrOfByteinDB], 4);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[4])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 5:
                                values[5] = GetBit(DB[DBRead_NrOfByteinDB], 5);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[5])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 6:
                                values[6] = GetBit(DB[DBRead_NrOfByteinDB], 6);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[6])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                            case 7:
                                values[7] = GetBit(DB[DBRead_NrOfByteinDB], 7);
                                txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                                if (txt == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (values[7])
                                    {
                                        txt.BackColor = System.Drawing.Color.LawnGreen;
                                    }
                                    else
                                    {
                                        txt.BackColor = System.Drawing.Color.OrangeRed;
                                    }
                                }
                                break;
                        }
                        break;
                    case "BYTE":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".")+1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        //DBRead_TagName = dbTag.TagName;
                        //PONIZEJ LINIA DO WYRZUCENIA KROPKI ZZA NUMERU DBKA
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null)
                        {
                            //Wyjscie z case'a jesli nie znajdzie textboxa i bedzie nullem
                            break;
                        }
                        else
                        {
                            txt.Text = Convert.ToString(DB[DBRead_NrOfByteinDB]);
                        }
                        break;
                    case "INT":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        //DBRead_TagName = dbTag.TagName;
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null)
                        {
                            //Wyjscie z case'a jesli nie znajdzie textboxa i bedzie nullem
                            break;
                        }
                        else
                        {
                            txt.Text = Convert.ToString(libnodave.getS16from(DB, DBRead_NrOfByteinDB));
                        }
                        break;
                    case "REAL":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        //DBRead_TagName = dbTag.TagName;
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null)
                        {
                            //Wyjscie z case'a jesli nie znajdzie textboxa i bedzie nullem
                            break;
                        }
                        else
                        {
                            txt.Text = Convert.ToString(libnodave.getFloatfrom(DB, DBRead_NrOfByteinDB));
                        }

                        break;
                    case "DINT":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        //DBRead_TagName = dbTag.TagName;
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null)
                        {
                            break;
                        }
                        else
                        {
                            txt.Text = Convert.ToString(libnodave.getS32from(DB, DBRead_NrOfByteinDB));
                        }
                        break;
                    case "STRING":
                        //Pierwszy bajt stringa - oznacza calkowita długosc
                        //Drugi bajt stringa oznacza dlugosc obecnie wpisanego ciagu znaków
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        //DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        //DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        //DBRead_TagName = dbTag.TagName;
                        DBRead_position1 = dbTag.TagName.IndexOf(".");
                        DBRead_TagName = dbTag.TagName.Remove(DBRead_position1, 1);
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        DBRead_LengthOfDataType = dbTag.LengthDataType;

                        int SecondByte = DBRead_NrOfByteinDB + 1;
                        int actuallengthofstring = Convert.ToInt16(DB[SecondByte]);
                        string StringFromDB = "";
                        for (int i = SecondByte + 1; i <= actuallengthofstring + SecondByte; i++)
                        {
                            StringFromDB += (char)DB[i];

                        }
                        txt = Form1._Form1.Controls.Find($"{DBRead_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null)
                        {
                            break;
                        }
                        else
                        {
                            txt.Text = StringFromDB;
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        //Zapis danych do DB i odczyt pliku z zapisywaniem
        //Index = 0 zapis do DB666, index = 1 zapis do DB667
        public void WriteToDB(string valuetoWrite, string NameofTaginDB, int filenameIndex = 0)
        {
            string filename = "";
            if (filenameIndex == 0)
            {
                filename = "D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv";
            }
            else if (filenameIndex == 1)
            {
                filename = "D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv";
            }
            //Odczyt listy z tagami do zapisu
            var dbtags = CSVReader.DBStructure(filename);

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBWrite_position = dbTag.TagName.IndexOf(".") - 2;
                DBWrite_NumberOfDB = dbTag.TagName.Substring(2, DBWrite_position);
                //Przepisanie danych potrzebnych do konfiguracji zapisu odpowiednich danych
                if (NameofTaginDB == dbTag.TagName)
                {
                    DBWrite_NrOfByteinDB = dbTag.NumberOfByteInDB;
                    DBWrite_LengthofDataType = dbTag.LengthDataType;
                    DBWrite_DataTypeofTag = dbTag.DataTypeOfTag;
                    DBWrite_NrOfBitinByte = dbTag.NumberOfBitInByte;
                }
            }
            byte[] IntBytes = new byte[0];
            switch (DBWrite_DataTypeofTag.ToUpper())
            {
                //ZAPISYWANIE BOOLI 
                case "BOOL":
                    byte[] DB = new byte[1];
                    bool values = false;
                    string valuesToString = "";
                    int boolValueFromDB = 0;
                    int boolValue = 0;
                    byte[] BoolToSave = new byte[0];
                    //Read DB - Odczytanie DBka w celu ustalenia jaką wartosc od 0 do 255 jest w danym bajcie
                    PLC.Read(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, DB);
                    //Write BOOL
                    boolValueFromDB = DB[0];
                    values = GetBit(DB[0], DBWrite_NrOfBitinByte);
                    valuesToString = values.ToString();
                    if (valuesToString.ToUpper() == "TRUE")
                    {
                        if (valuetoWrite.ToUpper() == "TRUE" || valuetoWrite.ToUpper() == "")
                        {
                            boolValueFromDB = DB[0];
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                        }
                        else if (valuetoWrite.ToUpper() == "FALSE")
                        {
                            switch (DBWrite_NrOfBitinByte)
                            {
                                case 0:
                                    boolValue = 1;
                                    break;
                                case 1:
                                    boolValue = 2;
                                    break;
                                case 2:
                                    boolValue = 4;
                                    break;
                                case 3:
                                    boolValue = 8;
                                    break;
                                case 4:
                                    boolValue = 16;
                                    break;
                                case 5:
                                    boolValue = 32;
                                    break;
                                case 6:
                                    boolValue = 64;
                                    break;
                                case 7:
                                    boolValue = 128;
                                    break;
                            }

                            boolValueFromDB = DB[0];
                            boolValueFromDB = boolValueFromDB - boolValue;
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, BoolToSave);
                        }
                    }
                    else if (valuesToString.ToUpper() == "FALSE")
                    {
                        if (valuetoWrite.ToUpper() == "FALSE" || valuetoWrite.ToUpper() == "")
                        {
                            boolValueFromDB = DB[0];
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                        }
                        else if (valuetoWrite.ToUpper() == "TRUE")
                        {
                            switch (DBWrite_NrOfBitinByte)
                            {
                                case 0:
                                    boolValue = 1;
                                    break;
                                case 1:
                                    boolValue = 2;
                                    break;
                                case 2:
                                    boolValue = 4;
                                    break;
                                case 3:
                                    boolValue = 8;
                                    break;
                                case 4:
                                    boolValue = 16;
                                    break;
                                case 5:
                                    boolValue = 32;
                                    break;
                                case 6:
                                    boolValue = 64;
                                    break;
                                case 7:
                                    boolValue = 128;
                                    break;
                            }
                            boolValueFromDB = DB[0];
                            boolValueFromDB = boolValueFromDB + boolValue;
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, BoolToSave);
                        }
                    }

                    break;
                case "BYTE":
                    //Write BYTE
                    if (valuetoWrite != "")
                    {
                        short ByteValue = Convert.ToByte(valuetoWrite);
                        IntBytes = BitConverter.GetBytes(ByteValue);
                        PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                    }
                    break;
                case "INT":
                    //write INT
                    if (valuetoWrite != "")
                    {
                        short intValue = Convert.ToInt16(valuetoWrite);
                        IntBytes = BitConverter.GetBytes(intValue);
                        Array.Reverse(IntBytes);
                        PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                    }
                    break;
                case "REAL":
                    //Write Float
                    if (!valuetoWrite.Contains("."))
                    {
                        if (valuetoWrite != "")
                        {
                            float fltValue = (float)Convert.ToDouble(valuetoWrite);
                            var i = libnodave.daveToPLCfloat(fltValue);
                            IntBytes = BitConverter.GetBytes(i);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Zamiast kropki daj przecinek :)");
                    }
                    break;
                case "DINT":
                    //Write DINT
                    if (valuetoWrite != "")
                    {
                        int dintValue = Convert.ToInt32(valuetoWrite);
                        IntBytes = BitConverter.GetBytes(dintValue);
                        Array.Reverse(IntBytes);
                        PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                    }
                    break;
                case "STRING":
                    //Write string
                    if (valuetoWrite != "")
                    {
                        int length = valuetoWrite.Length;
                        char[] charArray = new char[length + 2];
                        charArray[0] = (char)DBWrite_LengthofDataType;
                        charArray[1] = (char)length;
                        for (int i = 2; i <= length + 1; i++)
                        {
                            charArray[i] = Convert.ToChar(valuetoWrite[i - 2]);
                        }
                        IntBytes = Encoding.ASCII.GetBytes(charArray);
                        if (length <= DBWrite_LengthofDataType - 2)
                        {
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                        }
                    }
                    break;
                default:
                    break;

            }
        }

        //ALARMY
        public void ReadAlarmsFromDB(string filepath)
        {
            var dbtags = CSVReader.DBAlarmStructure(filepath);

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBReadAlarm_position = dbTag.TagName.IndexOf('.') - 2;
                DBReadAlarm_NumberOfDB = dbTag.TagName.Substring(2, DBReadAlarm_position);
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych + ustalenie dlugosci tablicy
                if (dbtags.First() == dbTag)
                {
                    DBReadAlarm_StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    //Zabezpieczenie wyjscia poza index tablicy
                    DBReadAlarm_EndDB = dbTag.NumberOfByteInDB;
                    if (DBReadAlarm_EndDB == 0)
                    {
                        DBReadAlarm_EndDB++;
                    }
                    else
                    {
                        DBReadAlarm_EndDB = dbTag.NumberOfByteInDB + dbTag.LengthDataType;
                    }
                }
            }

            byte[] DB = new byte[DBReadAlarm_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBReadAlarm_NumberOfDB), DBReadAlarm_StartDB, DBReadAlarm_EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                //Read BOOL
                DBReadAlarm_NrOfBitinByte = dbTag.NumberOfBitInByte;
                DBReadAlarm_NrOfByteinDB = dbTag.NumberOfByteInDB;
                DBReadAlarm_AlarmName = dbTag.NameofAlarm;
                DBReadAlarm_TagName = dbTag.TagName;
                DBReadAlarm_DataTypeofTag = dbTag.DataTypeOfTag;
                bool[] values = new bool[8];
                switch (DBReadAlarm_DataTypeofTag.ToUpper())
                {
                    case "BOOL":

                        switch (DBReadAlarm_NrOfBitinByte)
                        {
                            case 0:
                                values[0] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 0);
                                if (values[0].ToString().ToUpper() == "TRUE")
                                {
                                    //Dodanie alarmu tylko wtedy jeśli nie ma go juz w liscie
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {

                                        item[0] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[0].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[0]);
                                        index[0] = Form1._Form1.listView_Alarms.Items.IndexOf(item[0]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }

                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;

                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[0] == false)
                                    {
                                        itemArchive[0] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[0].BackColor = System.Drawing.Color.Red; itemArchive[0].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[0]);
                                        index1[0] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[0]);
                                        blockade[0] = true;
                                    }

                                }
                                else
                                {
                                    blockade[0] = false;
                                    //Zabezpieczenie zeby to sie nie wykonywalo jak bedzie pusta lista
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[0]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }

                                    //Kasowanie archiwum alarmów po 100 alarmach
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[0]);
                                        }
                                    }
                                }
                                break;
                            case 1:
                                values[1] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 1);

                                if (values[1].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[1] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[1].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[1]);
                                        index[1] = Form1._Form1.listView_Alarms.Items.IndexOf(item[1]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;
                   
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[1] == false)
                                    {
                                        itemArchive[1] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[1].BackColor = System.Drawing.Color.Red; itemArchive[1].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[1]);
                                        index1[1] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[1]);
                                        blockade[1] = true;
                                    }
                                }
                                else
                                {
                                    blockade[1] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[1]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[1]);
                                        }
                                    }
                                }
                                break;
                            case 2:
                                values[2] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 2);
                                if (values[2].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[2] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[2].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[2]);
                                        index[2] = Form1._Form1.listView_Alarms.Items.IndexOf(item[2]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;

                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[2] == false)
                                    {
                                        itemArchive[2] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[2].BackColor = System.Drawing.Color.Red; itemArchive[2].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[2]);
                                        index1[2] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[2]);
                                        blockade[2] = true;
                                    }
                                }
                                else
                                {
                                    blockade[2] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[2]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[2]);
                                        }
                                    }
                                }
                                break;
                            case 3:
                                values[3] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 3);
                                if (values[3].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[3] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[3].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[3]);
                                        index[3] = Form1._Form1.listView_Alarms.Items.IndexOf(item[3]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;
           
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[3] == false)
                                    {
                                        itemArchive[3] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[3].BackColor = System.Drawing.Color.Red; itemArchive[3].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[3]);
                                        index1[3] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[3]);
                                        blockade[3] = true;
                                    }
                                }
                                else
                                {
                                    blockade[3] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[3]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[3]);
                                        }
                                    }
                                }
                                break;
                            case 4:
                                values[4] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 4);
                                if (values[4].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[4] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[4].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[4]);
                                        index[4] = Form1._Form1.listView_Alarms.Items.IndexOf(item[4]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;

                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[4] == false)
                                    {
                                        itemArchive[4] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[4].BackColor = System.Drawing.Color.Red; itemArchive[4].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[4]);
                                        index1[4] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[4]);
                                        blockade[4] = true;
                                    }
                                }
                                else
                                {
                                    blockade[4] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[4]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[4]);
                                        }
                                    }
                                }
                                break;
                            case 5:
                                values[5] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 5);
                                if (values[5].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[5] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[5].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[5]);
                                        index[5] = Form1._Form1.listView_Alarms.Items.IndexOf(item[5]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;

                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[5] == false)
                                    {
                                        itemArchive[5] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[5].BackColor = System.Drawing.Color.Red; itemArchive[5].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[5]);
                                        index1[5] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[5]);
                                        blockade[5] = true;
                                    }
                                }
                                else
                                {
                                    blockade[5] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[5]);
                                        }
                                    }

                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[5]);
                                        }
                                    }
                                }
                                break;
                            case 6:
                                values[6] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 6);
                                if (values[6].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[6] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[6].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[6]);
                                        index[6] = Form1._Form1.listView_Alarms.Items.IndexOf(item[6]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }
                                    if (blockade[6] == false)
                                    {
                                        itemArchive[6] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[6].BackColor = System.Drawing.Color.Red; itemArchive[6].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[6]);
                                        index1[6] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[6]);
                                        blockade[6] = true;
                                    }
                                }
                                else
                                {
                                    blockade[6] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[6]);
                                        }
                                    }

                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[6]);
                                        }
                                    }
                                }
                                break;
                            case 7:
                                values[7] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 7);
                                if (values[7].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item[7] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        item[7].ForeColor = System.Drawing.Color.Red;
                                        Form1._Form1.listView_Alarms.Items.Add(item[7]);
                                        index[7] = Form1._Form1.listView_Alarms.Items.IndexOf(item[7]);
                                        Form1._Form1.dataGridView1.Visible = true;
                                        Form1._Form1.ButtonOKClosePopUpAlarms.Visible = true;
                                    }
                                    //z uzyciem LINQ
                                    try
                                    {
                                        int rowIndex = -1;
                                        bool tempAllowUserToAddRows = Form1._Form1.dataGridView1.AllowUserToAddRows;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = false;
                                        DataGridViewRow row = Form1._Form1.dataGridView1.Rows
                                            .Cast<DataGridViewRow>()
                                            .Where(r => r.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            .First();

                                        rowIndex = row.Index;
                                        Form1._Form1.dataGridView1.AllowUserToAddRows = tempAllowUserToAddRows;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Form1._Form1.dataGridView1.Rows.Add(DBReadAlarm_AlarmName);
                                    }

                                    if (blockade[7] == false)
                                    {
                                        itemArchive[7] = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive[7].BackColor = System.Drawing.Color.Red; itemArchive[7].ForeColor = System.Drawing.Color.Black;
                                        Form1._Form1.listView_AlarmsArchive.Items.Add(itemArchive[7]);
                                        index1[7] = Form1._Form1.listView_AlarmsArchive.Items.IndexOf(itemArchive[7]);
                                        blockade[7] = true;
                                    }
                                }
                                else
                                {
                                    blockade[7] = false;
                                    if (Form1._Form1.listView_Alarms.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_Alarms.Items.Count > 0)
                                        {
                                            Form1._Form1.listView_Alarms.Items.Remove(item[7]);
                                        }
                                    }
                                    foreach (DataGridViewRow row in Form1._Form1.dataGridView1.Rows)
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            if (row.Cells[0].Value.ToString().Equals(DBReadAlarm_AlarmName))
                                            {
                                                Form1._Form1.dataGridView1.Rows.Remove(row);
                                            }
                                        }
                                    }
                                    if (Form1._Form1.listView_AlarmsArchive.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView_AlarmsArchive.Items.Count > 100)
                                        {
                                            Form1._Form1.listView_AlarmsArchive.Items.Remove(itemArchive[7]);
                                        }
                                    }
                                }

                                break;

                        }
                        if (Form1._Form1.dataGridView1.Rows.Count == 0 )
                        {
                            Form1._Form1.dataGridView1.Visible = false;
                            Form1._Form1.ButtonOKClosePopUpAlarms.Visible = false;
                        }
                        break;
                    case "INT":
                        NrOfMessage = libnodave.getS16from(DB, DBReadAlarm_NrOfByteinDB);
                        Form1._Form1.listBoxWarningsView.SelectionMode = SelectionMode.MultiSimple;
                        Form1._Form1.listBoxWarningsView.SetSelected(NrOfMessage, true);
                        break;
                }

            }
        }


        //metoda do ustawienia po srodku item w listBox - moduł komunikatów z PLC
        public void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            var textRect = e.Bounds;
            var textColor = e.ForeColor;
            textColor = System.Drawing.Color.Black;
            string itemText = Form1._Form1.listBoxWarningsView.Items[e.Index].ToString();
            TextRenderer.DrawText(e.Graphics, itemText, e.Font, textRect, textColor, flags);
        }

        public void ReadIOFromDB(string filepath)
        {
            var dbtags = CSVReader.DBStructure(filepath);

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBReadIO_position = dbTag.TagName.IndexOf('.') - 2;
                DBReadIO_NumberOfDB = dbTag.TagName.Substring(2, DBReadIO_position);
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych + ustalenie dlugosci tablicy
                if (dbtags.First() == dbTag)
                {
                    DBReadIO_StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    //Zabezpieczenie wyjscia poza index tablicy
                    DBReadIO_EndDB = dbTag.NumberOfByteInDB + 1;
                }
            }

            byte[] DB = new byte[DBReadIO_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBReadIO_NumberOfDB), DBReadIO_StartDB, DBReadIO_EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                //Read BOOL

                DBReadIO_NrOfBitinByte = dbTag.NumberOfBitInByte;
                DBReadIO_NrOfByteinDB = dbTag.NumberOfByteInDB;
                DBReadIO_DataTypeofTag = dbTag.DataTypeOfTag;
                bool[] values = new bool[8];
                //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                DBReadIO_position1 = dbTag.TagName.IndexOf(".");
                DBReadIO_TagName = dbTag.TagName.Remove(DBReadIO_position1, 1);
                TextBox txt;
                switch (DBReadIO_NrOfBitinByte)
                {
                    case 0:
                        values[0] = GetBit(DB[DBReadIO_NrOfByteinDB], 0);
                        //Wyszukanie TextBoxa po jego nazwie i ustawienie kolor uw zaleznosci od stanu value[x]
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[0] == true)
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 1:
                        values[1] = GetBit(DB[DBReadIO_NrOfByteinDB], 1);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[1])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 2:
                        values[2] = GetBit(DB[DBReadIO_NrOfByteinDB], 2);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[2])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 3:
                        values[3] = GetBit(DB[DBReadIO_NrOfByteinDB], 3);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[3])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 4:
                        values[4] = GetBit(DB[DBReadIO_NrOfByteinDB], 4);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[4])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 5:
                        values[5] = GetBit(DB[DBReadIO_NrOfByteinDB], 5);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[5])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 6:
                        values[6] = GetBit(DB[DBReadIO_NrOfByteinDB], 6);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[6])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                    case 7:
                        values[7] = GetBit(DB[DBReadIO_NrOfByteinDB], 7);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_TagName}", true).FirstOrDefault() as TextBox;
                        if (txt == null) break;
                        else
                        {
                            if (values[7])
                            {
                                txt.BackColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                txt.BackColor = System.Drawing.Color.White;
                            }
                        }
                        break;
                }

            }

        }

    }
}
