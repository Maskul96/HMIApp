using HMIApp.Components;
using HMIApp.Components.CSVReader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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

            if (IsLicensed())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("HMIApp is not licensed. Please contact your IT department.");
            }
        }

        static bool IsLicensed()
        {
            //Zebranie adresu Mac
            String firstMacAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
            string LicenceKey = "";
            string LicenceKey1 = "";
            if(File.Exists("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\LicenceKey.txt"))
            {
                LicenceKey = File.ReadAllText("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\LicenceKey.txt");
                if(File.Exists("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\LicenceKey1.txt"))
                {
                    LicenceKey1 = File.ReadAllText("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\LicenceKey1.txt");
                    if(LicenceKey == LicenceKey1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
