using HMIApp.Components;
using HMIApp.Components.CSVReader;
using HMIApp.Components.CSVReader.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

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

        //Metoda uruchamiająca komunikacje z PLC
        public void RunInitPLC()
        {
            PLC.Init();
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
                    switch(dbTag.DataTypeOfTag.ToUpper())
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

                switch (dbTag.DataTypeOfTag.ToUpper())
                {
                    case "BOOL":
                        //Read BOOL               
                        DBRead_NrOfBitinByte = dbTag.NumberOfBitInByte;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                        BitArray bitArray = new BitArray(DB[DBRead_NrOfByteinDB]);
                        bool[] values = new bool[8];
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") +1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        TextBox txt;
                        switch (DBRead_NrOfBitinByte)
                        {
                            case 0:
                                values[0] = GetBit(DB[DBRead_NrOfByteinDB], 0);
                                //Wyszukanie TextBoxa po jego nazwie
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[0].ToString();
                                break;
                            case 1:
                                values[1] = GetBit(DB[DBRead_NrOfByteinDB], 1);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[1].ToString();
                                break;
                            case 2:
                                values[2] = GetBit(DB[DBRead_NrOfByteinDB], 2);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[2].ToString();
                                break;
                            case 3:
                                values[3] = GetBit(DB[DBRead_NrOfByteinDB], 3);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[3].ToString();
                                break;
                            case 4:
                                values[4] = GetBit(DB[DBRead_NrOfByteinDB], 4);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[4].ToString();
                                break;
                            case 5:
                                values[5] = GetBit(DB[DBRead_NrOfByteinDB], 5);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[5].ToString();
                                break;
                            case 6:
                                values[6] = GetBit(DB[DBRead_NrOfByteinDB], 6);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[6].ToString();
                                break;
                            case 7:
                                values[7] = GetBit(DB[DBRead_NrOfByteinDB], 7);
                                txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                                txt.Text = values[7].ToString();
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
                        txt.Text = Convert.ToString(DB[DBRead_NrOfByteinDB]);
                        break;
                    case "INT":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        txt.Text = Convert.ToString(libnodave.getS16from(DB, DBRead_NrOfByteinDB));
                        break;
                    case "REAL":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        txt.Text = Convert.ToString(libnodave.getFloatfrom(DB, DBRead_NrOfByteinDB));
                        break;
                    case "DINT":
                        //Wyszukanie samej nazwy Taga, która odpowiada 1:1 nazwie TextBoxa
                        DBRead_position1 = dbTag.TagName.IndexOf(".") + 1;
                        DBRead_NameofTagWithoutNumberofDB = dbTag.TagName.Substring(DBRead_position1);
                        DBRead_TagName = dbTag.TagName;
                        DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;

                        txt = Form1._Form1.Controls.Find($"{DBRead_NameofTagWithoutNumberofDB}", true).FirstOrDefault() as TextBox;
                        txt.Text = Convert.ToString(libnodave.getS32from(DB, DBRead_NrOfByteinDB));
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

    }
}
