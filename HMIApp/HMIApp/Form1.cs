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
        public Form1()
        {
            InitializeComponent();
            _Form1 = this;

            //Services do dependency injection
            var services = new ServiceCollection();
            services.AddSingleton<iApp, App>();
            //services.AddSingleton<iCSVReader, CSVReader>();

            //ZArejestrowanie DBContextu - connection string wrzucic pozniej w jakis plik konfiguracyjny
            services.AddDbContext<HMIAppDBContext>(options => options
            .UseSqlServer("Data Source=NB022173\\SQLEXPRESS;Initial Catalog=test699;Integrated Security=True;Trust Server Certificate=True"));

            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<iApp>();

            Users.Run();
            //App.RunInitPLC();
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            OdczytDB.Enabled = true;
            listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(App.listBox1_DrawItem);
            label63.Text = "";
            Users.EnabledObjects();


        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;

        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(DB667Tag000.Checked.ToString(), DB667Tag000.Tag.ToString());
            App.WriteToDB(DB667Tag111.Checked.ToString(), DB667Tag111.Tag.ToString());
            App.WriteToDB(DB667Tag888.Checked.ToString(), DB667Tag888.Tag.ToString());
            App.WriteToDB(DB667Tag999.Checked.ToString(), DB667Tag999.Tag.ToString());
            App.WriteToDB(DB667Tag100.Checked.ToString(), DB667Tag100.Tag.ToString());
            App.WriteToDB(DB667Tag1111.Checked.ToString(), DB667Tag1111.Tag.ToString());
            App.WriteToDB(DB667Tag122.Checked.ToString(), DB667Tag122.Tag.ToString());
            App.WriteToDB(DB667Tag133.Checked.ToString(), DB667Tag133.Tag.ToString());
            App.WriteToDB(DB667Tag222.Text, DB667Tag222.Tag.ToString());
            App.WriteToDB(DB667Tag444.Text, DB667Tag444.Tag.ToString());
            App.WriteToDB(DB667Tag666.Text, DB667Tag666.Tag.ToString());
            App.WriteToDB(DB667Tag2222.Text, DB667Tag2222.Tag.ToString());
            App.WriteToDB(DB667Tag2223.Text, DB667Tag2223.Tag.ToString());


        }

        //Timer co 100ms do oczytywania DBka
        private void timer1_Tick(object sender, EventArgs e)
        {
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

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


    }
}
