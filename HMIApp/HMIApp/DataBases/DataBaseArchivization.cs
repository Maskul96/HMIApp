using HMIApp.Components;
using HMIApp.Components.DataBase;
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
            ReadConfFile("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\DataBaseConfiguration.txt");

        }

        //Metoda inserta do bazy parametrow i eventów - ta metode mozna wywolac w momencie odpalenia eventu
        public void InsertToDataBase(string message)
        {
            _hmiAppDbContextArchivization.ArchivizationsForParameters.Add(new ArchivizationModelExtendedDataBase()
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

        //Update danych "?" zezwala na null - operator warunkowego dostepu
        //Metoda do zwrocenia wyszukiwanego przez nas pola w bazie
#nullable enable
        public ArchivizationModelExtendedDataBase? ReadFirst(string DateTime)
        {
            return _hmiAppDbContextArchivization.ArchivizationsForParameters.FirstOrDefault(x => x.DateTime == DateTime);
#nullable disable
        }

        public void Delete(string DateTime)
        {
            ////usuwanie danych
            if (DateTime != "")
            {
                var ref2 = ReadFirst(DateTime);
                _hmiAppDbContextArchivization.ArchivizationsForParameters.Remove(ref2);
                _hmiAppDbContextArchivization.SaveChanges();
            }
        }
    }
}
