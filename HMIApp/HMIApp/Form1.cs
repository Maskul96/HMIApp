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
            app.RunInitPLC();
            app.RunReadFromCSVFileandReadFromDB();   
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Zapis do DBka
            App.WriteToDB(textBox1.Text, 0, 2);
        }
        //OBCZAIC DELEGATY I ZDARZENIA 
    }
}
