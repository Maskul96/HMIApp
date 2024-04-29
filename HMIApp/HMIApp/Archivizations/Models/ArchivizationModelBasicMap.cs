using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModelBasicMap :ClassMap<ArchivizationModelBasic>
    {
        public ArchivizationModelBasicMap() 
        {
           // Map(x => x.Id)
              //  .Index(0)
               // .Name("Id");

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
