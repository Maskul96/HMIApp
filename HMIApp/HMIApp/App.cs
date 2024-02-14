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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMIApp
{
    public class App : iApp
    {
        //Deklaracja zmiennych
        //Zmienne do DBka do odczytania
        private readonly iCSVReader _csvReader;
        public int DBRead_position;
        public string DBRead_NumberOfDB;
        public int DBRead_StartDB;
        public int DBRead_EndDB;
        public int DBRead_NrOfByteinDB;
        public Form1 obj;

        //Zmienne do DBka do zapisywania
        public int DBWrite_position;
        public string DBWrite_NumberOfDB;
        public string DBWrite_DataTypeofTag;
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
        CSVReader CSVReader = new CSVReader();
        public void RunInitPLC()
        {
            PLC.Init();
        }

        //Odczyt z pliku CSV i od razu odczyt danych z DBka
        public void ReadFromDB()
        {
            var dbtags = CSVReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBRead_position = dbTag.TagName.IndexOf('.') - 2;
                DBRead_NumberOfDB = dbTag.TagName.Substring(2, DBRead_position);
                DBRead_NrOfByteinDB = dbTag.NumberOfByteInDB;
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych
                if (dbtags.First() == dbTag)
                {
                    DBRead_StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    DBRead_EndDB = dbTag.NumberOfByteInDB;
                }
            }

            byte[] DB = new byte[DBRead_EndDB];
            //Read DB
            PLC.Read(int.Parse(DBRead_NumberOfDB), DBRead_StartDB, DBRead_EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                switch (dbTag.DataTypeOfTag.ToUpper())
                {
                    //ODCZYTYWANIE BOOLI - NIE DZIALA JESZCZE POPRAWNIE - POTESTOWAC I TO POPRAWIC
                    case "BOOL":
                        //Read BOOL               
                        var value = DB[0];
                        var values = new bool[8];
                        //To ponizej trzeba bedzie przerobic pozniej na cos mądrzejszego
                        switch (value)
                        {
                            case 1:
                                Form1._Form1.label5.Text = values[0].ToString();
                                break;
                            case 2:
                                Form1._Form1.label6.Text = values[1].ToString();
                                break;
                            case 4:
                                Form1._Form1.label7.Text = values[2].ToString();
                                break;
                            case 8:
                                Form1._Form1.label8.Text = values[3].ToString();
                                break;
                            case 16:
                                Form1._Form1.label9.Text = values[4].ToString();
                                break;
                            case 32:
                                Form1._Form1.label10.Text = values[5].ToString();
                                break;
                            case 64:
                                Form1._Form1.label11.Text = values[6].ToString();
                                break;
                            case 128:
                                Form1._Form1.label12.Text = values[7].ToString();
                                break;
                        }
                        break;
                    case "BYTE":
                        Form1._Form1.label13.Text = Convert.ToString(DB[1]);
                        Form1._Form1.label14.Text = Convert.ToString(DB[2]);
                        break;
                    case "INT":
                        //Read INT16
                        Form1._Form1.label15.Text = Convert.ToString(libnodave.getS16from(DB, 4));
                        Form1._Form1.label16.Text = Convert.ToString(libnodave.getS16from(DB, 6));
                        break;
                    case "REAL":
                        //Read FLOAT
                        Form1._Form1.label17.Text = Convert.ToString(libnodave.getFloatfrom(DB, 8));
                        break;
                    case "DINT":
                        //Read DINT
                        Form1._Form1.label18.Text = Convert.ToString(libnodave.getS32from(DB, 12));
                        break;
                    default:
                        break;
                }

            }
        }

        //Zapis danych do DB
        public void WriteToDB(string valuetoWrite, string NameofTaginDB)
        {

            var dbtags = CSVReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBWrite_position = dbTag.TagName.IndexOf('.') - 2;
                DBWrite_NumberOfDB = dbTag.TagName.Substring(2, DBWrite_position);

                if (NameofTaginDB == dbTag.TagName)
                {
                    DBWrite_NrOfByteinDB = dbTag.NumberOfByteInDB;
                    DBWrite_LengthofDataType = dbTag.LengthDataType;
                    DBWrite_DataTypeofTag = dbTag.DataTypeOfTag;
                    DBWrite_NrOfBitinByte = dbTag.NumberOfBitInByte;
                }

            }

            switch (DBWrite_DataTypeofTag.ToUpper())
            {
                //ZAPISYWANIE BOOLI TEZ NIE DZIALA POPRAWNIE - TEZ TO POPRAWIC
                case "BOOL":
                    //Write BOOL
                    byte[] Byte = new byte[8];
                    int boolValue;
                    switch (DBWrite_NrOfBitinByte)
                    {
                        case 0:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 1;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 1:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 2;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 2:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 4;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 3:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 8;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 4:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 16;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 5:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 32;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 6:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 64;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 7:
                            if (valuetoWrite.ToUpper() == "TRUE")
                            {
                                boolValue = 128;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            else if (valuetoWrite.ToUpper() == "FALSE")
                            {
                                boolValue = 0;
                                Byte = BitConverter.GetBytes(boolValue);
                            }
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        default:
                            break;
                    }
                    break;
                case "BYTE":
                    //Write BYTE
                    short ByteValue = Convert.ToByte(valuetoWrite);
                    byte[] IntBytes = BitConverter.GetBytes(ByteValue);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, IntBytes);
                    break;
                case "INT":
                    //write INT
                    short intValue = Convert.ToInt16(valuetoWrite);
                    byte[] intBytes1 = BitConverter.GetBytes(intValue);
                    Array.Reverse(intBytes1);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes1);
                    break;
                case "REAL":
                    //Write Float
                    float fltValue = (float)Convert.ToDouble(valuetoWrite);
                    var i = libnodave.daveToPLCfloat(fltValue);
                    byte[] intBytes2 = BitConverter.GetBytes(i);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes2);
                    break;
                case "DINT":
                    //Write DINT
                    int intValue1 = Convert.ToInt32(valuetoWrite);
                    byte[] intBytes3 = BitConverter.GetBytes(intValue1);
                    Array.Reverse(intBytes3);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes3);
                    break;
                default:
                    break;

            }
        }
        
    }
}
