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
        //Konstruktor bezparametrowy do wywolywania funkcji z klasy App do Zapisu danych
        //Nie dziala prawidlowo - zapis dziala tylko dla konstruktora wywolanego w Form1
        //App App = new App();
        //Zapis i odczyt trzeba bedzie przerzucic raczej tutaj
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
            app.RunInitPLC();
            app.RunReadFromCSVFileandReadFromDB();
            app.WriteToDB("Bool", 0, 1,7);

        }

        //OBCZAIC DELEGATY I ZDARZENIA 
    }
}
