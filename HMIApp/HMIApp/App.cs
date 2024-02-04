using HMIApp.Components;
using HMIApp.Components.CSVReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMIApp
{
    public class App :iApp
    {
        private readonly iCSVReader _csvReader;
        public List<DBTag> dbtags { get; set; }
        //ponizej dwa konstruktory pierwszy parametryzowany drugi bez parametrow
        public App(iCSVReader csvReader)
        {
            _csvReader = csvReader;
        }
        public App() 
        { 

        }

        public void Run()
        {
            var dbtags = _csvReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");

            foreach (var dbTag in dbtags)
            {
                //Wyciagniecie z nazwy DBka jego numer
                int position = dbTag.TagName.IndexOf('.')-2;
                dbTag.TagName = dbTag.TagName.Substring(2, position);

                //MessageBox.Show($"Nazwa Taga z DB: {dbTag.TagName}, Typ danych z DB: {dbTag.DataTypeOfTag}, Numer byte z DB: {dbTag.NumberOfByteInDB}" +
                //    $"Numer byte z DB: {dbTag.NumberOfBitInByte}, Dlugosc danej z DB: {dbTag.LengthDataType}");

            }

            
        }
    }
}
