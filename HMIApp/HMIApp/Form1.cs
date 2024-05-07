using HMIApp.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using HMIApp.Components.UserAdministration;
using HMIApp.Archivizations;
using HMIApp.Data;
using Microsoft.EntityFrameworkCore;
using HMIApp.Components.RFIDCardReader;
using Microsoft.Win32;
using System.Diagnostics;
using ScottPlot.Colormaps;




namespace HMIApp
{
    public partial class Form1 : Form
    {
        Archivization _Archive = new Archivization();
        App App = new App();
        UserAdministration Users = new UserAdministration();
        DataBase DataBase = new DataBase();
        Logger _logger = new Logger();
        //Services do dependency injection
        public ServiceCollection services = new ServiceCollection();
        public ServiceProvider serviceProvider;
        public SerialPortReader serialPortReader = new SerialPortReader();
        public bool blockade;
        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
            //Services do dependency injection
            services.AddSingleton<iDataBase, DataBase>();
            //ZArejestrowanie DBContextu - Stworzenie połączenia do bazy danych i service providera
            services.AddDbContext<HMIAppDBContext>();

            serviceProvider = services.BuildServiceProvider();

            ReadFromDbWhenAppIsStarting();

            Users.Run();

            OdczytDB.Enabled = true;

            listBoxWarningsView.DrawItem += new DrawItemEventHandler(App.listBox1_DrawItem);
            label13.Text = "";
            Users.EnabledObjects();

            CommaReplaceDotTextBox(this);
            PassedValueControls.Run();

            _Archive.Run();
            _Archive.ArchiveEventRun();
            //serialPortReader.InitializeSerialPort();
            // serialPortReader.Run();

        }

        //Event zmiany koloru - wyzwala metode uruchmiajaca Event przejscia w Auto/Man
        public void BackColor_ColorChanged(object sender, EventArgs e)
        {
            if (DB667Auto.BackColor.Name == "LimeGreen")
            {
                _Archive.OnArchiveEventsMethod("Event - Przejście w Auto");
            }
            else if (DB667Man.BackColor.Name == "LimeGreen")
            {
                _Archive.OnArchiveEventsMethod("Event - Przejście w Man");
            }
        }

        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;

        private void Form1_Load(object sender, EventArgs e)
        {
            //zakomentuj ponizsze cztery metody do odpalenia apki bez PLC
            App.RunInitPLC();
            PróbyUruchomieniaKomunikacjizPLC.Enabled = true;

            App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            App.ReadActualValueFromDBChart_Simplified("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_4.csv");
            App.ReadActualValueFromDBReferenceOrProcessData("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");

            //Blokada rysowania wykresu dopoki nie zaczytasz referencji
            blockade = false;

            CzyszczenieStatusówLogowania.Enabled = true;
          //  CzyszczenieStatusowArchiwizacji.Enabled = true; 
        }


