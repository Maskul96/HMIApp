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

            //ZROBIC DODATKOWO MODUŁ, KTORY ZAPISUJE DO BAZY DANYCH TE PARAMETRY I NP ZA POMOCA PRZYCISKU WYKORZYSTUJE CSVHELPER DO EXPORTU DANYCH DO CSV

            var NumberOfShifts = NumberOfProductionShift();
            var LocationOfArchivizationFolder = "D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\Archivizations\\";

            if (File.Exists(LocationOfArchivizationFolder + $"Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var stream = File.Open(LocationOfArchivizationFolder+$"HMIApp\\Resources\\Files\\Archivizations\\Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv", FileMode.Append);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, configEventsWhenFileExist);
                csv.Context.RegisterClassMap<ArchivizationModelMap>();
                csv.WriteRecords(_archivizationmodels);
            }
            else if (!File.Exists(LocationOfArchivizationFolder+$"HMIApp\\Resources\\Files\\Archivizations\\Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv"))
            {
                using var writer = new StreamWriter(LocationOfArchivizationFolder+$"HMIApp\\Resources\\Files\\Archivizations\\Archivization_NrZmiany{NumberOfShifts}_Data{DateTime.Now.ToString("d")}.csv");
                using var csv = new CsvWriter(writer, configEventsWhenFileNOTExist);
                csv.Context.RegisterClassMap<ArchivizationModelMap>();
                csv.WriteRecords(_archivizationmodels);
            }
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
