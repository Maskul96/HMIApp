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

namespace HMIApp
{
    public partial class Form1 : Form
    {
        //Referencje przechowywane w PLC - w pozniejszym etapie zrobic przechowywanie w bazie danych
        //Obiekty z Form1 Design ustawione z dostepem jako public zeby mozna bylo miec do nich dostep z innej klasy poprzez konstruktor
        //Ogarniete odczytywanie/zapisywanie z PLC - teraz je przetestowac i potem ogarnac wlasne ikony

        App App = new App();
       
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
            // app.RunInitPLC();
            App.RunInitPLC();
            App.ReadFromDB();
            timer1.Enabled = true;

        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;
        //Metoda do update'u obiektow z poziomu innej klasy np. label5
        public void update(string message)
        {
            label5.Text = message;
        }
        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox1.Text, "DB667.Tag0");



        }

        private void button2_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox17.Text, "DB667.Tag1");

        }

        //Timer co 100ms do oczytywania DBka
        private void timer1_Tick(object sender, EventArgs e)
        {
            App.ReadFromDB();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox16.Text, "DB667.Tag8");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox18.Text, "DB667.Tag9");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox19.Text, "DB667.Tag10");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox20.Text, "DB667.Tag11");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox22.Text, "DB667.Tag13");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox21.Text, "DB667.Tag12");
        }






        //OBCZAIC DELEGATY I ZDARZENIA 
        //Na samym koncu zajac sie swoimi kontrolkami jak juz bede wiedzial jak stricte maja wygladac i co od nich oczekuje
    }
}
