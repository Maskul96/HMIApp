using HMIApp.Components.CSVReader;
using HMIApp.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using HMIApp.Components.UserAdministration;

namespace HMIApp
{
    public partial class Form1 : Form
    {
        //Referencje przechowywane w PLC - w pozniejszym etapie zrobic przechowywanie w bazie danych
        //Obiekty z Form1 Design ustawione z dostepem jako public zeby mozna bylo miec do nich dostep z innej klasy poprzez konstruktor
        //Ogarniete odczytywanie/zapisywanie z PLC - teraz je przetestowac i potem ogarnac wlasne ikony
        //WSZYSTKIE TEXTBOXY NAZYWAMY NAZWA TAGA NP. JESLI DB666.TAG0 TO TEXTBOX DO KTOREGO PRZYPISUJEMY WARTOSC MA NAZWAE TAG0
        //NAZWY TAGOW NIE MOGA SIE POWTARZAC!
        App App = new App();
        UserAdministration Users = new UserAdministration();
        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
            //Services do dependency injection
            var services = new ServiceCollection();
            services.AddSingleton<iApp, App>();
            services.AddSingleton<iCSVReader, CSVReader>();
            //Na podstawie service providera wyciagamy sobie implementacje Interfejsu
            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<iApp>();
            Users.SaveToXML();
            Users.Run();
           // App.RunInitPLC();
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");
            timer1.Enabled = true;
            listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(App.listBox1_DrawItem);

        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;
        //Metoda do update'u obiektow z poziomu innej klasy np. label5
        public void update(string message)
        {
           // label5.Text = message;
        }
        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(Tag000.Checked.ToString(), Tag000.Tag.ToString());
            App.WriteToDB(Tag111.Checked.ToString(), Tag111.Tag.ToString());
            App.WriteToDB(Tag888.Checked.ToString(), Tag888.Tag.ToString());
            App.WriteToDB(Tag999.Checked.ToString(), Tag999.Tag.ToString());
            App.WriteToDB(Tag100.Checked.ToString(), Tag100.Tag.ToString());
            App.WriteToDB(Tag1111.Checked.ToString(), Tag1111.Tag.ToString());
            App.WriteToDB(Tag122.Checked.ToString(), Tag122.Tag.ToString());
            App.WriteToDB(Tag133.Checked.ToString(), Tag133.Tag.ToString());
            App.WriteToDB(Tag222.Text, Tag222.Tag.ToString());
            App.WriteToDB(Tag444.Text, Tag444.Tag.ToString());
            App.WriteToDB(Tag666.Text, Tag666.Tag.ToString());

        }

        //Timer co 100ms do oczytywania DBka
        private void timer1_Tick(object sender, EventArgs e)
        {
            //App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

            this.Text = DateTime.Now.ToString();
            label57.Text = this.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Ikonka TextBoxa moze sluzyc jako wskaznik IO's na zasadzie kolorowania jej backcoloru i enabled dajesz jako false
            Input2.BackColor = Color.LightGreen;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "alarm";
            listAlarmView.Items.Add(item);
            int index = listAlarmView.Items.IndexOf(item);
        }

        private void button10_Click(object sender, EventArgs e)
        {
           // listAlarmView.Items.RemoveAt(0);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            App.WriteToDB("15", button4.Tag.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            App.WriteToDB("11", button5.Tag.ToString());
        }





        //OBCZAIC DELEGATY I ZDARZENIA 
        //Na samym koncu zajac sie swoimi kontrolkami jak juz bede wiedzial jak stricte maja wygladac i co od nich oczekuje
    }
}
