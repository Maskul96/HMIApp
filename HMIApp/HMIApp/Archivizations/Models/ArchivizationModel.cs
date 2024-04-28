using CsvHelper.Configuration.Attributes;

namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModel 
    {
       // public int Id { get; set; }

        public string DateTime { get; set; }

        public string NrOfCard { get; set; }

        public string Message { get; set; }
    }
}
