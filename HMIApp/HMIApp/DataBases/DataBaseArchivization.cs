using HMIApp.Components;
using HMIApp.Components.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
namespace HMIApp.Data
{
    public class DataBaseArchivization :iDataBaseArchivization
    {
        private readonly HMIAppDBContextArchivization _hmiAppDbContextArchivization;
        public string ConnectionString = "";
        Logger _logger;
        public DataBaseArchivization()
        {

        }

        public DataBaseArchivization(HMIAppDBContextArchivization hmiAppDbContextArchivization)
        {
            _hmiAppDbContextArchivization = hmiAppDbContextArchivization;
            //w momencie wywolania konstruktora sprawdzamy czy nasza baza danych jest stworzona - jesli nie jest stworzona to ponizsza metoda ja utworzy
            _hmiAppDbContextArchivization.Database.EnsureCreated();
        }

        //OGARNAC DZIEDZICZENIE TUTAJ z uwagi na powtarzajace sie metody

        //Odczyt pliku konfiguracyjnego z connection stringiem
        public string ReadConfFile(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                ConnectionString = System.IO.File.ReadAllText(filepath);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie mozna otworzyć pliku konfiguracyjnego bazy danych");
                _logger.LogMessage("Nie mozna otworzyc pliku konfiguracyjnego bazy danych");
            }
            return ConnectionString;
        }

        public void Run()
        {
            ReadConfFile("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\DataBaseArchivizationConfiguration.txt");

        }

        public void CountRowsAndDeleteAllData()
        {
            var AllRowsInDataBaseArchive = _hmiAppDbContextArchivization.ArchivizationsForParameters.Count();
            //Kasowanie bazy danych jak jest wiecej niz 100000 tys. wierszy
            if(AllRowsInDataBaseArchive >= 100000)
            {
                Delete();
            }
        }

