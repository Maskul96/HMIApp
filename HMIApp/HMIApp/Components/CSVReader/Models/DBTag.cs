
namespace HMIApp.Components.CSVReader.Models
{
    public class DBTag
    {

        public string TagName { get; set; }
        public string DataTypeOfTag { get; set; }
        public int NumberOfByteInDB { get; set; }
        public int NumberOfBitInByte { get; set; }
        public int LengthDataType { get; set; }
    }
}
