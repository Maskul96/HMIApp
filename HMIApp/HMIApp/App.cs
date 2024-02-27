using HMIApp.Components;
using HMIApp.Components.CSVReader;
using HMIApp.Components.CSVReader.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;

namespace HMIApp
{
    public class App : iApp
    {
        //Deklaracja zmiennych
        //Zmienne do DBka do odczytania
        private readonly iCSVReader _csvReader;
        public int DBRead_position;
        public string DBRead_NumberOfDB;
        public int DBRead_position1;
        public string DBRead_NameofTagWithoutNumberofDB;
        public int DBRead_StartDB;
        public int DBRead_EndDB;
        public int DBRead_NrOfByteinDB;
        public int DBRead_NrOfBitinByte;
        public int DBRead_LengthOfDataType;
        public string DBRead_TagName;
        public Form1 obj;

        //Zmienne do DBka do zapisywania
        public int DBWrite_position;
        public string DBWrite_NumberOfDB;
        public string DBWrite_DataTypeofTag;
        public int DBWrite_StartDB;
        public int DBWrite_EndDB;
        public int DBWrite_LengthofDataType;
        public int DBWrite_NrOfByteinDB;
        public int DBWrite_NrOfBitinByte;


        //Zmienne do DBka do alarmów
        public int DBReadAlarm_position;
        public string DBReadAlarm_NumberOfDB;
        public int DBReadAlarm_position1;
        public string DBReadAlarm_NameofTagWithoutNumberofDB;
        public int DBReadAlarm_StartDB;
        public int DBReadAlarm_EndDB;
        public int DBReadAlarm_NrOfByteinDB;
        public int DBReadAlarm_NrOfBitinByte;
        public int DBReadAlarm_LengthOfDataType;
        public string DBReadAlarm_AlarmName;
        public string DBReadAlarm_TagName;
        public string DBReadAlarm_DataTypeofTag;
        public int NrOfMessage;
        public int[] index = new int[8];
        public int[] index1 = new int[8];
        public  ListViewItem item1 = new ListViewItem();
        public ListViewItem item = new ListViewItem();
        public ListViewItem item2= new ListViewItem();
        public ListViewItem item3= new ListViewItem();
        public ListViewItem item4= new ListViewItem();
        public ListViewItem item5= new ListViewItem();
        public ListViewItem item6= new ListViewItem();
        public ListViewItem item7= new ListViewItem();
        public ListViewItem itemArchive = new ListViewItem();
        public ListViewItem itemArchive1 = new ListViewItem();
        public ListViewItem itemArchive2 = new ListViewItem();
        public ListViewItem itemArchive3 = new ListViewItem();
        public ListViewItem itemArchive4 = new ListViewItem();
        public ListViewItem itemArchive5 = new ListViewItem();
        public ListViewItem itemArchive6 = new ListViewItem();
        public ListViewItem itemArchive7 = new ListViewItem();


        //Zmienne do DBka do IO
        public int DBReadIO_position;
        public string DBReadIO_NumberOfDB;
        public int DBReadIO_position1;
        public string DBReadIO_NameofTagWithoutNumberofDB;
        public int DBReadIO_StartDB;
        public int DBReadIO_EndDB;
        public int DBReadIO_NrOfByteinDB;
        public int DBReadIO_NrOfBitinByte;
        public int DBReadIO_LengthOfDataType;
        public string DBReadIO_AlarmName;
        public string DBReadIO_TagName;
        public string DBReadIO_DataTypeofTag;

        public App(iCSVReader csvReader)
        {
            _csvReader = csvReader;
        }
        public App()
        {

        }
        //Ponizej konstruktor z parametrem obj do przekazywania obiektow z Form1 do wnętrza klasy
        public App(Form1 obj)
        {
            this.obj = obj;
        }

        //Stworzenie obiektu z konfiguracja sterownika
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000000);

        //Stworzenie obiektu CSVReader do odczytu z pliku
        CSVReader CSVReader = new CSVReader();

        //Metoda do tworzenia alarmów z trzema tekstami w listview
        public ListViewItem MakeList( string Alarm, string Alarm1, string Alarm2)
        {
           return new ListViewItem(new[] {Alarm, Alarm1, Alarm2});
        }

        //Metoda uruchamiająca komunikacje z PLC
        public void RunInitPLC()
        {
            PLC.Init();

            int TimeoutPLC = PLC.dave_interface_.getTimeout();
            Form1._Form1.textBox5.Text = TimeoutPLC.ToString();
        }

