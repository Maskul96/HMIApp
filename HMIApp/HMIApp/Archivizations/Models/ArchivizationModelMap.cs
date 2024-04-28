using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModelMap :ClassMap<ArchivizationModel>
    {
        public ArchivizationModelMap() 
        {
           // Map(x => x.Id)
              //  .Index(0)
               // .Name("Id");

            Map(x => x.DateTime)
                .Index(0)
                .TypeConverterOption.Format("dd/MM/yyyy HH:mm:ss")
                .Name("Data i Godzina DD-MM-YY H:M");
                

            Map(x => x.NrOfCard)
                .Index(2)
                 .Name("Numer karty");

            Map(x => x.Message)
                 .Index(1)
                 .Name("Opis zdarzenia");
        }

    }
}
