using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HMIApp.Components.CSVReader.Models;

namespace HMIApp.Components.CSVReader.Extensions
{
    public static class DBTagExtensions
    {
        //metoda pozwala uzyskac DBTagi bezposrednio wypelnione z pliku z ominietymi przecinkami
        public static IEnumerable<DBTag> ToDBTag(this IEnumerable<string> source)
        {
            foreach (var item in source)
            {
                var columns = item.Split(',');

                yield return new DBTag
                {
                    TagName = columns[0],
                    DataTypeOfTag = columns[1],
                    NumberOfByteInDB = int.Parse(columns[2]),
                    NumberOfBitInByte = int.Parse(columns[3]),
                    LengthDataType = int.Parse(columns[4])
                };
            }
        }
    }
}