        public void ClosePLCConnection()
        {
            PLC.Close();
        }

        //Metoda wyciagająca dany bit z całego Byte'a
        public bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        //Odczyt z pliku CSV i od razu odczyt danych z DBka
        public void ReadActualValueFromDB(string filepath)
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
                    switch (dbTag.DataTypeOfTag.ToUpper())
                    {
                        case "BOOL":
                            DBRead_EndDB = dbTag.NumberOfByteInDB;
                            break;
                        case "BYTE":
                            DBRead_EndDB = dbTag.NumberOfByteInDB + 1;
                            break;
                        case "INT":
                            DBRead_EndDB = dbTag.NumberOfByteInDB + 2;
                            break;
                        default:
                            DBRead_EndDB = dbTag.NumberOfByteInDB + 4;
                            break;
                    }
                }
            }

            byte[] DB = new byte[DBRead_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBRead_NumberOfDB), DBRead_StartDB, DBRead_EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                //sprobowac uproscic tego switcha
                switch (dbTag.DataTypeOfTag.ToUpper())
                {
                    case "BOOL":
                        //Read BOOL               
                        DBRead_NrOfBitinByte = dbTag.NumberOfBitInByte;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        BitArray bitArray = new BitArray(DB[DBRead_NrOfByteinDB]);
                        bool[] values = new bool[8];
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        TextBox txt;
                        CheckBox chk;
                        switch (DBRead_NrOfBitinByte)
                        {
                            case 0:
                                values[0] = GetBit(DB[DBRead_NrOfByteinDB], 0);
                                //Wyszukanie TextBoxa po jego nazwie
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[0];
                                }
                                break;
                            case 1:
                                values[1] = GetBit(DB[DBRead_NrOfByteinDB], 1);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[1];
                                }
                                break;
                            case 2:
                                values[2] = GetBit(DB[DBRead_NrOfByteinDB], 2);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[2];
                                }

                                break;
                            case 3:
                                values[3] = GetBit(DB[DBRead_NrOfByteinDB], 3);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[3];
                                }

                                break;
                            case 4:
                                values[4] = GetBit(DB[DBRead_NrOfByteinDB], 4);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[4];
                                }

                                break;
                            case 5:
                                values[5] = GetBit(DB[DBRead_NrOfByteinDB], 5);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[5];
                                }

                                break;
                            case 6:
                                values[6] = GetBit(DB[DBRead_NrOfByteinDB], 6);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[6];
                                }

                                break;
                            case 7:
                                values[7] = GetBit(DB[DBRead_NrOfByteinDB], 7);
                                chk = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as CheckBox;
                                if (chk == null)
                                {
                                    //Wyjscie z case'a jesli nie znajdzie checkboxa i bedzie nullem
                                    break;
                                }
                                else
                                {
                                    chk.Checked = values[7];
                                }
                                break;
                        }
                        break;
                    case "BYTE":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
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
                            DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                            DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                            DBRead_TagName = dbTag.TagName;
                            DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                            txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if(txt == null)
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
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
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
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if(txt == null)
                        {
                            break;
                        }
                        else
                        {
                            txt.Text = Convert.ToString(libnodave.getS32from(DB, DBRead_NrOfByteinDB));
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        //Zapis danych do DB i odczyt pliku z zapisywaniem
        public void WriteToDB(string valuetoWrite, string NameofTaginDB)
        {
            //Odczyt listy z tagami do zapisu
            var dbtags = CSVReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBWrite_position = dbTag.TagName.IndexOf('.') - 2;
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
                    byte[] DB = new byte[DBWrite_LengthofDataType];
                    bool values = false;
                    string valuesToString = "";
                    int boolValueFromDB = 0;
                    int boolValue = 0;
                    byte[] BoolToSave = new byte[0];
                    //Read DB - Odczytanie DBka w celu ustalenia jaką wartosc od 0 do 255 jest w danym bajcie
                    PLC.Read(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, DB);
                    //Write BOOL
                    values = GetBit(DB[DBWrite_NrOfByteinDB], DBWrite_NrOfBitinByte);
                    valuesToString = values.ToString();
                    if (valuesToString.ToUpper() == "TRUE")
                    {
                        if (valuetoWrite.ToUpper() == "TRUE" || valuetoWrite.ToUpper() == "")
                        {
                            boolValueFromDB = DB[DBWrite_NrOfByteinDB];
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

                            boolValueFromDB = DB[DBWrite_NrOfByteinDB];
                            boolValueFromDB = boolValueFromDB - boolValue;
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                        }
                    }
                    else if (valuesToString.ToUpper() == "FALSE")
                    {
                        if (valuetoWrite.ToUpper() == "FALSE" || valuetoWrite.ToUpper() == "")
                        {
                            boolValueFromDB = DB[DBWrite_NrOfByteinDB];
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
                            boolValueFromDB = DB[DBWrite_NrOfByteinDB];
                            boolValueFromDB = boolValueFromDB + boolValue;
                            BoolToSave = BitConverter.GetBytes(boolValueFromDB);
                        }
                    }
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, BoolToSave);
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
                    if (valuetoWrite != "")
                    {
                        float fltValue = (float)Convert.ToDouble(valuetoWrite);
                        var i = libnodave.daveToPLCfloat(fltValue);
                        IntBytes = BitConverter.GetBytes(i);
                        PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
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
                    else if (dbTag.DataTypeOfTag.ToUpper() == "INT")
                    {
                        DBReadAlarm_EndDB = dbTag.NumberOfByteInDB + 2;
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
                BitArray bitArray = new BitArray(DB[DBReadAlarm_NrOfByteinDB]);
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
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        
                                        item.Text = DBReadAlarm_AlarmName;
                                        item.BackColor = Color.Red; item.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item);
                                        index[0] = Form1._Form1.listAlarmView.Items.IndexOf(item);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                       itemArchive = MakeList( DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive.BackColor = Color.Red; itemArchive.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive);
                                       index1[0] = Form1._Form1.listView1.Items.IndexOf(itemArchive);
                                    }
                                }
                                else
                                {
                                    //Zabezpieczenie zeby to sie nie wykonywalo jak bedzie pusta lista
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive);
                                        }
                                    }
                                }
                                break;
                            case 1:
                                values[1] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 1);
                                
                                if (values[1].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {                                        
                                        item1.Text = DBReadAlarm_AlarmName;
                                        item1.BackColor = Color.Red; item1.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item1);
                                        index[1] = Form1._Form1.listAlarmView.Items.IndexOf(item1);                                       
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive1 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive1.BackColor = Color.Red; itemArchive1.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive1);
                                        index1[1] = Form1._Form1.listView1.Items.IndexOf(itemArchive1);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item1);                                         
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive1);
                                        }
                                    }
                                }
                                break;
                            case 2:
                                values[2] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 2);
                                if (values[2].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item2.Text = DBReadAlarm_AlarmName;
                                        item2.BackColor = Color.Red; item2.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item2);
                                        index[2] = Form1._Form1.listAlarmView.Items.IndexOf(item2);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive2 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive2.BackColor = Color.Red; itemArchive2.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive2);
                                        index1[2] = Form1._Form1.listView1.Items.IndexOf(itemArchive2);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item2);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive2);
                                        }
                                    }
                                }
                                break;
                            case 3:
                                values[3] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 3);
                                if (values[3].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item3.Text = DBReadAlarm_AlarmName;
                                        item3.BackColor = Color.Red; item3.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item3);
                                        index[3] = Form1._Form1.listAlarmView.Items.IndexOf(item3);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive3 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive3.BackColor = Color.Red; itemArchive3.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive3);
                                        index1[3] = Form1._Form1.listView1.Items.IndexOf(itemArchive3);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item3);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive3);
                                        }
                                    }
                                }
                                break;
                            case 4:
                                values[4] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 4);
                                if (values[4].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item4.Text = DBReadAlarm_AlarmName;
                                        item4.BackColor = Color.Red; item4.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item4);
                                        index[4] = Form1._Form1.listAlarmView.Items.IndexOf(item4);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive4 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive4.BackColor = Color.Red; itemArchive4.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive4);
                                        index1[4] = Form1._Form1.listView1.Items.IndexOf(itemArchive4);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item4);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive4);
                                        }
                                    }
                                }
                                break;
                            case 5:
                                values[5] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 5);
                                if (values[5].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item5.Text = DBReadAlarm_AlarmName;
                                        item5.BackColor = Color.Red; item5.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item5);
                                        index[5] = Form1._Form1.listAlarmView.Items.IndexOf(item5);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive5 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive5.BackColor = Color.Red; itemArchive5.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive5);
                                        index1[5] = Form1._Form1.listView1.Items.IndexOf(itemArchive5);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item5);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive5);
                                        }
                                    }
                                }
                                break;
                            case 6:
                                values[6] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 6);
                                if (values[6].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item6.Text = DBReadAlarm_AlarmName;
                                        item6.BackColor = Color.Red; item6.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item6);
                                        index[6] = Form1._Form1.listAlarmView.Items.IndexOf(item6);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive6 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive6.BackColor = Color.Red; itemArchive6.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive6);
                                        index1[6] = Form1._Form1.listView1.Items.IndexOf(itemArchive6);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item6);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive6);
                                        }
                                    }
                                }
                                break;
                            case 7:
                                values[7] = GetBit(DB[DBReadAlarm_NrOfByteinDB], 7);
                                if (values[7].ToString().ToUpper() == "TRUE")
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        item7.Text = DBReadAlarm_AlarmName;
                                        item7.BackColor = Color.Red; item7.ForeColor = Color.Black;
                                        Form1._Form1.listAlarmView.Items.Add(item7);
                                        index[7] = Form1._Form1.listAlarmView.Items.IndexOf(item7);
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) == null)
                                    {
                                        itemArchive7 = MakeList(DateTime.Now.ToString(), DBReadAlarm_TagName, DBReadAlarm_AlarmName);
                                        itemArchive7.BackColor = Color.Red; itemArchive7.ForeColor = Color.Black;
                                        Form1._Form1.listView1.Items.Add(itemArchive);
                                        index1[7] = Form1._Form1.listView1.Items.IndexOf(itemArchive7);
                                    }
                                }
                                else
                                {
                                    if (Form1._Form1.listAlarmView.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listAlarmView.Items.Count > 0)
                                        {
                                            Form1._Form1.listAlarmView.Items.Remove(item7);
                                        }
                                    }
                                    if (Form1._Form1.listView1.FindItemWithText(DBReadAlarm_AlarmName) != null)
                                    {
                                        if (Form1._Form1.listView1.Items.Count > 0)
                                        {
                                            Form1._Form1.listView1.Items.Remove(itemArchive7);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    case "INT":
                        NrOfMessage = libnodave.getS16from(DB, DBReadAlarm_NrOfByteinDB);
                        Form1._Form1.listBox1.SelectionMode = SelectionMode.MultiSimple;
                        Form1._Form1.listBox1.SetSelected(NrOfMessage, true);
                        break;
                }

            }
        }

        //metoda do zmiany kolor selection item w listBox
        public void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            //Jesli item jest selected to zmień kolor
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.Yellow); 

            //Draw background kolor dla kazdego itema
            e.DrawBackground();
            //Obecny item text
            e.Graphics.DrawString(Form1._Form1.listBox1.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            //Jesli item jest selected to rysuj prostokat wokol itema
            e.DrawFocusRectangle();
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
                DBReadIO_TagName = dbTag.TagName;
                DBReadIO_DataTypeofTag = dbTag.DataTypeOfTag;
                BitArray bitArray = new BitArray(DB[DBReadIO_NrOfByteinDB]);
                bool[] values = new bool[8];
                //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                DBReadIO_position1 = dbTag.TagName.IndexOf(".") + 1;
                DBReadIO_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBReadIO_position1);
                TextBox txt;
                switch (DBReadIO_NrOfBitinByte)
                {
                    case 0:
                        values[0] = GetBit(DB[DBReadIO_NrOfByteinDB], 0);
                        //Wyszukanie TextBoxa po jego nazwie i ustawienie kolor uw zaleznosci od stanu value[x]
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[0] == true)
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 1:
                        values[1] = GetBit(DB[DBReadIO_NrOfByteinDB], 1);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[1])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 2:
                        values[2] = GetBit(DB[DBReadIO_NrOfByteinDB], 2);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[2])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 3:
                        values[3] = GetBit(DB[DBReadIO_NrOfByteinDB], 3);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[3])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 4:
                        values[4] = GetBit(DB[DBReadIO_NrOfByteinDB], 4);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[4])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 5:
                        values[5] = GetBit(DB[DBReadIO_NrOfByteinDB], 5);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[5])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 6:
                        values[6] = GetBit(DB[DBReadIO_NrOfByteinDB], 6);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[6])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                    case 7:
                        values[7] = GetBit(DB[DBReadIO_NrOfByteinDB], 7);
                        txt = Form1._Form1.Controls.Find($"{DBReadIO_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        if (values[7])
                        {
                            txt.BackColor = Color.Green;
                        }
                        else
                        {
                            txt.BackColor = Color.White;
                        }
                        break;
                }

            }

        }
    }
}
