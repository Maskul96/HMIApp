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
using System.IO.Ports;




namespace HMIApp
{
    public partial class Form1 : Form
    {
        #region obiekty klas
        Archivization _Archive = new();
        App App = new();
        UserAdministration Users = new();
        public SerialPortReader serialPortReader = new();

        #endregion
        #region Services do dependency injection i EF
        public ServiceCollection services = new();
        public ServiceProvider serviceProvider;
        #endregion
        //Zmienna do Blokady rysowania wykresu dopoki nie zaczytasz referencji
        public bool blockade;
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;
        #region Względna ścieżka do folderu ArchivizationsFromDataBase
        // Określenie ścieżki folderu ArchivizationsFromDataBase, który chcemy wyświetlić - Ścieżka względna
        public static string basePathToHMIAppFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        public static string basePathToFilesFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Resources", "Files"));
        public static string basePathAndArchivizationsFromDataBaseCSV = Path.Combine(basePathToFilesFolder, "ArchivizationsFromDataBaseCSV");
        public static string relativePath = "ArchivizationsFromDataBase";
        public static string absolutePath = Path.Combine(basePathAndArchivizationsFromDataBaseCSV, relativePath);
        string folderPath = absolutePath;
        #endregion

        public Form1()
        {
            InitializeComponent();
            _Form1 = this;

            services.AddSingleton<IDataBase, DataBase>();
            services.AddDbContext<HMIAppDBContext>();
            serviceProvider = services.BuildServiceProvider();

            ReadFromDbWhenAppIsStarting();

            Users.Run();

            OdczytDB.Enabled = true;

            listBoxWarningsView.DrawItem += new DrawItemEventHandler(App.listBoxWarningsView_DrawItem);
            label13.Text = "";
            Users.EnabledObjects();

            CommaReplaceDotTextBox(this);

            PassedValueControls.Run();

            _Archive.Run();
            _Archive.ArchiveEventRun();

            serialPortReader.InitializeSerialPort();
            serialPortReader.Run();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //zakomentuj ponizsze cztery metody do odpalenia apki bez PLC
            App.RunInitPLC();
            PróbyUruchomieniaKomunikacjizPLC.Enabled = true;
            App.ReadAlarmsFromDB(Path.Combine(basePathToFilesFolder, "tags_zone_2.csv"));
            App.ReadIOFromDB(Path.Combine(basePathToFilesFolder, "tags_zone_3.csv"));
            App.ReadActualValueFromDBChart_Simplified(Path.Combine(basePathToFilesFolder, "tags_zone_4.csv"));
            App.ReadActualValueFromDBReferenceOrProcessData(Path.Combine(basePathToFilesFolder, "tags_zone_1.csv"));

            //Blokada rysowania wykresu dopoki nie zaczytasz referencji
            blockade = false;
            // Wywołanie funkcji do dodawania węzłów do drzewa
            PopulateTreeView(folderPath, treeView1.Nodes);
            label_WczytajReferencje.Visible = true;
        }

        //Zamkniecie aplikacji
        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            App.ClosePLCConnection();
            serialPortReader.Close();
        }

        //Zamkniecie aplikacji z przycisku
        private void Button_Click_CloseAPP(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            //serialPortReader.Close();
            Close();
        }

        //Przekazanie receivedData z portu szeregowego z klasy SerialPortReader do textboxów
        //Właściwość ReadSerialPort umożliwia ustawienie TextBox właściwości kontrolki Text na nową wartość.Metoda wykonuje zapytanie InvokeRequired.
        //Jeśli InvokeRequired zwraca wartość true, ReadSerialPort rekursywnie wywołuje samą siebie, przekazując metodę jako delegata do Invoke metody.
        //Jeśli InvokeRequired funkcja zwraca false wartość , ReadSerialPort ustawia TextBox.Text wartość bezpośrednio
        public void ReadSerialPort(string receivedData)
        {
            if (buttonDodajUzytkownika.Visible == false)
            {
                if (textBox_MiejsceNaNrKarty_Zaloguj.InvokeRequired)
                {
                    Action safewrite = delegate { ReadSerialPort(receivedData); };
                    textBox_MiejsceNaNrKarty_Zaloguj.Invoke(safewrite);
                    //Action buttonZaloguj = delegate { ButtonZalogujUzytk(null, null); };
                    //ButtonZalogujUzytk(null,null).Invoke(buttonZaloguj);
                }
                else
                {
                    textBox_MiejsceNaNrKarty_Zaloguj.Text = receivedData;
                }
            }
            else
            {
                if (textbox_NumerKarty_DodajUzytk.InvokeRequired)
                {
                    Action safewrite = delegate { ReadSerialPort(receivedData); };
                    textbox_NumerKarty_DodajUzytk.Invoke(safewrite);
                }
                else
                {
                    textbox_NumerKarty_DodajUzytk.Text = receivedData;
                }
            }
        }