        //Metoda inserta do bazy parametrow i eventów - ta metode mozna wywolac w momencie odpalenia eventu
        public void InsertToDataBase(string message="")
        {

                _hmiAppDbContextArchivization?.ArchivizationsForParameters.Add(new ArchivizationModelExtendedDataBase()
                {
                    DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    NrOfCard = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text,
                    Message = message,
                    //Parametry
                    ReferenceNumber = Form1._Form1.DB666NrReference.Text,
                    NameOfClient = Form1._Form1.DB666NameOfClient.Text,
                    PrzeciskanieP1 = Form1._Form1.DB666PrzeciskanieP1.Checked,
                    MontazOslonkiP2 = Form1._Form1.DB666MontazOslonkiP2.Checked,
                    OetickerP3 = Form1._Form1.DB666OetickerP3.Checked,
                    DyszaWahliwaP4 = Form1._Form1.DB666DyszaWahliwaP4.Checked,
                    SmarTulipP5 = Form1._Form1.DB666SmarTulipP5.Checked,
                    SmarPrzegubP6 = Form1._Form1.DB666SmarPrzegubP6.Checked,
                    TraceUpP7 = Form1._Form1.DB666TraceUpP7.Checked,
                    TraceUpZapisP8 = Form1._Form1.DB666TraceUpZapisP8.Checked,
                    RFIDGlowicaGornaP9 = Form1._Form1.DB666RFIDGlowicaGornaP9.Checked,
                    RFIDPlytaSmarujacaP10 = Form1._Form1.DB666RFIDPlytaSmarujacaP10.Checked,
                    RFIDSzczekiOslonkiP11 = Form1._Form1.DB666RFIDSzczekiOslonkiP11.Checked,
                    RFIDGniazdoPrzegubuP12 = Form1._Form1.DB666RFIDGniazdoPrzegubuP12.Checked,
                    //Inne
                    PozwyjeciaOsi = float.Parse(Form1._Form1.DB666PozwyjeciaOsi.Text),
                    PozOetickera = float.Parse(Form1._Form1.DB666PozOetickera.Text),
                    //Przeciskanie
                    Przeciskanie_PozStartowa = float.Parse(Form1._Form1.DB666PozStartowa__Przeciskanie.Text),
                    Przeciskanie_DojazdWolny = float.Parse(Form1._Form1.DB666DojazdWolny_Przeciskanie.Text),
                    Przeciskanie_PoczCzytSily = float.Parse(Form1._Form1.DB666PoczCzytSily__Przeciskanie.Text),
                    Przeciskanie_KoniecCzytSily = float.Parse(Form1._Form1.DB666KoniecCzytSily__Przeciskanie.Text),
                    Przeciskanie_SilaMin = int.Parse(Form1._Form1.DB666SilaMin_Przeciskanie.Text),
                    Przeciskanie_SilaMax = int.Parse(Form1._Form1.DB666SilaMax__Przeciskanie.Text),
                    //Oslonka
                    Oslonka_PozStartowa = float.Parse(Form1._Form1.DB666PozStartowa_Oslonka.Text),
                    Oslonka_PozSmarowania = float.Parse(Form1._Form1.DB666PozSmarowania_Oslonka.Text),
                    Oslonka_PozNakladania = float.Parse(Form1._Form1.DB666PozNakladania_Oslonka.Text),
                    Oslonka_PozPowrotu = float.Parse(Form1._Form1.DB666PozPowrotu_Oslonka.Text),
                    //Dysza Wahliwa
                    DyszaWahliwa_PozPionowa = float.Parse(Form1._Form1.DB666PozPionowa_DyszaWahliwa.Text),
                    DyszaWahliwa_PozPozioma = float.Parse(Form1._Form1.DB666PozPozioma_DyszaWahliwa.Text),
                    DyszaWahliwa_PozdyszywOslonce = float.Parse(Form1._Form1.DB666PozDyszyWOslonce_DyszaWahliwa.Text),
                    DyszaWahliwa_PozZjazduOslonkiSmarowanie = float.Parse(Form1._Form1.DB666PozZjazduOslonkiSmarowanie_DyszaWahliwa.Text),
                    //Smarowania
                    Smarowanie_DawkaPrzegub = float.Parse(Form1._Form1.DB666DawkaPrzegub.Text),
                    Smarowanie_TolDawkiPrzegub = float.Parse(Form1._Form1.DB666TolDawkiPrzegub.Text),
                    Smarowanie_RodzajSmaruPrzegub = Form1._Form1.DB666RodzajSmaruPrzegub.SelectedIndex,
                    Smarowanie_DawkaTulip = float.Parse(Form1._Form1.DB666DawkaTulip.Text),
                    Smarowanie_TolDawkiTulip = float.Parse(Form1._Form1.DB666TolDawkiTulip.Text),
                    Smarowanie_RodzajSmaruTulip = Form1._Form1.DB666RodzajSmaruTulip.SelectedIndex,
                    //RFID
                    TagRFID_GlowicaGorna = int.Parse(Form1._Form1.DB666TagRFIDGlowicaGorna.Text),
                    TagRFID_PlytaSmar = int.Parse(Form1._Form1.DB666TagRFIDPlytaSmar.Text),
                    TagRFID_SzczekiOslonki = int.Parse(Form1._Form1.DB666TagRFIDSzczekiOslonki.Text),
                    TagRFID_Przegub = int.Parse(Form1._Form1.DB666TagRFIDPrzegub.Text)
                });

                _hmiAppDbContextArchivization.SaveChanges();
            
        }

