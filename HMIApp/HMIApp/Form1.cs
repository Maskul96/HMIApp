using HMIApp.Components.CSVReader;
using HMIApp.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows.Forms;
using HMIApp.Components.UserAdministration;
using HMIApp.Data;
using Microsoft.EntityFrameworkCore;
using Humanizer;


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

        //konstruktor Form1
        public Form1()
        {
            InitializeComponent();
            _Form1 = this;

            App.RunInitPLC();
            App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

            DataBase.Run();
            #region zakomentowane services ktore bylo wczesniej
            ////Services do dependency injection
            //var services = new ServiceCollection();
            //services.AddSingleton<iDataBase, DataBase>();
            ////ZArejestrowanie DBContextu - connection string wrzucic pozniej w jakis plik konfiguracyjny
            //services.AddDbContext<HMIAppDBContext>(options => options
            //.UseSqlServer(Data.ConnectionString));
            //var serviceProvider = services.BuildServiceProvider();
            //var database = serviceProvider.GetService<iDataBase>();
            //database.SaveToDataBase();
            //database.ReadFromDataBase();
            # endregion koniec komentarza services

            //Services do dependency injection
            services.AddSingleton<iDataBase, DataBase>();
            //ZArejestrowanie DBContextu - Stworzenie połączenia do bazy danych i service providera
            services.AddDbContext<HMIAppDBContext>(options => options
            .UseSqlServer(DataBase.ConnectionString));
            serviceProvider = services.BuildServiceProvider();
            ReadFromDb();
            Users.Run();
    
            OdczytDB.Enabled = true;
            listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(App.listBox1_DrawItem);
            label63.Text = "";
            Users.EnabledObjects();


        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;

        //TESTOWA METODA DO ODCZYTU DANYCH Z BAZY
        public void ReadFromDb()
        {
            var database = serviceProvider.GetService<iDataBase>();
            // database.SelectFromDataBase();
            database.SelectFromDataBase("12345");
            database.SelectFromDbToComboBox();
        }

        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(DB666Tag0PassedValue.Checked.ToString(), DB666Tag0PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag1PassedValue.Checked.ToString(), DB666Tag1PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag8PassedValue.Checked.ToString(), DB666Tag8PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag9PassedValue.Checked.ToString(), DB666Tag9PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag10PassedValue.Checked.ToString(), DB666Tag10PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag11PassedValue.Checked.ToString(), DB666Tag11PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag12PassedValue.Checked.ToString(), DB666Tag12PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag13PassedValue.Checked.ToString(), DB666Tag13PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag2PassedValue.Text, DB666Tag2PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag4PassedValue.Text, DB666Tag4PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag6PassedValue.Text, DB666Tag6PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag16PassedValue.Text, DB666Tag16PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag17PassedValue.Text, DB666Tag17PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag3PassedValue.Text, DB666Tag3PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag5PassedValue.Text, DB666Tag5PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag15PassedValue.Text, DB666Tag15PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag7PassedValue.Text, DB666Tag7PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag14PassedValue.Text, DB666Tag14PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag18PassedValue.Text, DB666Tag18PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag19PassedValue.Text, DB666Tag19PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag20PassedValue.Text, DB666Tag20PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag21PassedValue.Text, DB666Tag21PassedValue.Tag.ToString());
            App.WriteToDB(DB666Tag22PassedValue.Text, DB666Tag22PassedValue.Tag.ToString());

            var database = serviceProvider.GetService<iDataBase>();
            database.UpdateDb("12345");
        }

        //Timer co 100ms do oczytywania DBka
        private void timer1_Tick(object sender, EventArgs e)
        {
            App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

            this.Text = DateTime.Now.ToString();
            label57.Text = this.Text;
            maskedTextBox1.Text = textBox2.Text;

        }

        //Cofniecie zmian dokonanych w karcie Dane - Modyfikowalne
        private void button2_Click(object sender, EventArgs e)
        {
            App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
        }

        //Zamkniecie aplikacji
        private void button3_Click(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            Close();
        }

        //Testowy przycisk z karty Manual do wyslania komendy
        private void button4_Click(object sender, EventArgs e)
        {
            App.WriteToDB("15", button4.Tag.ToString());
        }

        //Testowy przycisk z karty Manual do wyslania komendy
        private void button5_Click(object sender, EventArgs e)
        {
            App.WriteToDB("11", button5.Tag.ToString());
        }

        //Przycisk wyzwalajacy zapis uzytkownika
        private void button6_Click(object sender, EventArgs e)
        {
            Users.SaveToXML();
        }

        //Wyczyszczenie statusu karty Użytkownicy
        private void timer2_Tick(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        //Wyswietlenie uzytkownikow z bazy
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox15.Text = comboBox2.SelectedItem.ToString();
            Users.DisplayValuesFromXML(Users.LoadFromXML("document.xml"), textBox15.Text);

        }

        //Przycisk wyzwalający edycje użytkownika
        private void button7_Click(object sender, EventArgs e)
        {
            Users.EditXML();

        }

        //Przycisk zastepujacy event przyłożenia karty RFID d oczytnika
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
                label63.Text = Users.Interval.ToString();
            }
            else
            {
                OdliczaSekunde.Enabled = false;
                Users.Interval = 100000 / 1000;
                label63.Text = Users.Interval.ToString();
            }
        }

        //TESTOWY PRZYCISK WYZWALAJACY ZAPIS
        private void button10_Click(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.InsertToDataBase();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.SelectFromDataBase(comboBox5.Text);
        }
    }
}
