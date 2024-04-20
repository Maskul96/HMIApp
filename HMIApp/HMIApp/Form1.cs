﻿using HMIApp.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using HMIApp.Components.UserAdministration;
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
        App App = new App();
        UserAdministration Users = new UserAdministration();
        DataBase DataBase = new DataBase();
        Logger _logger = new Logger();
        //Services do dependency injection
        ServiceCollection services = new ServiceCollection();
        ServiceProvider serviceProvider;
        SerialPortReader serialPortReader = new SerialPortReader();

        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
            DataBase.Run();
            //Services do dependency injection
            services.AddSingleton<iDataBase, DataBase>();
            //ZArejestrowanie DBContextu - Stworzenie połączenia do bazy danych i service providera
            services.AddDbContext<HMIAppDBContext>(options => options
            .UseSqlServer(DataBase.ConnectionString));
            serviceProvider = services.BuildServiceProvider();
            ReadFromDbWhenAppIsStarting();
            Users.Run();

            OdczytDB.Enabled = true;
            listBoxWarningsView.DrawItem += new System.Windows.Forms.DrawItemEventHandler(App.listBox1_DrawItem);
            label13.Text = "";
            Users.EnabledObjects();

            CommaReplaceDotTextBox(this);
            PassedValueControls.Run();
            App.CreateStaticPlot();
            //serialPortReader.InitializeSerialPort();
            // serialPortReader.Run();
        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;

        private void Form1_Load(object sender, EventArgs e)
        {
            App.RunInitPLC();
            PróbyUruchomieniaKomunikacjizPLC.Enabled = true;
            //zakomentuj ponizsze cztery metody do odpalenia apki bez PLC
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            //App.ReadActualValueFromDBChart_Simplified("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_4.csv");
            //App.ReadActualValueFromDBReferenceOrProcessData("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
        }
        //Timer do prób uruchomienia komunikacji z PLC
        private void Timer1_Tick_1(object sender, EventArgs e)
        {
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

        //Zapisz referencje do bazy danych
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
            {
                var database = serviceProvider.GetService<iDataBase>();
                database.UpdateDb(comboBoxListaReferencji.SelectedItem.ToString());
            }
            else
            {//komunikat dla usera
                MessageBox.Show("Nie wybrano referencji do zapisania");
            }
            formsPlot1.Plot.Clear();
            App.CreateStaticPlot();
            formsPlot1.Refresh();
        }
        //wczytaj referencje do PLC
        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBoxListaReferencji.Text != null && comboBoxListaReferencji.Text != "")
            {
                App.WriteToDB(DB666PrzeciskanieP1.Checked.ToString(), DB666PrzeciskanieP1.Tag.ToString());
                App.WriteToDB(DB666MontazOslonkiP2.Checked.ToString(), DB666MontazOslonkiP2.Tag.ToString());
                App.WriteToDB(DB666OetickerP3.Checked.ToString(), DB666OetickerP3.Tag.ToString());
                App.WriteToDB(DB666DyszaWahliwaP4.Checked.ToString(), DB666DyszaWahliwaP4.Tag.ToString());
                App.WriteToDB(DB666SmarTulipP5.Checked.ToString(), DB666SmarTulipP5.Tag.ToString());
                App.WriteToDB(DB666SmarPrzegubP6.Checked.ToString(), DB666SmarPrzegubP6.Tag.ToString());
                App.WriteToDB(DB666TraceUpP7.Checked.ToString(), DB666TraceUpP7.Tag.ToString());
                App.WriteToDB(DB666RFIDGlowicaGornaP9.Checked.ToString(), DB666RFIDGlowicaGornaP9.Tag.ToString());
                App.WriteToDB(DB666PozNakladania_Oslonka.Text, DB666PozNakladania_Oslonka.Tag.ToString());
                App.WriteToDB(DB666SilaMax__Przeciskanie.Text, DB666SilaMax__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666PoczCzytSily__Przeciskanie.Text, DB666PoczCzytSily__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666NrReference.Text, DB666NrReference.Tag.ToString());
                App.WriteToDB(DB666NameOfClient.Text, DB666NameOfClient.Tag.ToString());
                App.WriteToDB(DB666SilaMin_Przeciskanie.Text, DB666SilaMin_Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666PozStartowa__Przeciskanie.Text, DB666PozStartowa__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666KoniecCzytSily__Przeciskanie.Text, DB666KoniecCzytSily__Przeciskanie.Tag.ToString());
                App.WriteToDB(DB666PozStartowa_Oslonka.Text, DB666PozStartowa_Oslonka.Tag.ToString());
                App.WriteToDB(DB666PozSmarowania_Oslonka.Text, DB666PozSmarowania_Oslonka.Tag.ToString());
                App.WriteToDB(DB666PozPowrotu_Oslonka.Text, DB666PozPowrotu_Oslonka.Tag.ToString());
                App.WriteToDB(DB666DawkaPrzegub.Text, DB666DawkaPrzegub.Tag.ToString());
                App.WriteToDB(DB666PozOetickera.Text, DB666PozOetickera.Tag.ToString());
                App.WriteToDB(DB666PozwyjeciaOsi.Text, DB666PozwyjeciaOsi.Tag.ToString());
                App.WriteToDB(DB666TolDawkiPrzegub.Text, DB666TolDawkiPrzegub.Tag.ToString());
                //Przepisanie wartosci do wygenerowania okna czytania sily w wykresie
                App.WriteSpecifiedValueFromReference();
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
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            //App.ReadActualValueFromDBReferenceOrProcessData("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
            //App.CreatePlot();
            //aktualizacja daty i godziny
            this.Text = DateTime.Now.ToString();
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
            StatusyLogowania.Items.Clear();
        }

        //Wyswietlenie uzytkownikow z bazy
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_Imie_Edycja.Text = comboBox_ListaUzytkWBazie.SelectedItem.ToString();
            Users.DisplayValuesFromXML(Users.LoadFromXML("document.xml"), textBox_Imie_Edycja.Text);

        }

        //Przycisk wyzwalający edycje użytkownika
        private void button7_Click(object sender, EventArgs e)
        {
            Users.EditXML();

        }

        //Przycisk zastepujacy event przyłożenia karty RFID do czytnika
        private void button9_Click_1(object sender, EventArgs e)
        {
            Users.FindUserinXML();
            Users.EnabledObjects();
        }

        //Wylogowanie uzytkownika po uplywie czasu
        private void TimeoutWylogowania_Tick(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            TimeoutWylogowania.Enabled = false;
            Users.EnabledObjects();
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
        // Obsluga OnScreenKeyboard po wcisnieciu TextBoxa
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
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_18U1);
        }
        private void checkBox_Serwo18U1_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo18U1.Checked)
            {
                checkBox_Serwo20U1.Checked = false;
            }
        }
        private void checkBox_Serwo20U1_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("11", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_20U1);
        }
        private void checkBox_Serwo20U1_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo20U1.Checked)
            {
                checkBox_Serwo18U1.Checked = false;
            }
        }
        private void checkBox_Serwo16U2_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("12", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_16U2);
        }
        private void checkBox_Serwo16U2_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_Serwo16U2.Checked)
            {
                checkBox_Serwo16U2.Checked = false;
            }
        }
        private void checkBox_UkladPodnoszenia_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("13", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.UkladPodnoszenia);
        }
        private void checkBox_UkladPodnoszenia_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_UkladPodnoszenia.Checked)
            {
                checkBox_UkladPodnoszenia.Checked = false;
            }
        }
        private void checkBox_ChwytakGorny_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("14", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ChwytakGorny);
        }
        private void checkBox_ChwytakGorny_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ChwytakGorny.Checked)
            {
                checkBox_ChwytakGorny.Checked = false;
            }
        }
        private void checkBox_ChwytakDolny_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("16", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ChwytakDolny);
        }
        private void checkBox_ChwytakDolny_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ChwytakDolny.Checked)
            {
                checkBox_ChwytakDolny.Checked = false;
            }
        }
        private void checkBox_ZaciskTulipa_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("17", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.ZaciskTulipa);
        }
        private void checkBox_ZaciskTulipa_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_ZaciskTulipa.Checked)
            {
                checkBox_ZaciskTulipa.Checked = false;
            }
        }
        private void checkBox_SzczekiOslonki_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("18", checkBox_Serwo20U1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.SzczekiOslonki);
        }
        private void checkBox_SzczekiOslonki_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox_SzczekiOslonki.Checked)
            {
                checkBox_SzczekiOslonki.Checked = false;
            }
        }
        #endregion
        //Zamkniecie aplikacji
        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            App.ClosePLCConnection();
            //serialPortReader.Close();
        }

        //Zamkniecie aplikacji
        private void button3_Click(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            //serialPortReader.Close();
            Close();
        }

        private void TimerDoKoloruDataGridView_Tick(object sender, EventArgs e)
        {
            dataGridView1.BackColor = Color.Red;
        }

        private void TimerDoKoloruDataGridView1_Tick(object sender, EventArgs e)
        {
            dataGridView1.BackColor = Color.White;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label51.Text = comboBox1.SelectedIndex.ToString();
        }
    }
}
