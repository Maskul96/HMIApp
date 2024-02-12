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

        //Zmienne do DBka do zapisywania
        public int DBWrite_position;
        public string DBWrite_NumberOfDB;
        public string DBWrite_DataTypeofTag;
        public int DBWrite_LengthofDataType;
        public int DBWrite_NrOfByteinDB;
        public int DBWrite_NrOfBitinByte;
        //ponizej dwa konstruktory pierwszy parametryzowany drugi bez parametrow
        public App(iCSVReader csvReader)
        {
            _csvReader = csvReader;
        }
        public App()
        {

        }

        //Stworzenie obiektu z konfiguracja sterownika
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000000);
        CSVReader CSVReader = new CSVReader();
        public void RunInitPLC()
        {
            PLC.Init();
        }

        //Odczyt z pliku CSV i od razu odczyt danych z DBka
        //Tutaj metoda powinna zwracać w postaci listy odczytane dane i udostępniac żeby móc je użyć w Form1 i podpisac pod obiekty
        public void ReadFromDB()
        {
            var dbtags = CSVReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                DBRead_position = dbTag.TagName.IndexOf('.') - 2;
                DBRead_NumberOfDB = dbTag.TagName.Substring(2, DBRead_position);

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
                    case "BOOL":
                        //Read BOOL
                        var value = DB[0];
                        var values = new bool[8];
                        values[0] = (value & 1) == 0 ? false : true;
                        values[1] = (value & 2) == 0 ? false : true;
                        values[2] = (value & 4) == 0 ? false : true;
                        values[3] = (value & 8) == 0 ? false : true;
                        values[4] = (value & 16) == 0 ? false : true;
                        values[5] = (value & 32) == 0 ? false : true;
                        values[6] = (value & 64) == 0 ? false : true;
                        values[7] = (value & 128) == 0 ? false : true;
                        break;
                    case "BYTE":
                        var l = DB[2];
                        break;
                    case "INT":
                        //Read INT16
                        var j = libnodave.getS16from(DB, 4);
                        break;
                    case "REAL":
                        //Read FLOAT
                        var k = libnodave.getFloatfrom(DB, 8);
                        break;
                    case "DINT":
                        //Read DINT
                        var m = libnodave.getS32from(DB, 12);
                        break;
                    default:
                        break;
                }

            }
        }

        //Zapis danych do DB
        public void WriteToDB(string NameofTaginDB)
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
                else
                {
                    throw new Exception("Nie znaleziono Taga");
                }

            }

            switch (DBWrite_DataTypeofTag.ToUpper())
            {
                case "BOOL":
                    //Write BOOL
                    byte[] Byte = new byte[8];
                    int boolValue;
                    switch (DBWrite_NrOfBitinByte)
                    {
                        case 0:
                            boolValue = 1;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 0, DBWrite_LengthofDataType, Byte);
                            break;
                        case 1:
                            boolValue = 2;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 1, DBWrite_LengthofDataType, Byte);
                            break;
                        case 2:
                            boolValue = 4;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 2, DBWrite_LengthofDataType, Byte);
                            break;
                        case 3:
                            boolValue = 8;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 3, DBWrite_LengthofDataType, Byte);
                            break;
                        case 4:
                            boolValue = 16;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 4, DBWrite_LengthofDataType, Byte);
                            break;
                        case 5:
                            boolValue = 32;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 5, DBWrite_LengthofDataType, Byte);
                            break;
                        case 6:
                            boolValue = 64;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 6, DBWrite_LengthofDataType, Byte);
                            break;
                        case 7:
                            boolValue = 128;
                            Byte = BitConverter.GetBytes(boolValue);
                            PLC.Write(int.Parse(DBWrite_NumberOfDB), 7, DBWrite_LengthofDataType, Byte);
                            break;
                        default:
                            break;
                    }
                    break;
                case "BYTE":
                    //Write BYTE
                    string temp5 = "5";
                    short ByteValue = Convert.ToByte(temp5);
                    byte[] intBytes4 = BitConverter.GetBytes(ByteValue);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes4);
                    break;
                case "INT":
                    //write INT
                    string temp3 = "15";
                    short intValue = Convert.ToInt16(temp3);
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    Array.Reverse(intBytes);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes);
                    break;
                case "REAL":
                    //Write Float
                    string temp2 = "10";
                    float fltValue = (float)Convert.ToDouble(temp2);
                    var i = libnodave.daveToPLCfloat(fltValue);
                    byte[] intBytes1 = BitConverter.GetBytes(i);
                    PLC.Write(int.Parse(DBWrite_NumberOfDB), DBWrite_NrOfByteinDB, DBWrite_LengthofDataType, intBytes1);
                    break;
                case "DINT":
                    //Write DINT
                    string temp6 = "18";
                    int intValue1 = Convert.ToInt32(temp6);
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
