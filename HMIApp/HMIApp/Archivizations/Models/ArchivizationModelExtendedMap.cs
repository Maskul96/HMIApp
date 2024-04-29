
using CsvHelper.Configuration;

namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModelExtendedMap : ClassMap<ArchivizationModelExtended>
    {
        public ArchivizationModelExtendedMap()
        {
            Map(x => x.DateTime)
                .Index(2)
                .TypeConverterOption.Format("dd/MM/yyyy HH:mm:ss")
                .Name("Data i Godzina DD-MM-YY H:M");


            Map(x => x.NrOfCard)
                    .Index(1)
                     .Name("Numer karty");

            Map(x => x.Message)
                     .Index(0)
                     .Name("Opis zdarzenia");


        }
    }
}
