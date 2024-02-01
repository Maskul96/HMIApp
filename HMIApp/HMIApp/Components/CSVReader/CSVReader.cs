using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMIApp.Components.CSVReader.Extensions;
using HMIApp.Components.CSVReader.Models;

namespace HMIApp.Components.CSVReader
{
    public class CSVReader : iCSVReader
    {
        public List<DBTag> DBStructure(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return new List<DBTag>();
            }

            //Metoda skip omijamy pierwszy wiersz gdzie sa naglowki
            //Metoda Where sprawdza zeby nie czytac pustych wierszy np na koncu
            var dbtags =
                File.ReadAllLines(filePath)
                .Skip(1)
                .Where(x => x.Length > 1)
                .ToDBTag();


            return dbtags.ToList();
        }
    }
}