        //metoda select z uzyciem LINQ parametrów referencji z bazy danych
        public void SelectFromDataBase(string DateTimeStart,string DateTimeEnd)
        {
            if (DateTimeStart != "" && DateTimeEnd!= "")
            {
                var query = _hmiAppDbContextArchivization.ArchivizationsForParameters.Where(x => x.DateTime.Contains(DateTimeStart))// && x.DateTime.Contains(DateTimeEnd))
                    .Select(x => new
                    {
                        x.ReferenceNumber,
                        x.NameOfClient,
                        x.PrzeciskanieP1,
                        x.MontazOslonkiP2,
                        x.OetickerP3,
                        x.DyszaWahliwaP4,
                        x.SmarTulipP5,
                        x.SmarPrzegubP6,
                        x.TraceUpP7,
                        x.TraceUpZapisP8,
                        x.RFIDGlowicaGornaP9,
                        x.RFIDPlytaSmarujacaP10,
                        x.RFIDSzczekiOslonkiP11,
                        x.RFIDGniazdoPrzegubuP12,
                        x.PozwyjeciaOsi,
                        x.PozOetickera,
                        x.Przeciskanie_PozStartowa,
                        x.Przeciskanie_DojazdWolny,
                        x.Przeciskanie_PoczCzytSily,
                        x.Przeciskanie_KoniecCzytSily,
                        x.Przeciskanie_SilaMin,
                        x.Przeciskanie_SilaMax,
                        x.Oslonka_PozStartowa,
                        x.Oslonka_PozSmarowania,
                        x.Oslonka_PozNakladania,
                        x.Oslonka_PozPowrotu,
                        x.DyszaWahliwa_PozPionowa,
                        x.DyszaWahliwa_PozPozioma,
                        x.DyszaWahliwa_PozdyszywOslonce,
                        x.DyszaWahliwa_PozZjazduOslonkiSmarowanie,
                        x.Smarowanie_DawkaPrzegub,
                        x.Smarowanie_TolDawkiPrzegub,
                        x.Smarowanie_RodzajSmaruPrzegub,
                        x.Smarowanie_DawkaTulip,
                        x.Smarowanie_TolDawkiTulip,
                        x.Smarowanie_RodzajSmaruTulip,
                        x.TagRFID_GlowicaGorna,
                        x.TagRFID_PlytaSmar,
                        x.TagRFID_SzczekiOslonki,
                        x.TagRFID_Przegub,
                    })
                        .ToList();

                foreach (var item in query)
                {
                    //Parametry
                    Form1._Form1.DB666NrReference.Text = item.ReferenceNumber;
                    Form1._Form1.DB666NameOfClient.Text = item.NameOfClient;
                    Form1._Form1.DB666PrzeciskanieP1.Checked = item.PrzeciskanieP1;
                    Form1._Form1.DB666MontazOslonkiP2.Checked = item.MontazOslonkiP2;
                    Form1._Form1.DB666OetickerP3.Checked = item.OetickerP3;
                    Form1._Form1.DB666DyszaWahliwaP4.Checked = item.DyszaWahliwaP4;
                    Form1._Form1.DB666SmarTulipP5.Checked = item.SmarTulipP5;
                    Form1._Form1.DB666SmarPrzegubP6.Checked = item.SmarPrzegubP6;
                    Form1._Form1.DB666TraceUpP7.Checked = item.TraceUpP7;
                    Form1._Form1.DB666TraceUpZapisP8.Checked = item.TraceUpZapisP8;
                    Form1._Form1.DB666RFIDGlowicaGornaP9.Checked = item.RFIDGlowicaGornaP9;
                    Form1._Form1.DB666RFIDPlytaSmarujacaP10.Checked = item.RFIDPlytaSmarujacaP10;
                    Form1._Form1.DB666RFIDSzczekiOslonkiP11.Checked = item.RFIDSzczekiOslonkiP11;
                    Form1._Form1.DB666RFIDGniazdoPrzegubuP12.Checked = item.RFIDGniazdoPrzegubuP12;
                    //Inne
                    Form1._Form1.DB666PozwyjeciaOsi.Text = item.PozwyjeciaOsi.ToString();
                    Form1._Form1.DB666PozOetickera.Text = item.PozOetickera.ToString();
                    //Przeciskanie
                    Form1._Form1.DB666PozStartowa__Przeciskanie.Text = item.Przeciskanie_PozStartowa.ToString();
                    Form1._Form1.DB666DojazdWolny_Przeciskanie.Text = item.Przeciskanie_DojazdWolny.ToString();
                    Form1._Form1.DB666PoczCzytSily__Przeciskanie.Text = item.Przeciskanie_PoczCzytSily.ToString();
                    Form1._Form1.DB666KoniecCzytSily__Przeciskanie.Text = item.Przeciskanie_KoniecCzytSily.ToString();
                    Form1._Form1.DB666SilaMin_Przeciskanie.Text = item.Przeciskanie_SilaMin.ToString();
                    Form1._Form1.DB666SilaMax__Przeciskanie.Text = item.Przeciskanie_SilaMax.ToString();
                    //Oslonka
                    Form1._Form1.DB666PozStartowa_Oslonka.Text = item.Oslonka_PozStartowa.ToString();
                    Form1._Form1.DB666PozSmarowania_Oslonka.Text = item.Oslonka_PozSmarowania.ToString();
                    Form1._Form1.DB666PozNakladania_Oslonka.Text = item.Oslonka_PozNakladania.ToString();
                    Form1._Form1.DB666PozPowrotu_Oslonka.Text = item.Oslonka_PozPowrotu.ToString();
                    //Dysza Wahliwa
                    Form1._Form1.DB666PozPionowa_DyszaWahliwa.Text = item.DyszaWahliwa_PozPionowa.ToString();
                    Form1._Form1.DB666PozPozioma_DyszaWahliwa.Text = item.DyszaWahliwa_PozPozioma.ToString();
                    Form1._Form1.DB666PozDyszyWOslonce_DyszaWahliwa.Text = item.DyszaWahliwa_PozdyszywOslonce.ToString();
                    Form1._Form1.DB666PozZjazduOslonkiSmarowanie_DyszaWahliwa.Text = item.DyszaWahliwa_PozZjazduOslonkiSmarowanie.ToString();
                    //Smarowania
                    Form1._Form1.DB666DawkaPrzegub.Text = item.Smarowanie_DawkaPrzegub.ToString();
                    Form1._Form1.DB666TolDawkiPrzegub.Text = item.Smarowanie_TolDawkiPrzegub.ToString();
                    Form1._Form1.DB666RodzajSmaruPrzegub.SelectedIndex = item.Smarowanie_RodzajSmaruPrzegub;
                    Form1._Form1.DB666DawkaTulip.Text = item.Smarowanie_DawkaTulip.ToString();
                    Form1._Form1.DB666TolDawkiTulip.Text = item.Smarowanie_TolDawkiTulip.ToString();
                    Form1._Form1.DB666RodzajSmaruTulip.SelectedIndex = item.Smarowanie_RodzajSmaruTulip;
                    //RFID
                    Form1._Form1.DB666TagRFIDGlowicaGorna.Text = item.TagRFID_GlowicaGorna.ToString();
                    Form1._Form1.DB666TagRFIDPlytaSmar.Text = item.TagRFID_PlytaSmar.ToString();
                    Form1._Form1.DB666TagRFIDSzczekiOslonki.Text = item.TagRFID_SzczekiOslonki.ToString();
                    Form1._Form1.DB666TagRFIDPrzegub.Text = item.TagRFID_Przegub.ToString();

                }
            }
        }

        //Update danych "?" zezwala na null - operator warunkowego dostepu
        //Metoda do zwrocenia wyszukiwanego przez nas pola w bazie
#nullable enable
        public ArchivizationModelExtendedDataBase? ReadFirst()
        {
            return _hmiAppDbContextArchivization.ArchivizationsForParameters.FirstOrDefault(x=>x.Id == 1);
#nullable disable
        }

        public void Delete()
        {
            ////usuwanie danych 
            _hmiAppDbContextArchivization.Database.ExecuteSqlRaw("TRUNCATE TABLE [ArchivizationsForParameters]");
        }
    }
}
