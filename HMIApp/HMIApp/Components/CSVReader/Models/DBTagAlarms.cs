
namespace HMIApp.Components.CSVReader.Models
{
    public class DBTagAlarms
    {
        public string TagName { get; set; }
        public string DataTypeOfTag { get; set; }
        public int NumberOfByteInDB { get; set; }
        public int NumberOfBitInByte { get; set; }
        public int LengthDataType { get; set; }

        public string NameofAlarm { get; set; }
    }
}
