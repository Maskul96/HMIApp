using HMIApp.Components.CSVReader.Models;
using System.Collections.Generic;

namespace HMIApp.Components.CSVReader.Extensions
{
    public static class DBTagAlarmsExtensions
    {
        //metoda pozwala uzyskac DBTagi bezposrednio wypelnione z pliku z ominietymi przecinkami w postaci stringow
        public static IEnumerable<DBTagAlarms> ToDBTagAlarms(this IEnumerable<string> source)
        {
            foreach (var item in source)
            {
                var columns = item.Split(',');

                yield return new DBTagAlarms
                {
                    TagName = columns[0],
                    DataTypeOfTag = columns[1],
                    NumberOfByteInDB = int.Parse(columns[2]),
                    NumberOfBitInByte = int.Parse(columns[3]),
                    LengthDataType = int.Parse(columns[4]),
                    NameofAlarm = columns[5]
                };
            }
        }
    }
}
