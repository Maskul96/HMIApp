
namespace HMIApp.Archivizations.Models
{
    public class ArchivizationModelExtended
    {
        public string DateTime { get; set; }

        public string NrOfCard { get; set; }

        public string Message { get; set; }

        //Dane referencji
        public string ReferenceNumber { get; set; }
        public string NameOfClient { get; set; }
        public bool PrzeciskanieP1 { get; set; }
        public bool MontazOslonkiP2 { get; set; }
        public bool OetickerP3 { get; set; }
        public bool DyszaWahliwaP4 { get; set; }
        public bool SmarTulipP5 { get; set; }
        public bool SmarPrzegubP6 { get; set; }
        public bool TraceUpP7 { get; set; }
        public bool TraceUpZapisP8 { get; set; }
        public bool RFIDGlowicaGornaP9 { get; set; }
        public bool RFIDPlytaSmarujacaP10 { get; set; }
        public bool RFIDSzczekiOslonkiP11 { get; set; }
        public bool RFIDGniazdoPrzegubuP12 { get; set; }

        public float PozwyjeciaOsi { get; set; }
        public float PozOetickera { get; set; }

        //Przeciskanie
        public float Przeciskanie_PozStartowa { get; set; }
        public float Przeciskanie_DojazdWolny { get; set; }
        public float Przeciskanie_PoczCzytSily { get; set; }
        public float Przeciskanie_KoniecCzytSily { get; set; }
        public int Przeciskanie_SilaMin { get; set; }
        public int Przeciskanie_SilaMax { get; set; }

        //Oslonka
        public float Oslonka_PozStartowa { get; set; }
        public float Oslonka_PozSmarowania { get; set; }
        public float Oslonka_PozNakladania { get; set; }
        public float Oslonka_PozPowrotu { get; set; }

        //Dysza Wahliwa
        public float DyszaWahliwa_PozPionowa { get; set; }
        public float DyszaWahliwa_PozPozioma { get; set; }
        public float DyszaWahliwa_PozdyszywOslonce { get; set; }
        public float DyszaWahliwa_PozZjazduOslonkiSmarowanie { get; set; }

        //Smarowania
        public float Smarowanie_DawkaPrzegub { get; set; }
        public float Smarowanie_TolDawkiPrzegub { get; set; }
        public int Smarowanie_RodzajSmaruPrzegub { get; set; }
        public float Smarowanie_DawkaTulip { get; set; }
        public float Smarowanie_TolDawkiTulip { get; set; }
        public int Smarowanie_RodzajSmaruTulip { get; set; }

        //RFID
        public int TagRFID_GlowicaGorna { get; set; }
        public int TagRFID_PlytaSmar { get; set; }
        public int TagRFID_SzczekiOslonki { get; set; }
        public int TagRFID_Przegub { get; set; }
    }
}
