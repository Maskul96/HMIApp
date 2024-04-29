using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using HMIApp.Archivizations.Models;
using HMIApp.Components.UserAdministration;
using HMIApp.Data;
using SkiaSharp;

namespace HMIApp.Archivizations
{

    public class Archivization : iArchivization
    {
        public DataBaseArchivization _databaseArchive;
        public Form1 obj;
        public List<ArchivizationModelBasic> _archivizationmodelsbasic = new List<ArchivizationModelBasic>();
        public List<ArchivizationModelExtended> _archivizationmodelextended = new List<ArchivizationModelExtended>();
        //public int Id = 0;
        public Archivization(Form1 obj)
        {
            this.obj = obj;
        }
        public Archivization()
        {

        }

        //Delegaty  - jest to referencja na metode
        public delegate void ArchiveEvents(object sender, EventArgs args, string message);
        public event ArchiveEvents ArchiveEvent;

        public void Run()
        {
            //Odpalamy metode _Archive_ArchiveEvent jak event sie zadzieje
            ArchiveEvent += _Archive_ArchiveEvent;
        }

        //Metoda uruchamiająca się jak odpalimy event
        public void _Archive_ArchiveEvent(object sender, EventArgs args, string message)
        {
            //Ponizszy kod testowo do wyswietlenia na HMI 
            System.Windows.MessageBox.Show("Odpaliłeś event");
            Form1._Form1.label51.Text = Form1._Form1.label_DataIGodzina.Text;
            Form1._Form1.label139.Text = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text;
            Form1._Form1.label140.Text = message;

            //this.Id = this.Id + 1;
            var archivizationmodelsbasic = new ArchivizationModelBasic
            {
                // Id = this.Id,
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                NrOfCard = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text,
                Message = message
                //dorobic logowanie parametrow - najprosciej to mozna by bylo w trakcie wywolania eventu zrobic po prosu selecta z bazy z aktualna referencja i ja dopisac?
            };
            _archivizationmodelsbasic.Add(archivizationmodelsbasic);
            ArchivizationCsvFileHandlingForBasicModel();

            //Uruchomienie tego typu archiwizacji czyli razem z parametrami dopiero jak bedzie wczytana referencja
            //if (Form1._Form1.DB666NrReferencePassedValue.Text != "")
            //{
                var archivizationmodelsextended = new ArchivizationModelExtended
                {
                    // Id = this.Id,
                    DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    NrOfCard = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text,
                    Message = message,

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

                    //dorobic logowanie parametrow - najprosciej to mozna by bylo w trakcie wywolania eventu zrobic po prosu selecta z bazy z aktualna referencja i ja dopisac?
                };
                _archivizationmodelextended.Add(archivizationmodelsextended);
                ArchivizationCsvFileHandlingForExtendedModel();
            //}

            //Wywolanie metody inserta referencji i danych z eventu do bazy
            //_databaseArchive.InsertToDataBase(message);

        }

        public void ArchivizationCsvFileHandlingForBasicModel()
        {

            var configEventsWhenFileExist = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8,

            };
            var configEventsWhenFileNOTExist = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8
            };

            //ZROBIC DODATKOWO MODUŁ, KTORY ZAPISUJE DO BAZY DANYCH TE PARAMETRY I NP ZA POMOCA PRZYCISKU WYKORZYSTUJE CSVHELPER DO EXPORTU DANYCH DO CSV

            var NumberOfShifts = NumberOfProductionShift();
            var LocationOfArchivizationFolder = "D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\Archivizations\\";

            if (File.Exists(LocationOfArchivizationFolder + $"Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var stream = File.Open(LocationOfArchivizationFolder+$"Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, configEventsWhenFileExist);
                csv.Context.RegisterClassMap<ArchivizationModelBasicMap>();
                csv.WriteRecords(_archivizationmodelsbasic);
            }
            else if (!File.Exists(LocationOfArchivizationFolder+$"Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var writer = new StreamWriter(LocationOfArchivizationFolder+$"Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv");
                using var csv = new CsvWriter(writer, configEventsWhenFileNOTExist);
                csv.Context.RegisterClassMap<ArchivizationModelBasicMap>();
                csv.WriteRecords(_archivizationmodelsbasic);
            }

            ClearListArchivizationModelBasic();
        }

        public void ArchivizationCsvFileHandlingForExtendedModel()
        {
            var configEventsWhenFileExist = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8,

            };
            var configEventsWhenFileNOTExist = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8
            };

            //ZROBIC DODATKOWO MODUŁ, KTORY ZAPISUJE DO BAZY DANYCH TE PARAMETRY I NP ZA POMOCA PRZYCISKU WYKORZYSTUJE CSVHELPER DO EXPORTU DANYCH DO CSV

            var NumberOfShifts = NumberOfProductionShift();
            var LocationOfArchivizationFolder = "D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\ArchivizationsExtended\\";

            if (File.Exists(LocationOfArchivizationFolder + $"ArchivizationExtended_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var stream = File.Open(LocationOfArchivizationFolder + $"ArchivizationExtended_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, configEventsWhenFileExist);
                csv.Context.RegisterClassMap<ArchivizationModelBasicMap>();
                csv.WriteRecords(_archivizationmodelextended);
            }
            else if (!File.Exists(LocationOfArchivizationFolder + $"ArchivizationExtended_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var writer = new StreamWriter(LocationOfArchivizationFolder + $"ArchivizationExtended_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv");
                using var csv = new CsvWriter(writer, configEventsWhenFileNOTExist);
                csv.Context.RegisterClassMap<ArchivizationModelBasicMap>();
                csv.WriteRecords(_archivizationmodelextended);
            }

            ClearListArchivizationModelExtended();
        }

        public int NumberOfProductionShift()
        {
            int NumberOfShift=0;
            int HourOfDay = DateTime.Now.Hour;

            if(HourOfDay>=6)
            {
                NumberOfShift = 1;
            }
            else if(HourOfDay >= 14)
            {
                NumberOfShift = 2;
            }
            else if(HourOfDay >= 24)
            {
                NumberOfShift = 3;
            }
            return NumberOfShift;
        }
       
        public void ClearListArchivizationModelBasic()
        {
            _archivizationmodelsbasic.Clear();
        }

        public void ClearListArchivizationModelExtended()
        {
            _archivizationmodelextended.Clear();
        }

        public void OnArchiveEventsMethod(string message)
        {
            //Sprawdzamy w ifie czy ktos z zewnatrz (subscriber) podpiął się pod ten event i jak tak to dopiero odpalamy event
            if (ArchiveEvent != null)
            {
                //odpalenie eventu
                ArchiveEvent(this, new EventArgs(), message);
            }
        }

    }

    //Archiwizacja do bazy danych i moduł pozwalajacy wyciagnac z bazy danych eventy z danego dnia i wyeksportowac do csv
}
