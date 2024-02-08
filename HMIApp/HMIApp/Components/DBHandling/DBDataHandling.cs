using HMIApp.Components.CSVReader.Models;
using HMIApp.Components.CSVReader;
using HMIApp.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HMIApp.Components.DBHandling
{
    //Testowo zrobiona klasa
    public class DBDataHandling
    {

        public int EndDB { get; private set; }
        public int StartDB { get; private set; }
        public string NumberofDB { get; private set; }

        public List<DBTag> Dbtags { get; private set; }

        public byte[] DB { get; private set; }

        public DBDataHandling(List<DBTag> dbtags, string numberofDB, int startDB, int endDB )
        {
            this.Dbtags = dbtags;
            this.NumberofDB = numberofDB;
            this.StartDB = startDB;
            this.EndDB = endDB;

        }

        //Stworzenie obiektu z konfiguracja sterownika
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000000);
        
        public void RunInitPLC()
        {
            PLC.Init();
        }

        public void ReadFromDB()
        {
            DB = new byte[EndDB];
      
            //Read DB
            PLC.Read(int.Parse(NumberofDB), StartDB, EndDB, DB);

            foreach (var dbTag in Dbtags)
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
        public void WriteToDB(string DataTypeofTag, int NrOfByteinDB, int LengthofDataType, int NrOfBitinByte)
        {
            if (DataTypeofTag.ToUpper() == "BOOL")
            {
                //Write BOOL
                byte[] Byte = new byte[8];
                int boolValue;
                switch (NrOfBitinByte)
                {
                    case 0:
                        boolValue = 1;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 1:
                        boolValue = 2;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 2:
                        boolValue = 4;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 3:
                        boolValue = 8;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 4:
                        boolValue = 16;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 5:
                        boolValue = 32;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 6:
                        boolValue = 64;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    case 7:
                        boolValue = 128;
                        Byte = BitConverter.GetBytes(boolValue);
                        PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, Byte);
                        break;
                    default:
                        break;
                }
            }
        }
        public void WriteToDB(string DataTypeofTag, int NrOfByteinDB, int LengthofDataType)
        {


            switch (DataTypeofTag.ToUpper())
            {
                case "BYTE":
                    //Write BYTE
                    string temp5 = "5";
                    short ByteValue = Convert.ToByte(temp5);
                    byte[] intBytes4 = BitConverter.GetBytes(ByteValue);
                    PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, intBytes4);
                    break;
                case "INT":
                    //write INT
                    string temp3 = "15";
                    short intValue = Convert.ToInt16(temp3);
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    Array.Reverse(intBytes);
                    PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, intBytes);
                    break;
                case "REAL":
                    //Write Float
                    string temp2 = "10";
                    float fltValue = (float)Convert.ToDouble(temp2);
                    var i = libnodave.daveToPLCfloat(fltValue);
                    byte[] intBytes1 = BitConverter.GetBytes(i);
                    PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, intBytes1);
                    break;
                case "DINT":
                    //Write DINT
                    string temp6 = "18";
                    int intValue1 = Convert.ToInt32(temp6);
                    byte[] intBytes3 = BitConverter.GetBytes(intValue1);
                    Array.Reverse(intBytes3);
                    PLC.Write(int.Parse(NumberofDB), NrOfByteinDB, LengthofDataType, intBytes3);
                    break;
                default:
                    break;

            }

        }

    }
}
