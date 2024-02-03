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
        //Stworzenie obiektu z konfiguracja sterownika
        SiemensPLC PLC = new SiemensPLC("192.168.2.1", 102, 0, 1, 1000000);
        //Teraz zrobic odczytywanie danych z excela na wzor DBkow Siemens
        byte[] result = new byte[7];



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
            app.Run();

            // PLC.Init();

        }
        //OBCZAIC DELEGATY I ZDARZENIA NJAPIERW!
        //Odczytywanie z pliku mamy - teraz rekordy w pliku powrzucac do metody PLC.Read i przetestować odczyt
        //I te same rekordy uzyć do wskazan miejsc do zapisu

        //metody read i write
        private void button1_Click(object sender, EventArgs e)
        {
            //Read DB
            PLC.Read(666, 0, 7, result);
            //Read INT16
            var j = libnodave.getS16from(result, 0);
            //Read FLOAT
            var k = libnodave.getFloatfrom(result, 2);
            //Read BOOL
            var l = result[6];
            //textBox1.Text = $"INT: {j} ; FLOAT: {k} ; BOOL: {l}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Write DB
           // string temp2 = textBox2.Text;
            //Write Float
            //float fltValue = (float)Convert.ToDouble(temp2);
           // var i = libnodave.daveToPLCfloat(fltValue);
           // byte[] intBytes1 = BitConverter.GetBytes(i);
           // PLC.Write(666, 2, 4, intBytes1);
            //write INT
            //string temp3 = textBox3.Text;
           // short intValue = Convert.ToInt16(temp3);
            //byte[] intBytes = BitConverter.GetBytes(intValue);
           //Array.Reverse(intBytes);
            //PLC.Write(666, 0, 2, intBytes);
            //Write BOOL
            //string temp4 = textBox4.Text;
           // bool boolValue = Convert.ToBoolean(temp4);
           // byte[] intBytes2 = BitConverter.GetBytes(boolValue);
           // PLC.Write(666, 6, 1, intBytes2);
        }
    }
}