        //Timer do prób uruchomienia komunikacji z PLC
        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            //zakomentuj ponizsze cztery metody do odpalenia apki bez PLC
            if (App.RunInitPLC() == false)
            {
                App.RunInitPLC();
            }
        }

        //METODA DO ODCZYTU DANYCH Z BAZY przy starcie aplikacji
        public void ReadFromDbWhenAppIsStarting()
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.SelectFromDbToComboBox();
            //Najpierw odczyt z combobox zeby potem moc odczytac z bazy danych pierwszy element na starcie
            if (comboBoxListaReferencji.Items.Count > 0)
            {
                database.SelectFromDataBase(comboBoxListaReferencji.Items[0].ToString());
            }
            else
            {
                // MessageBox.Show("Nie znaleziono referencji");
            }
        }

        //wczytaj referencje do PLC - DOROBIC ZAPIS POZOSTALYCH PARAMETROW REFERENCJI
        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
            {
                //Zapisz referencje do bazy danych
                var database = serviceProvider.GetService<iDataBase>();
                database.UpdateDb(comboBoxListaReferencji.SelectedItem.ToString());

                //Wczytaj parametry referencji do PLC
                //Nr referencji i klient
                App.WriteToDB(DB666NrReference.Text, DB666NrReference.Tag.ToString());
                App.WriteToDB(DB666NameOfClient.Text, DB666NameOfClient.Tag.ToString());
                //CheckBoxy
                App.WriteToDB(DB666PrzeciskanieP1.Checked.ToString(), DB666PrzeciskanieP1.Tag.ToString());
                App.WriteToDB(DB666MontazOslonkiP2.Checked.ToString(), DB666MontazOslonkiP2.Tag.ToString());
                App.WriteToDB(DB666OetickerP3.Checked.ToString(), DB666OetickerP3.Tag.ToString());
                App.WriteToDB(DB666DyszaWahliwaP4.Checked.ToString(), DB666DyszaWahliwaP4.Tag.ToString());
                App.WriteToDB(DB666SmarTulipP5.Checked.ToString(), DB666SmarTulipP5.Tag.ToString());
                App.WriteToDB(DB666SmarPrzegubP6.Checked.ToString(), DB666SmarPrzegubP6.Tag.ToString());
                App.WriteToDB(DB666TraceUpP7.Checked.ToString(), DB666TraceUpP7.Tag.ToString());
                App.WriteToDB(DB666TraceUpZapisP8.Checked.ToString(), DB666TraceUpZapisP8.Tag.ToString());
                App.WriteToDB(DB666RFIDGlowicaGornaP9.Checked.ToString(), DB666RFIDGlowicaGornaP9.Tag.ToString());
                App.WriteToDB(DB666RFIDPlytaSmarujacaP10.Checked.ToString(), DB666RFIDPlytaSmarujacaP10.Tag.ToString());
                App.WriteToDB(DB666RFIDSzczekiOslonkiP11.Checked.ToString(), DB666RFIDSzczekiOslonkiP11.Tag.ToString());
                App.WriteToDB(DB666RFIDGniazdoPrzegubuP12.Checked.ToString(), DB666RFIDGniazdoPrzegubuP12.Tag.ToString());
                //Inne
                App.WriteToDB(DB666PozwyjeciaOsi.Text, DB666PozwyjeciaOsi.Tag.ToString());
                App.WriteToDB(DB666PozOetickera.Text, DB666PozOetickera.Tag.ToString());
                //Przeciskanie
                App.WriteToDB(DB666PozStartowa__Przeciskanie.Text, DB666PozStartowa__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666DojazdWolny_Przeciskanie.Text, DB666DojazdWolny_Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666SilaMin_Przeciskanie.Text, DB666SilaMin_Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666SilaMax__Przeciskanie.Text, DB666SilaMax__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666PoczCzytSily__Przeciskanie.Text, DB666PoczCzytSily__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666KoniecCzytSily__Przeciskanie.Text, DB666KoniecCzytSily__Przeciskanie.Tag.ToString());
                //Oslonka
                App.WriteToDB(DB666PozStartowa_Oslonka.Text, DB666PozStartowa_Oslonka.Tag.ToString());
                App.WriteToDB(DB666PozSmarowania_Oslonka.Text, DB666PozSmarowania_Oslonka.Tag.ToString());
                App.WriteToDB(DB666PozNakladania_Oslonka.Text, DB666PozNakladania_Oslonka.Tag.ToString());
                App.WriteToDB(DB666PozPowrotu_Oslonka.Text, DB666PozPowrotu_Oslonka.Tag.ToString());
                //Dysza Wahliwa
                App.WriteToDB(DB666PozPionowa_DyszaWahliwa.Text, DB666PozPionowa_DyszaWahliwa.Tag.ToString());
                App.WriteToDB(DB666PozPozioma_DyszaWahliwa.Text, DB666PozPozioma_DyszaWahliwa.Tag.ToString());
                App.WriteToDB(DB666PozDyszyWOslonce_DyszaWahliwa.Text, DB666PozDyszyWOslonce_DyszaWahliwa.Tag.ToString());
                App.WriteToDB(DB666PozZjazduOslonkiSmarowanie_DyszaWahliwa.Text, DB666PozZjazduOslonkiSmarowanie_DyszaWahliwa.Tag.ToString());
                //Smarowanie
                App.WriteToDB(DB666DawkaPrzegub.Text, DB666DawkaPrzegub.Tag.ToString());
                App.WriteToDB(DB666TolDawkiPrzegub.Text, DB666TolDawkiPrzegub.Tag.ToString());
                App.WriteToDB(DB666RodzajSmaruPrzegub.SelectedIndex.ToString(), DB666RodzajSmaruPrzegub.Tag.ToString());

                App.WriteToDB(DB666DawkaTulip.Text, DB666DawkaTulip.Tag.ToString());
                App.WriteToDB(DB666TolDawkiTulip.Text, DB666TolDawkiTulip.Tag.ToString());
                App.WriteToDB(DB666RodzajSmaruTulip.SelectedIndex.ToString(), DB666RodzajSmaruTulip.Tag.ToString());
                //RFID
                App.WriteToDB(DB666TagRFIDGlowicaGorna.Text, DB666TagRFIDGlowicaGorna.Tag.ToString());
                App.WriteToDB(DB666TagRFIDPlytaSmar.Text, DB666TagRFIDPlytaSmar.Tag.ToString());
                App.WriteToDB(DB666TagRFIDSzczekiOslonki.Text, DB666TagRFIDSzczekiOslonki.Tag.ToString());
                App.WriteToDB(DB666TagRFIDPrzegub.Text, DB666TagRFIDPrzegub.Tag.ToString());

                //przepisywanie numeru referencji i klienta do wyswietlenia dla operatora
                PassedValueControls.StringOfReference();

                blockade = true;
                //Przepisanie wartosci do wygenerowania okna czytania sily w wykresie
                App.WriteSpecifiedValueFromReference();
                //Event 
                _Archive.OnArchiveEventsMethod("Event - Przezbrojenie");
            }
            else
            {//komunikat dla usera
                MessageBox.Show("Nie wybrano referencji do wczytania");
            }
            formsPlot1.Plot.Clear();
            App.CreateStaticPlot();
            formsPlot1.Refresh();
        }

        //Timer co 10ms do oczytywania danych 
        private void timer1_Tick(object sender, EventArgs e)
        {
            //zakomentuj ponizsze dwie metody do odpalenia apki bez PLC
            App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            App.ReadActualValueFromDBReferenceOrProcessData("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");

            if (blockade)
            {
                App.CreatePlot();
            }
            PassedValueControls.Run();
            //aktualizacja daty i godziny
            this.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            label_DataIGodzina.Text = this.Text;


        }

        //Przycisk wyzwalajacy zapis uzytkownika
        private void button6_Click(object sender, EventArgs e)
        {
            Users.SaveToXML();
        }

        //Wyczyszczenie statusu karty Użytkownicy
        private void timer2_Tick(object sender, EventArgs e)
        {
            StatusyLogowania.Text = "";
            CzyszczenieStatusówLogowania.Enabled = false;
        }

        //Wyswietlenie uzytkownikow z bazy
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_Imie_Edycja.Text = comboBox_ListaUzytkWBazie.SelectedItem.ToString();
            Users.DisplayValuesFromXML(Users.LoadFromXML("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\UserData.xml"), textBox_Imie_Edycja.Text);
        }

        //Przycisk wyzwalający edycje użytkownika
        private void button7_Click(object sender, EventArgs e)
        {
            Users.EditXML();
        }

        //Przycisk zastepujacy event przyłożenia karty RFID do czytnika
        private void buttonZalogujUzytk(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            OdliczaSekunde.Enabled = false;
            Users.Interval = 100000 / 1000;
            label13.Text = Users.Interval.ToString();
            Users.FindUserinXML();
            //Po zalogowaniu uruchamiamy metode OnArchiveEventsMethod - jak bedzie subscriber podpiety pod event to odpali ona Event
            if (Users.UserIsLoggedIn)
            {
                _Archive.OnArchiveEventsMethod("Event - Logowanie użytkownika");
            }
            Users.EnabledObjects();

            CzyszczenieStatusówLogowania.Enabled = true;
        }

        //Wylogowanie uzytkownika po uplywie czasu
        private void TimeoutWylogowania_Tick(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            TimeoutWylogowania.Enabled = false;
            Users.EnabledObjects();
            StatusyLogowania.Text = "Uzytkownik wylogowany automatycznie";
            CzyszczenieStatusówLogowania.Enabled = true;
        }


        //Obsluga odliczania czasu do wylogowania
        private void OdliczaSekunde_Tick(object sender, EventArgs e)
        {
            if (TimeoutWylogowania.Enabled)
            {
                Users.Interval -= 1;
                label13.Text = Users.Interval.ToString();
            }
            else
            {
                OdliczaSekunde.Enabled = false;
                Users.Interval = 100000 / 1000;
                label13.Text = Users.Interval.ToString();
            }
        }


        // PRZYCISK WYZWALAJACY ZAPIS referencji
        private void button13_Click(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.InsertToDataBase();

        }


        //Wyrzucenie referencji po rozwinieciu comboboxa
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.SelectFromDataBase(comboBoxListaReferencji.Text);
        }


        //Usuwanie wybranej referencji
        private void button2_Click_1(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            if (MessageBox.Show("Czy na pewno chcesz usunąć referencję?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
                {
                    database.Delete(comboBoxListaReferencji.Text);
                    App.ClearAllValueInForm1("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
                }
            };
        }


        //Zamiana kropki na przecinek
        private void CommaReplaceDotTextBox(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    //if(ctrl.Name == "textbox1")
                    TextBox textBox = (TextBox)ctrl;
                    textBox.KeyPress += TextBox_KeyPress;
                }
                else if (ctrl.HasChildren)
                {
                    CommaReplaceDotTextBox(ctrl);
                }
            }
        }


        //Obsluga zamiany kropki na przecinek
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            TextBox textBox = (TextBox)sender;
            // Jeśli naciśnięto kropkę, zamień na przecinek
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }


        // Obsluga OnScreenKeyboard po wcisnieciu TextBoxa - OGARNAC ZEBY NIE WYSWIETLALA SIE JAK JUZ JEST NA EKRANIE
        private void TextBox_Click(object sender, EventArgs e)
        {
            Process[] oslProcessArry = Process.GetProcessesByName("TabTip");
            foreach (Process oslProcess in oslProcessArry)
            {
                oslProcess.Kill();
            }
            string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
            Process.Start(progFiles);
        }


        //Obsluga ToggleButtonow w zakładce Tryb Reczny
        #region obsluga ToggleButtonow - Tryb Reczny
        private void checkBox_Serwo18U1_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("15", checkBox_Serwo18U1.Tag.ToString(), 1);
        }
        private void checkBox_Serwo18U1_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo18U1.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_18U1);
            }
        }
        private void checkBox_Serwo20U1_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("11", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_Serwo20U1_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo20U1.Checked)
            {
                checkBox_Serwo18U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_20U1);
            }
        }
        private void checkBox_Serwo16U2_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("12", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_Serwo16U2_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo16U2.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_16U2);
            }
        }
        private void checkBox_UkladPodnoszenia_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("13", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_UkladPodnoszenia_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_UkladPodnoszenia.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.UkladPodnoszenia);
            }
        }
        private void checkBox_ChwytakGorny_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("14", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_ChwytakGorny_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ChwytakGorny.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ChwytakGorny);
            }
        }
        private void checkBox_ChwytakDolny_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("16", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_ChwytakDolny_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ChwytakDolny.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ChwytakDolny);
            }
        }
        private void checkBox_ZaciskTulipa_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("17", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        private void checkBox_ZaciskTulipa_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ZaciskTulipa.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ZaciskTulipa);
            }
        }
        private void checkBox_SzczekiOslonki_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("18", checkBox_SzczekiOslonki.Tag.ToString(), 1);
        }
        private void checkBox_SzczekiOslonki_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_SzczekiOslonki.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.SzczekiOslonki);
            }
        }

        private void checkBox_DyszaPionowa_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("19", checkBox_DyszaPionowa.Tag.ToString(), 1);
        }
        private void checkBox_DyszaPionowa_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_DyszaPionowa.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_DyszaPozioma.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Main);
            }
        }

        private void checkBox_DyszaPozioma_Checked(object sender, EventArgs e)
        {
            App.WriteToDB("20", checkBox_DyszaPozioma.Tag.ToString(), 1);
        }
        private void checkBox_DyszaPozioma_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_DyszaPozioma.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
                checkBox_Serwo16U2.Checked = false;
                checkBox_Serwo18U1.Checked = false;
                checkBox_ChwytakGorny.Checked = false;
                checkBox_ChwytakDolny.Checked = false;
                checkBox_ZaciskTulipa.Checked = false;
                checkBox_UkladPodnoszenia.Checked = false;
                checkBox_DyszaPionowa.Checked = false;
                checkBox_SzczekiOslonki.Checked = false;
                pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Main);
            }
        }
        #endregion


        //Zamkniecie aplikacji
        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            App.ClosePLCConnection();
            //serialPortReader.Close();
        }


        //Zamkniecie aplikacji z przycisku
        private void button3_Click(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            //serialPortReader.Close();
            Close();
        }

        //Przycisk Rework Event
        private void Button_Rework_Click(object sender, EventArgs e)
        {
            App.WriteToDB("57", Button_Rework.Tag.ToString(), 1);
        }

        //Przycisk Reset NOK event
        private void Button_ResetSztukNOK_Click(object sender, EventArgs e)
        {
            App.WriteToDB("58", Button_ResetSztukNOK.Tag.ToString(), 1);
        }

        //Przycisk Reset OK event
        private void Button_ResetSztukOK_Click(object sender, EventArgs e)
        {
            App.WriteToDB("59", Button_ResetSztukOK.Tag.ToString(), 1);
        }

        //Przycisk Dawka testowa przegub event
        private void Button_DawkaTestowaPrzegub_Click(object sender, EventArgs e)
        {
            App.WriteToDB("56", Button_DawkaTestowaPrzegub.Tag.ToString(), 1);
        }

        //Przycisk Dawka testowa tulip event
        private void Button_DawkaTestowaTulip_Click(object sender, EventArgs e)
        {
            App.WriteToDB("55", Button_DawkaTestowaTulip.Tag.ToString(), 1);
        }

        private void Btn_ExportToCSVArchiwizacja_Click(object sender, EventArgs e)
        {
            _Archive.ExportToCSVButtonFromForm1();
            CzyszczenieStatusowArchiwizacji.Enabled = true;
        }

        private void CzyszczenieStatusowArchiwizacji_Tick(object sender, EventArgs e)
        {
            label_StatusyArchiwizacji.Text = "";
            CzyszczenieStatusowArchiwizacji.Enabled = false;
        }
    }
}
