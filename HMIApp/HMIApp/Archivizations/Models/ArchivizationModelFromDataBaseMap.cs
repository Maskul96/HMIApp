

using CsvHelper.Configuration;
using HMIApp.Components.DataBase;

namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModelFromDataBaseMap : ClassMap<ArchivizationModelExtendedDataBase>
    {
        public ArchivizationModelFromDataBaseMap()
        {
            Map(x => x.DateAndTime)
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
