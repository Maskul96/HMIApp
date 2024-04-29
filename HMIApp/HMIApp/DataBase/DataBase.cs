using HMIApp.Components;
using HMIApp.Components.DataBase;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace HMIApp.Data
{
    public class DataBase : iDataBase
    {
        public string ConnectionString = "";
        private readonly HMIAppDBContext _hmiAppDbContext;
        public Form1 obj;

        //konstruktor do wstrzykiwania DBContext
        public DataBase(HMIAppDBContext hmiAppDbContext)
        {
            _hmiAppDbContext = hmiAppDbContext;
            //w momencie wywolania konstruktora sprawdzamy czy nasza baza danych jest stworzona - jesli nie jest stworzona to ponizsza metoda ja utworzy
            _hmiAppDbContext.Database.EnsureCreated();
        }
        //konstruktor bezparametrowy
        public DataBase()
        {

        }
        //Ponizej konstruktor z parametrem obj do przekazywania obiektow z Form1 do wnętrza klasy
        public DataBase(Form1 obj)
        {
            this.obj = obj;
        }

        Logger _logger = new Logger();

        //Odczyt pliku konfiguracyjnego z connection stringiem
        public string ReadConfFile(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                ConnectionString = System.IO.File.ReadAllText(filepath);
            }
            else
            {
                MessageBox.Show("Nie mozna otworzyć pliku konfiguracyjnego bazy danych");
                _logger.LogMessage("Nie mozna otworzyc pliku konfiguracyjnego bazy danych");
            }
            return ConnectionString;
        }

        public void Run()
        {
            ReadConfFile("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\DataBaseConfiguration.txt");

        }

        //Metoda inserta do bazy
        public void InsertToDataBase()
        {
            var query = _hmiAppDbContext.References.Where(x => x.ReferenceNumber == Form1._Form1.DB666NrReference.Text).SingleOrDefault();
            if (query == null)
            {
                _hmiAppDbContext.References.Add(new Reference()
                {
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
                    SpareP13 = false,
                    SpareP14 = false,
                    SpareP15 = false,
                    SpareP16 = false,
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

                _hmiAppDbContext.SaveChanges();
                ClearCombobox();
                SelectFromDbToComboBox();
            }
            else
            {//komunikat dla usera
                MessageBox.Show("Istnieje juz taka referencja");
            }
        }

        //metoda select z uzyciem LINQ
        public void SelectFromDataBase(string referencenumber)
        {
            if(referencenumber == "")
            {//komunikat dla usera
                MessageBox.Show("Nie podano referencji");
            }
            if (referencenumber != "")
            {
                var query = _hmiAppDbContext.References.Where(x => x.ReferenceNumber == referencenumber)
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
                        x.SpareP13,
                        x.SpareP14,
                        x.SpareP15,
                        x.SpareP16,
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

        //Metoda do wyswietlenia referencji w combobox5
        public void SelectFromDbToComboBox()
        {
            var References = _hmiAppDbContext.References.ToList();
            foreach(var Reference in References)
            {
                if (!Form1._Form1.comboBoxListaReferencji.Items.Contains(Reference.ReferenceNumber))
                {
                    Form1._Form1.comboBoxListaReferencji.Items.Add(Reference.ReferenceNumber);
                }
            }
        }

        public void ClearCombobox()
        {
            Form1._Form1.comboBoxListaReferencji.Items.Clear();
            Form1._Form1.comboBoxListaReferencji.Text = "";
        }

        //Update danych "?" zezwala na null - operator warunkowego dostepu
        //Metoda do zwrocenia wyszukiwanego przez nas pola w bazie
#nullable enable
        public Reference? ReadFirst(string referencenumber)
        {
            return _hmiAppDbContext.References.FirstOrDefault(x => x.ReferenceNumber == referencenumber);
#nullable disable
        }

        public void UpdateDb(string referencenumber)
        {
            bool blockade = false;
            //    ////Update danych cd
            var ref1 = ReadFirst(referencenumber);
            if(referencenumber == Form1._Form1.DB666NrReference.Text)
            {
                blockade = true;
            }
            //Parametry
            ref1.ReferenceNumber = Form1._Form1.DB666NrReference.Text;
            ref1.NameOfClient = Form1._Form1.DB666NameOfClient.Text;
            ref1.PrzeciskanieP1 = Form1._Form1.DB666PrzeciskanieP1.Checked;
            ref1.MontazOslonkiP2 = Form1._Form1.DB666MontazOslonkiP2.Checked;
            ref1.OetickerP3 = Form1._Form1.DB666OetickerP3.Checked;
            ref1.DyszaWahliwaP4 = Form1._Form1.DB666DyszaWahliwaP4.Checked;
            ref1.SmarTulipP5 = Form1._Form1.DB666SmarTulipP5.Checked;
            ref1.SmarPrzegubP6 = Form1._Form1.DB666SmarPrzegubP6.Checked;
            ref1.TraceUpP7 = Form1._Form1.DB666TraceUpP7.Checked;
            ref1.TraceUpZapisP8 = Form1._Form1.DB666TraceUpZapisP8.Checked;
            ref1.RFIDGlowicaGornaP9 = Form1._Form1.DB666RFIDGlowicaGornaP9.Checked;
            ref1.RFIDPlytaSmarujacaP10 = Form1._Form1.DB666RFIDPlytaSmarujacaP10.Checked;
            ref1.RFIDSzczekiOslonkiP11 = Form1._Form1.DB666RFIDSzczekiOslonkiP11.Checked;
            ref1.RFIDGniazdoPrzegubuP12 = Form1._Form1.DB666RFIDGniazdoPrzegubuP12.Checked;
            //Inne
            ref1.PozwyjeciaOsi = float.Parse(Form1._Form1.DB666PozwyjeciaOsi.Text);
            ref1.PozOetickera = float.Parse(Form1._Form1.DB666PozOetickera.Text);
            //Przeciskanie
            ref1.Przeciskanie_PozStartowa = float.Parse(Form1._Form1.DB666PozStartowa__Przeciskanie.Text);
            ref1.Przeciskanie_DojazdWolny = float.Parse(Form1._Form1.DB666DojazdWolny_Przeciskanie.Text);
            ref1.Przeciskanie_PoczCzytSily = float.Parse(Form1._Form1.DB666PoczCzytSily__Przeciskanie.Text);
            ref1.Przeciskanie_KoniecCzytSily = float.Parse(Form1._Form1.DB666KoniecCzytSily__Przeciskanie.Text);
            ref1.Przeciskanie_SilaMin = int.Parse(Form1._Form1.DB666SilaMin_Przeciskanie.Text);
            ref1.Przeciskanie_SilaMax = int.Parse(Form1._Form1.DB666SilaMax__Przeciskanie.Text);
            //Oslonka
            ref1.Oslonka_PozStartowa = float.Parse(Form1._Form1.DB666PozStartowa_Oslonka.Text);
            ref1.Oslonka_PozSmarowania = float.Parse(Form1._Form1.DB666PozSmarowania_Oslonka.Text);
            ref1.Oslonka_PozNakladania = float.Parse(Form1._Form1.DB666PozNakladania_Oslonka.Text);
            ref1.Oslonka_PozPowrotu = float.Parse(Form1._Form1.DB666PozPowrotu_Oslonka.Text);
            //Dysza Wahliwa
            ref1.DyszaWahliwa_PozPionowa = float.Parse(Form1._Form1.DB666PozPionowa_DyszaWahliwa.Text);
            ref1.DyszaWahliwa_PozPozioma = float.Parse(Form1._Form1.DB666PozPozioma_DyszaWahliwa.Text);
            ref1.DyszaWahliwa_PozdyszywOslonce = float.Parse(Form1._Form1.DB666PozDyszyWOslonce_DyszaWahliwa.Text);
            ref1.DyszaWahliwa_PozZjazduOslonkiSmarowanie = float.Parse(Form1._Form1.DB666PozZjazduOslonkiSmarowanie_DyszaWahliwa.Text);
            //Smarowania
            ref1.Smarowanie_DawkaPrzegub = float.Parse(Form1._Form1.DB666DawkaPrzegub.Text);
            ref1.Smarowanie_TolDawkiPrzegub = float.Parse(Form1._Form1.DB666TolDawkiPrzegub.Text);
            ref1.Smarowanie_RodzajSmaruPrzegub =Form1._Form1.DB666RodzajSmaruPrzegub.SelectedIndex;
            ref1.Smarowanie_DawkaTulip = float.Parse(Form1._Form1.DB666DawkaTulip.Text);
            ref1.Smarowanie_TolDawkiTulip = float.Parse(Form1._Form1.DB666TolDawkiTulip.Text);
            ref1.Smarowanie_RodzajSmaruTulip = Form1._Form1.DB666RodzajSmaruTulip.SelectedIndex;
            //RFID
            ref1.TagRFID_GlowicaGorna = int.Parse(Form1._Form1.DB666TagRFIDGlowicaGorna.Text);
            ref1.TagRFID_PlytaSmar = int.Parse(Form1._Form1.DB666TagRFIDPlytaSmar.Text);
            ref1.TagRFID_SzczekiOslonki = int.Parse(Form1._Form1.DB666TagRFIDSzczekiOslonki.Text);
            ref1.TagRFID_Przegub = int.Parse(Form1._Form1.DB666TagRFIDPrzegub.Text);
            _hmiAppDbContext.SaveChanges();
            //Dorobić taką opcję, że czyscimy comboboxa tylko wtedy kiedy zmienimy samą nazwe
            if(blockade == false)
            {
                ClearCombobox();
            }
            SelectFromDbToComboBox();
            blockade = false;
        }

        public void Delete(string referencenumber)
        {

            //    ////usuwanie danych
            if (referencenumber != "")
            {
                var ref2 = ReadFirst(referencenumber);
                _hmiAppDbContext.References.Remove(ref2);
                _hmiAppDbContext.SaveChanges();
            }
            ClearCombobox();
            SelectFromDbToComboBox();
        }
    }
}
