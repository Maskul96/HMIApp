using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using HMIApp.Archivizations.Models;
using HMIApp.Components.UserAdministration;
using SkiaSharp;

namespace HMIApp.Archivizations
{

    public class Archivization : iArchivization
    {
        public Form1 obj;
        public List<ArchivizationModel> _archivizationmodels = new List<ArchivizationModel>();
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
            MessageBox.Show("Odpaliłeś event");
            Form1._Form1.label51.Text = Form1._Form1.label_DataIGodzina.Text;
            Form1._Form1.label139.Text = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text;
            Form1._Form1.label140.Text = message;
            //this.Id = this.Id + 1;
            var archivizationmodels = new ArchivizationModel
            {
               // Id = this.Id,
                DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                NrOfCard = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text,
                Message = message
                //dorobic logowanie parametrow - najprosciej to mozna by bylo w trakcie wywolania eventu zrobic po prosu selecta z bazy z aktualna referencja i ja dopisac?
            };

            _archivizationmodels.Add(archivizationmodels);

            ArchivizationCsvFileHandling();

            ClearList();
        }

        public void ArchivizationCsvFileHandling()
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
            //ZROBIC TAK ZE CSV GENERUJE SIE CO KAZDA ZMIANE CZYLI CO 8H I JEGO NAZWA JEST INDEKSOWANA DATĄ I KTORA TO ZMIANA
            //ZROBIC DODATKOWO MODUŁ, KTORY ZAPISUJE DO BAZY DANYCH TE PARAMETRY I NP ZA POMOCA PRZYCISKU WYKORZYSTUJE CSVHELPER DO EXPORTU DANYCH DO CSV
            if (File.Exists("Archivization.csv"))
            {
                using var stream = File.Open("Archivization.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, configEventsWhenFileExist);
                csv.Context.RegisterClassMap<ArchivizationModelMap>();
                csv.WriteRecords(_archivizationmodels);
            }
            else if (!File.Exists("Archivization.csv"))
            {
                using var writer = new StreamWriter("Archivization.csv");
                using var csv = new CsvWriter(writer, configEventsWhenFileNOTExist);
                csv.Context.RegisterClassMap<ArchivizationModelMap>();
                csv.WriteRecords(_archivizationmodels);
            }
        }
       
        public void ClearList()
        {
            _archivizationmodels.Clear();
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