        //Timer do prób uruchomienia komunikacji z PLC - zakomentowane bo crashuje apke
        private void Timer_Tick_TryCommunicateWithPLC(object sender, EventArgs e)
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
            var database = serviceProvider.GetService<IDataBase>();
            database.SelectFromDbToComboBox();
            //Najpierw odczyt z combobox zeby potem moc odczytac z bazy danych pierwszy element na starcie
            if (comboBoxListaReferencji.Items.Count > 0)
            {
                database.SelectFromDataBase(comboBoxListaReferencji.Items[0].ToString());
            }
        }

        //wczytaj referencje do PLC 
        private void Button_Click_WriteReferenceToPLC(object sender, EventArgs e)
        {
            if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
            {
                //Zapisz referencje do bazy danych
                var database = serviceProvider.GetService<IDataBase>();
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
                label_WczytajReferencje.Visible = false;
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

        //Timer co 1ms do oczytywania danych 
        private void Timer_Tick_ReadDataFromDB(object sender, EventArgs e)
        {
            //zakomentuj ponizsze dwie metody do odpalenia apki bez PLC
            App.ReadAlarmsFromDB(Path.Combine(basePathToFilesFolder, "tags_zone_2.csv"));
            App.ReadIOFromDB(Path.Combine(basePathToFilesFolder, "tags_zone_3.csv"));
            App.ReadActualValueFromDBReferenceOrProcessData(Path.Combine(basePathToFilesFolder, "tags_zone_1.csv"));
            if (blockade)
            {
                App.CreatePlot();
            }

            PassedValueControls.Run();
            //aktualizacja daty i godziny
            this.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            label_DataIGodzina.Text = this.Text;
        }

        //Przycisk wyzwalajacy zapis/dodanie uzytkownika
        private void Button_Click_SaveUser(object sender, EventArgs e)
        {
            Users.SaveToXML();
            UnVisibleSequenceOfAddUser();
            TimerDodaniaUzytkownika.Enabled = false;
            textbox_NumerKarty_DodajUzytk.Text = "";
            textBoxImie_DodajUzytk.Text = "";
        }

        //Wyswietlenie uzytkownikow z bazy
        private void ComboBox_ShowEditUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_Imie_Edycja.Text = comboBox_ListaUzytkWBazie.SelectedItem.ToString();
            Users.DisplayValuesFromXML(Users.LoadFromXML(Path.Combine(basePathToFilesFolder, "UserData.xml")), textBox_Imie_Edycja.Text);
        }

        //Przycisk wyzwalający edycje użytkownika
        private void Button_Click_EditUsers(object sender, EventArgs e)
        {
            Users.EditXML();
        }

        //Przycisk zastepujacy event przyłożenia karty RFID do czytnika
        private void ButtonZalogujUzytk(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            OdliczaSekunde.Enabled = false;
            Users.Interval = 100000 / 1000;
            label13.Text = Users.Interval.ToString();
            Users.FindUserinXML();
            //Po zalogowaniu uruchamiamy metode OnArchiveEventsMethod - jak bedzie subscriber podpiety pod event to odpali ona Event
            if (Users.hasAccess)
            {
                _Archive.OnArchiveEventsMethod("Event - Logowanie użytkownika");
                Users.EnabledObjects();
            }

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
            textBox_MiejsceNaNrKarty_Zaloguj.Text = "";
            textbox_NumerKarty_DodajUzytk.Text = "";
            textBoxImie_DodajUzytk.Text = "";
            UnVisibleSequenceOfAddUser();
            TimerDodaniaUzytkownika.Enabled = false;
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

        //Wyczyszczenie statusu karty Użytkownicy
        private void Timer_Tick_CzyszczenieStatusuLogowania(object sender, EventArgs e)
        {
            StatusyLogowania.Text = "";
            CzyszczenieStatusówLogowania.Enabled = false;
        }

        // PRZYCISK WYZWALAJACY dodanie i zapis referencji do bazy
        private void Button_Click_AddAndSaveReference(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<IDataBase>();
            database.InsertToDataBase();
        }

        //Wyrzucenie referencji po rozwinieciu comboboxa
        private void ComboBoxListaReferencji_SelectedIndexChanged(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<IDataBase>();
            database.SelectFromDataBase(comboBoxListaReferencji.Text);
        }

        //Usuwanie wybranej referencji
        private void Button_Click_DeleteReference(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<IDataBase>();
            if (MessageBox.Show("Czy na pewno chcesz usunąć referencję?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
                {
                    database.Delete(comboBoxListaReferencji.Text);
                    App.ClearAllValueInForm1(Path.Combine(basePathToFilesFolder, "tags_zone_0.csv"));
                }
            }
        }
        //Event zmiany koloru - wyzwala metode uruchmiajaca Event przejscia w Auto/Man
        public void BackColor_ColorChanged(object sender, EventArgs e)
        {
            if (DB666NrReferencePassedValue.Text != "")
            {
                //PO NAZWIE KOLORU ROZPOZNAJEMY, ŻE ZMIENIŁ SIĘ KOLOR IKONKI TZN, ŻE ZAISTNIAŁO PRZEJŚCIE W AUTO/MANUAL
                //Lepszy jest taki sposób niż sprawdzanie na sztywno danego adresu z PLC, bo jak ktoś nieumyślnie zmieni coś w DBku i nie zaktualizuje w Apce to nic nie będzie się archiwizować
                if (DB667Auto.BackColor.Name == "LimeGreen")
                {
                    _Archive.OnArchiveEventsMethod("Event - Przejście w Auto");
                }
                else if (DB667Man.BackColor.Name == "LimeGreen")
                {
                    _Archive.OnArchiveEventsMethod("Event - Przejście w Man");
                }
            }
        }

        //Obsluga ToggleButtonow w zakładce Tryb Reczny
        #region obsluga ToggleButtonow - Tryb Reczny i Przycisków na Ekranie głównym
        #region SERWO 18U1
        private void CheckBox_Serwo18U1_Checked(object sender, EventArgs e)
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
        private void CheckBox_Serwo18U1_Click(object sender, EventArgs e)
        {
            App.WriteToDB("15", checkBox_Serwo18U1.Tag.ToString(), 1);
        }
        #endregion
        #region SERWO 20U1
        private void CheckBox_Serwo20U1_Checked(object sender, EventArgs e)
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
        private void CheckBox_Serwo20U1_Click(object sender, EventArgs e)
        {
            App.WriteToDB("11", checkBox_Serwo20U1.Tag.ToString(), 1);
        }
        #endregion
        #region SERWO 16U2
        private void CheckBox_Serwo16U2_Checked(object sender, EventArgs e)
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
        private void CheckBox_Serwo16U2_Click(object sender, EventArgs e)
        {
            App.WriteToDB("12", checkBox_Serwo16U2.Tag.ToString(), 1);
        }
        #endregion
        #region UKŁAD PODNOSZENIA
        private void CheckBox_UkladPodnoszenia_Checked(object sender, EventArgs e)
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
        private void CheckBox_UkladPodnoszenia_CheckedStateChanged(object sender, EventArgs e)
        {
            App.WriteToDB("13", checkBox_UkladPodnoszenia.Tag.ToString(), 1);
        }
        #endregion
        #region CHWYTAK GÓRNY
        private void CheckBox_ChwytakGorny_Checked(object sender, EventArgs e)
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
        private void CheckBox_ChwytakGorny_Click(object sender, EventArgs e)
        {
            App.WriteToDB("14", checkBox_ChwytakGorny.Tag.ToString(), 1);
        }
        #endregion
        #region CHWYTAK DOLNY
        private void CheckBox_ChwytakDolny_Checked(object sender, EventArgs e)
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
        private void CheckBox_ChwytakDolny_Click(object sender, EventArgs e)
        {
            App.WriteToDB("16", checkBox_ChwytakDolny.Tag.ToString(), 1);
        }
        #endregion
        #region ZACISK TULIPA
        private void CheckBox_ZaciskTulipa_Checked(object sender, EventArgs e)
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
        private void CheckBox_ZaciskTulipa_Click(object sender, EventArgs e)
        {
            App.WriteToDB("17", checkBox_ZaciskTulipa.Tag.ToString(), 1);
        }
        #endregion
        #region SZCZĘKI OSŁONKI
        private void CheckBox_SzczekiOslonki_Checked(object sender, EventArgs e)
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
        private void CheckBox_SzczekiOslonki_Click(object sender, EventArgs e)
        {
            App.WriteToDB("18", checkBox_SzczekiOslonki.Tag.ToString(), 1);
        }
        #endregion
        #region DYSZA PIONOWA
        private void CheckBox_DyszaPionowa_Checked(object sender, EventArgs e)
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
        private void CheckBox_DyszaPionowa_Click(object sender, EventArgs e)
        {
            App.WriteToDB("19", checkBox_DyszaPionowa.Tag.ToString(), 1);
        }
        #endregion
        #region DYSZA POZIOMA
        private void CheckBox_DyszaPozioma_Checked(object sender, EventArgs e)
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
        private void CheckBox_DyszaPozioma_Click(object sender, EventArgs e)
        {
            App.WriteToDB("20", checkBox_DyszaPozioma.Tag.ToString(), 1);
        }
        #endregion
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

        private void button_TestLampek_Click(object sender, EventArgs e)
        {
            App.WriteToDB("100", button_TestLampek.Tag.ToString(), 1);
        }
        #endregion

        //Przycisk exportu danych z archiwizacji do plik ucsv
        private void Btn_ExportToCSVArchiwizacja_Click(object sender, EventArgs e)
        {
            if (comboBox_StartYear.Text != "" && comboBox_StartMonth.Text != "" && comboBox_StartDay.Text != "" && comboBox_EndYear.Text != "" && comboBox_EndMonth.Text != "" && comboBox_EndDay.Text != "")
            {
                _Archive.ExportToCSVButtonFromForm1();
                CzyszczenieStatusowArchiwizacji.Enabled = true;
                // Wywołanie funkcji do dodawania węzłów do drzewa
                PopulateTreeView(folderPath, treeView1.Nodes);
            }
            else
            {
                System.Windows.MessageBox.Show("Wpisz datę, miesiąc oraz dzień aby wygenerować plik csv");
            }
        }
        //Czyszczenie info dla uzytkownika - status archiwizacji
        private void CzyszczenieStatusowArchiwizacji_Tick(object sender, EventArgs e)
        {
            label_StatusyArchiwizacji.Text = "";
            CzyszczenieStatusowArchiwizacji.Enabled = false;
        }

        #region Metody pomocnicze dla Form1
        //Zamiana kropki na przecinek
        private void CommaReplaceDotTextBox(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
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
        // Funkcja rekurencyjna do dodawania węzłów do drzewa
        private static void PopulateTreeView(string directory, TreeNodeCollection parentNode)
        {
            // Dodawanie węzłów reprezentujących pliki
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                TreeNode node = new(Path.GetFileName(file));
                parentNode.Add(node);
            }
        }
        #endregion

        //Uruchomienie opcji dodawania użytkownika po przycisnięiu przycisku Rozpocznij
        private void button_RozpocznijDodawanieUzytk_Click(object sender, EventArgs e)
        {
            label_NumerKarty_DodajUzytk.Visible = true;
            label_Imie_DodajUzytk.Visible = true;
            label_Uprawnienia_DodajUzytk.Visible = true;
            buttonDodajUzytkownika.Visible = true;
            textbox_NumerKarty_DodajUzytk.Visible = true;
            textBoxImie_DodajUzytk.Visible = true;
            comboBox_ListaUprawnien_DodajUzytk.Visible = true;
            TimerDodaniaUzytkownika.Enabled = true;
        }
        //Wyłączenie opcji dodawania użytkownika po braku kontynuowania dodawania użytkownika
        private void TimerDodaniaUzytkownika_Tick(object sender, EventArgs e)
        {
            UnVisibleSequenceOfAddUser();
            TimerDodaniaUzytkownika.Enabled = false;
        }

        private void UnVisibleSequenceOfAddUser()
        {
            label_NumerKarty_DodajUzytk.Visible = false;
            label_Imie_DodajUzytk.Visible = false;
            label_Uprawnienia_DodajUzytk.Visible = false;
            buttonDodajUzytkownika.Visible = false;
            textbox_NumerKarty_DodajUzytk.Visible = false;
            textBoxImie_DodajUzytk.Visible = false;
            comboBox_ListaUprawnien_DodajUzytk.Visible = false;
        }
    }
}
