using HMIApp.Components;
using HMIApp.Components.CSVReader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HMIApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Ponizszy kod przerzucomy do Form1.cs
                ////Services do dependency injection
                //var services = new ServiceCollection();
                //services.AddSingleton<iApp,App>();
                //services.AddSingleton<iCSVReader, CSVReader>();
                ////Na podstawie service providera wyciagamy sobie implementacje Interfejsu
                //var serviceProvider = services.BuildServiceProvider();
                //var app = serviceProvider.GetService<iApp>();
            
        }
    }
}
