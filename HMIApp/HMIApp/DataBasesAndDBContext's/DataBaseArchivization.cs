using HMIApp.Archivizations;
using HMIApp.Archivizations.Models;
using HMIApp.Components;
using HMIApp.Components.DataBase;
using HMIApp.DataBasesAndDBContext_s;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Forms;

namespace HMIApp.Data
{
    public class DataBaseArchivization :BaseClassForDatabase, IDataBaseArchivization
    {
        private readonly HMIAppDBContextArchivization _hmiAppDbContextArchivization;
        public List<ArchivizationModelExtendedDataBase> _archivizationmodelextendeddatabase = new();
        public DataBaseArchivization() : base()
        {

        }
        public DataBaseArchivization(HMIAppDBContextArchivization hmiAppDbContextArchivization)
        {
            _hmiAppDbContextArchivization = hmiAppDbContextArchivization;
            //w momencie wywolania konstruktora sprawdzamy czy nasza baza danych jest stworzona - jesli nie jest stworzona to ponizsza metoda ja utworzy
            _hmiAppDbContextArchivization.Database.EnsureCreated();
        }

        public void Run()
        {
            ReadConfFile(Path.Combine(Form1.basePathToHMIAppFolder, "DataBaseArchivizationConfiguration.txt"));
        }

        public void CountRowsAndDeleteAllData()
        {
            var AllRowsInDataBaseArchive = _hmiAppDbContextArchivization.ArchivizationsForParameters.Count();
            //Kasowanie bazy danych jak jest wiecej niz milion wierszy
            if(AllRowsInDataBaseArchive >= 1000000)
            {
                Delete();
            }
        }

        #region OBSŁUGA BAZY DANYCH DLA EVENTÓW ARCHIWIZOWANYCH
        //Metoda inserta do bazy parametrow i eventów - ta metode mozna wywolac w momencie odpalenia eventu
        public void InsertToDataBase(string message="",int nrofshift=0)
        {
                _hmiAppDbContextArchivization?.ArchivizationsForParameters.Add(new ArchivizationModelExtendedDataBase()
                {
                    DateAndTime = DateTime.Now,
                    NrOfShift = nrofshift,
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
        public List<ArchivizationModelExtendedDataBase> SelectFromDataBase(string DateTimeStart,string DateTimeEnd, int HourStart = 0, int HourEnd=0)
        {
            _archivizationmodelextendeddatabase.Clear();
            if (DateTimeStart != "" && DateTimeEnd != "")
            {
                    //Ponizszy if daje przedział całodniowy w przypadku jak uzytkownik nie bedzie chcial eksportowac pliku dodatkowo po godzinie
                    if (HourStart == 0 && HourEnd == 0) HourEnd = 24;
                        var query = _hmiAppDbContextArchivization.ArchivizationsForParameters.Where(x => x.DateAndTime.Date >= Convert.ToDateTime(DateTimeStart) && x.DateAndTime.Date <= Convert.ToDateTime(DateTimeEnd))
                        .Where(x => x.DateAndTime.Hour >= HourStart && x.DateAndTime.Hour <= HourEnd)
                        .Select(x => new
                        {
                            x.DateAndTime,
                            x.NrOfShift,
                            x.NrOfCard,
                            x.Message,
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

                    if (query.Count != 0)
                    {
                        foreach (var item in query)
                        {
                            //Parametry
                            var _ArchivizationModelExtendedDataBase = new ArchivizationModelExtendedDataBase
                            {
                                DateAndTime = item.DateAndTime,
                                NrOfShift = item.NrOfShift,
                                NrOfCard = item.NrOfCard,
                                Message = item.Message,
                                ReferenceNumber = item.ReferenceNumber,
                                NameOfClient = item.NameOfClient,
                                PrzeciskanieP1 = item.PrzeciskanieP1,
                                MontazOslonkiP2 = item.MontazOslonkiP2,
                                OetickerP3 = item.OetickerP3,
                                DyszaWahliwaP4 = item.DyszaWahliwaP4,
                                SmarTulipP5 = item.SmarTulipP5,
                                SmarPrzegubP6 = item.SmarPrzegubP6,
                                TraceUpP7 = item.TraceUpP7,
                                TraceUpZapisP8 = item.TraceUpZapisP8,
                                RFIDGlowicaGornaP9 = item.RFIDGlowicaGornaP9,
                                RFIDPlytaSmarujacaP10 = item.RFIDPlytaSmarujacaP10,
                                RFIDSzczekiOslonkiP11 = item.RFIDSzczekiOslonkiP11,
                                RFIDGniazdoPrzegubuP12 = item.RFIDGniazdoPrzegubuP12,
                                PozwyjeciaOsi = item.PozwyjeciaOsi,
                                PozOetickera = item.PozOetickera,
                                Przeciskanie_PozStartowa = item.Przeciskanie_PozStartowa,
                                Przeciskanie_DojazdWolny = item.Przeciskanie_DojazdWolny,
                                Przeciskanie_PoczCzytSily = item.Przeciskanie_PoczCzytSily,
                                Przeciskanie_KoniecCzytSily = item.Przeciskanie_KoniecCzytSily,
                                Przeciskanie_SilaMin = item.Przeciskanie_SilaMin,
                                Przeciskanie_SilaMax = item.Przeciskanie_SilaMax,
                                Oslonka_PozStartowa = item.Oslonka_PozStartowa,
                                Oslonka_PozSmarowania = item.Oslonka_PozSmarowania,
                                Oslonka_PozNakladania = item.Oslonka_PozNakladania,
                                Oslonka_PozPowrotu = item.Oslonka_PozPowrotu,
                                DyszaWahliwa_PozPionowa = item.DyszaWahliwa_PozPionowa,
                                DyszaWahliwa_PozPozioma = item.DyszaWahliwa_PozPozioma,
                                DyszaWahliwa_PozdyszywOslonce = item.DyszaWahliwa_PozdyszywOslonce,
                                DyszaWahliwa_PozZjazduOslonkiSmarowanie = item.DyszaWahliwa_PozZjazduOslonkiSmarowanie,
                                Smarowanie_DawkaPrzegub = item.Smarowanie_DawkaPrzegub,
                                Smarowanie_TolDawkiPrzegub = item.Smarowanie_TolDawkiPrzegub,
                                Smarowanie_RodzajSmaruPrzegub = item.Smarowanie_RodzajSmaruPrzegub,
                                Smarowanie_DawkaTulip = item.Smarowanie_DawkaTulip,
                                Smarowanie_TolDawkiTulip = item.Smarowanie_TolDawkiTulip,
                                Smarowanie_RodzajSmaruTulip = item.Smarowanie_RodzajSmaruTulip,
                                TagRFID_GlowicaGorna = item.TagRFID_GlowicaGorna,
                                TagRFID_PlytaSmar = item.TagRFID_PlytaSmar,
                                TagRFID_SzczekiOslonki = item.TagRFID_SzczekiOslonki,
                                TagRFID_Przegub = item.TagRFID_Przegub
                            };

                            _archivizationmodelextendeddatabase.Add(_ArchivizationModelExtendedDataBase);
                        }
                    }
                   else  System.Windows.MessageBox.Show("Nie istnieją w bazie danych rekordy z podanego przedziału czasu");
              
            }
            return _archivizationmodelextendeddatabase;
        }
        #endregion

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
            //usuwanie danych 
            _hmiAppDbContextArchivization.Database.ExecuteSqlRaw("TRUNCATE TABLE [ArchivizationsForParameters]");
        }
    }
}
