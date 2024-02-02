using HMIApp.Components;
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
        public App(iCSVReader csvReader)
        {
            _csvReader = csvReader;
        }
        public App() { }

        public void Run()
        {
            MessageBox.Show("Hello");
            var dbtags = _csvReader.DBStructure("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");

            foreach (var dbTag in dbtags)
            {
                MessageBox.Show($"Nazwa Taga z DB: {dbTag.TagName}, Typ danych z DB: {dbTag.DataTypeOfTag}, Numer byte z DB: {dbTag.NumberOfByteInDB}" +
                    $"Numer byte z DB: {dbTag.NumberOfBitInByte}, Dlugosc danej z DB: {dbTag.LengthDataType}");

            }
            
        }
    }
}
