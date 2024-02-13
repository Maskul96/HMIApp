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

        }

        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(textBox1.Text);
        }




        //OBCZAIC DELEGATY I ZDARZENIA 
    }
}
