using HMIApp.Components;
using HMIApp.Components.CSVReader.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMIApp
{
    public class App : iApp
    {
        //Deklaracja zmiennych
        private readonly iCSVReader _csvReader;
        private int position;
        private string NumberOfDB;
        private int StartDB;
        private int EndDB;

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

        public void RunInitPLC()
        {
            PLC.Init();   
        }

        //Odczyt z pliku CSV i od razu odczyt danych z DBka
        public void RunReadFromCSVFileandReadFromDB()
        {
            var dbtags = _csvReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                position = dbTag.TagName.IndexOf('.') - 2;
                NumberOfDB = dbTag.TagName.Substring(2, position);
                //Wyciagniecie startowej pozycji i końcowej pozycji DBka - ustalamy dlugosc danych
                if (dbtags.First() == dbTag)
                {
                    StartDB = dbTag.NumberOfByteInDB;
                }
                if (dbtags.Last() == dbTag)
                {
                    EndDB = dbTag.NumberOfByteInDB;
                }
            }

            byte[] DB = new byte[EndDB];
            //Read DB
            PLC.Read(int.Parse(NumberOfDB), StartDB, EndDB, DB);

            foreach (var dbTag in dbtags)
            {
                switch (dbTag.DataTypeOfTag.ToUpper())
                {
                    case "BOOL":
                        var bits = new BitArray(DB[0]);
                        //Read BOOL
                        var a = bits[dbTag.NumberOfBitInByte];
                        break;
                    case "BYTE":
                        var l = DB[6];
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
        public void WriteToDB(string DataTypeofTag,int NrOfByteinDB, int LengthofDataType)
        {
            switch (DataTypeofTag.ToUpper())
            {
                case "BOOL":
                    //Write BOOL
                    string temp4 = "true";
                    bool boolValue = Convert.ToBoolean(temp4);
                    byte[] intBytes2 = BitConverter.GetBytes(boolValue);
                    PLC.Write(int.Parse(NumberOfDB), 6, 1, intBytes2);
                    break;
                case "BYTE":
                    //Write BYTE
                    string temp5 = "test";
                    short ByteValue = Convert.ToByte(temp5);
                    byte[] intBytes4 = BitConverter.GetBytes(ByteValue);
                    PLC.Write(int.Parse(NumberOfDB), 7, 1, intBytes4);
                    break;
                case "INT":
                    //write INT
                    string temp3 = "test";
                    short intValue = Convert.ToInt16(temp3);
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    Array.Reverse(intBytes);
                    PLC.Write(int.Parse(NumberOfDB), 0, 2, intBytes);
                    break;
                case "REAL":
                    //Write Float
                    string temp2 = "test";
                    float fltValue = (float)Convert.ToDouble(temp2);
                    var i = libnodave.daveToPLCfloat(fltValue);
                    byte[] intBytes1 = BitConverter.GetBytes(i);
                    PLC.Write(int.Parse(NumberOfDB), 2, 4, intBytes1);
                    break;
                case "DINT":
                    //Write DINT
                    string temp6 = "test";
                    double intValue1 = Convert.ToInt32(temp6);
                    byte[] intBytes3 = BitConverter.GetBytes(intValue1);
                    Array.Reverse(intBytes3);
                    PLC.Write(int.Parse(NumberOfDB), 0, 2, intBytes3);
                    break;
                default:
                    break;

            }

        }
    }
}
