

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

            Map(x => x.NrOfShift)
                    .Index(3)
                     .Name("Numer zmiany");

            Map(x => x.NrOfCard)
                    .Index(1)
                     .Name("Numer karty");

            Map(x => x.Message)
                     .Index(0)
                     .Name("Opis zdarzenia");

            Map(x => x.ReferenceNumber)
                   .Index(4)
                   .Name("Numer referencji");

            Map(x => x.NameOfClient)
                     .Index(5)
                     .Name("Nazwa klienta");

            Map(x => x.PrzeciskanieP1)
           .Index(6)
           .Name("PrzeciskanieP1");

            Map(x => x.MontazOslonkiP2)
       .Index(7)
       .Name("MontazOslonkiP2");

            Map(x => x.OetickerP3)
       .Index(8)
       .Name("OetickerP3");
            Map(x => x.DyszaWahliwaP4)
       .Index(9)
       .Name("DyszaWahliwaP4");

            Map(x => x.SmarTulipP5)
       .Index(10)
       .Name("SmarTulipP5");
            Map(x => x.SmarPrzegubP6)
       .Index(11)
       .Name("SmarPrzegubP6");

            Map(x => x.TraceUpP7)
       .Index(12)
       .Name("TraceUpP7");

            Map(x => x.TraceUpZapisP8)
       .Index(13)
       .Name("TraceUpZapisP8");

            Map(x => x.RFIDGlowicaGornaP9)
     .Index(14)
     .Name("RFIDGlowicaGornaP9");

            Map(x => x.RFIDPlytaSmarujacaP10)
   .Index(15)
   .Name("RFIDPlytaSmarujacaP10");

            Map(x => x.RFIDSzczekiOslonkiP11)
   .Index(16)
   .Name("RFIDSzczekiOslonkiP11");

            Map(x => x.RFIDGniazdoPrzegubuP12)
   .Index(17)
   .Name("RFIDGniazdoPrzegubuP12");

            Map(x => x.PozwyjeciaOsi)
  .Index(18)
  .Name("PozwyjeciaOsi");

            Map(x => x.PozOetickera)
 .Index(19)
 .Name("PozOetickera");

            Map(x => x.Przeciskanie_PozStartowa)
.Index(20)
.Name("Przeciskanie_PozStartowa");

            Map(x => x.Przeciskanie_DojazdWolny)
.Index(21)
.Name("Przeciskanie_DojazdWolny");

            Map(x => x.Przeciskanie_PoczCzytSily)
    .Index(22)
    .Name("Przeciskanie_PoczCzytSily");

            Map(x => x.Przeciskanie_KoniecCzytSily)
   .Index(23)
   .Name("Przeciskanie_KoniecCzytSily");

            Map(x => x.Przeciskanie_SilaMax)
   .Index(24)
   .Name("Przeciskanie_SilaMax");

            Map(x => x.Przeciskanie_SilaMin)
 .Index(25)
 .Name("Przeciskanie_SilaMin");

            Map(x => x.Oslonka_PozStartowa)
 .Index(26)
 .Name("Oslonka_PozStartowa");

            Map(x => x.Oslonka_PozNakladania)
 .Index(27)
 .Name("Oslonka_PozNakladania");

            Map(x => x.Oslonka_PozSmarowania)
.Index(28)
.Name("Oslonka_PozSmarowania");

            Map(x => x.Oslonka_PozPowrotu)
.Index(29)
.Name("Oslonka_PozPowrotu");

            Map(x => x.DyszaWahliwa_PozPionowa)
.Index(30)
.Name("DyszaWahliwa_PozPionowa");

            Map(x => x.DyszaWahliwa_PozPozioma)
.Index(31)
.Name("DyszaWahliwa_PozPozioma");

            Map(x => x.DyszaWahliwa_PozdyszywOslonce)
.Index(32)
.Name("DyszaWahliwa_PozdyszywOslonce");

            Map(x => x.DyszaWahliwa_PozZjazduOslonkiSmarowanie)
.Index(33)
.Name("DyszaWahliwa_PozZjazduOslonkiSmarowanie");

            Map(x => x.Smarowanie_DawkaTulip)
.Index(34)
.Name("Smarowanie_DawkaTulip");

            Map(x => x.Smarowanie_TolDawkiTulip)
.Index(35)
.Name("Smarowanie_TolDawkiTulip");

            Map(x => x.Smarowanie_RodzajSmaruTulip)
.Index(36)
.Name("Smarowanie_RodzajSmaruTulip");

            Map(x => x.Smarowanie_DawkaPrzegub)
.Index(37)
.Name("Smarowanie_DawkaTulip");

            Map(x => x.Smarowanie_TolDawkiPrzegub)
.Index(38)
.Name("Smarowanie_TolDawkiTulip");

            Map(x => x.Smarowanie_RodzajSmaruPrzegub)
.Index(39)
.Name("Smarowanie_RodzajSmaruTulip");

            Map(x => x.Smarowanie_RodzajSmaruPrzegub)
.Index(40)
.Name("Smarowanie_RodzajSmaruTulip");

            Map(x => x.TagRFID_GlowicaGorna)
.Index(41)
.Name("TagRFID_GlowicaGorna");

            Map(x => x.TagRFID_PlytaSmar)
.Index(42)
.Name("TagRFID_PlytaSmar");

            Map(x => x.TagRFID_Przegub)
.Index(43)
.Name("TagRFID_Przegub");

            Map(x => x.TagRFID_SzczekiOslonki)
.Index(44)
.Name("TagRFID_SzczekiOslonki");
        }
        
    }
}
