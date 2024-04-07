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




namespace HMIApp
{
    public partial class Form1 : Form
    {

        App App = new App();
        UserAdministration Users = new UserAdministration();
        DataBase DataBase = new DataBase();
        //Services do dependency injection
        ServiceCollection services = new ServiceCollection();
        ServiceProvider serviceProvider;
        SerialPortReader serialPortReader = new SerialPortReader();

        //konstruktor Form1
        public Form1()
        {
            InitializeComponent();


            _Form1 = this;
            //zakomentuj ponizsze cztery metody do odpalenia apki bez PLC
            App.RunInitPLC();
            App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            App.ReadActualValueFromDBChart_Simplified("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_4.csv");
            App.ReadActualValueFromDBReferenceOrProcessData("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");

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
            {
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
                App.WriteToDB(DB666Tag0.Checked.ToString(), DB666Tag0.Tag.ToString());
                App.WriteToDB(DB666Tag1.Checked.ToString(), DB666Tag1.Tag.ToString());
                App.WriteToDB(DB666Tag8.Checked.ToString(), DB666Tag8.Tag.ToString());
                App.WriteToDB(DB666Tag9.Checked.ToString(), DB666Tag9.Tag.ToString());
                App.WriteToDB(DB666Tag10.Checked.ToString(), DB666Tag10.Tag.ToString());
                App.WriteToDB(DB666Tag11.Checked.ToString(), DB666Tag11.Tag.ToString());
                App.WriteToDB(DB666Tag12.Checked.ToString(), DB666Tag12.Tag.ToString());
                App.WriteToDB(DB666Tag13.Checked.ToString(), DB666Tag13.Tag.ToString());
                App.WriteToDB(DB666Tag2.Text, DB666Tag2.Tag.ToString());
                App.WriteToDB(DB666Tag4.Text, DB666Tag4.Tag.ToString());
                App.WriteToDB(DB666Tag6.Text, DB666Tag6.Tag.ToString());
                App.WriteToDB(DB666Tag16.Text, DB666Tag16.Tag.ToString());
                App.WriteToDB(DB666Tag17.Text, DB666Tag17.Tag.ToString());
                App.WriteToDB(DB666Tag3.Text, DB666Tag3.Tag.ToString());
                App.WriteToDB(DB666Tag5.Text, DB666Tag5.Tag.ToString());
                App.WriteToDB(DB666Tag15.Text, DB666Tag15.Tag.ToString());
                App.WriteToDB(DB666Tag7.Text, DB666Tag7.Tag.ToString());
                App.WriteToDB(DB666Tag14.Text, DB666Tag14.Tag.ToString());
                App.WriteToDB(DB666Tag18.Text, DB666Tag18.Tag.ToString());
                App.WriteToDB(DB666Tag19.Text, DB666Tag19.Tag.ToString());
                App.WriteToDB(DB666Tag20.Text, DB666Tag20.Tag.ToString());
                App.WriteToDB(DB666Tag21.Text, DB666Tag21.Tag.ToString());
                App.WriteToDB(DB666Tag22.Text, DB666Tag22.Tag.ToString());
                //Przepisanie wartosci do wygenerowania okna czytania sily w wykresie
                App.WriteSpecifiedValueFromReference();
            }
            else
            {
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
            App.CreatePlot();
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
            listBoxStatusyLogowania.Items.Clear();
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
        private void timer3_Tick(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            TimeoutWylogowania.Enabled = false;
            Users.EnabledObjects();
        }

        //Obsluga odliczania czasu do wylogowania
        private void timer4_Tick(object sender, EventArgs e)
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

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Jeśli naciśnięto kropkę, zamień na przecinek
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

            dataGridView1.Visible = false;
            ButtonOKClosePopUpAlarms.Visible = false;
        }

        private void checkBox2_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("15", checkBox2.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_18U1);
        }
        private void checkBox2_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }
        private void checkBox1_Checked(object sender, EventArgs e)
        {

            App.WriteToDB("11", checkBox1.Tag.ToString(), 1);
            pictureBoxMachineImages.Image = new Bitmap(Properties.Resources.Serwo_20U1);
        }
        private void checkBox1_CheckedStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }

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

    }
}
