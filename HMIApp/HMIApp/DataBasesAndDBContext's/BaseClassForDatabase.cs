
using HMIApp.Components;

namespace HMIApp.DataBasesAndDBContext_s
{
    public class BaseClassForDatabase
    {
        public BaseClassForDatabase()
        {

        }
        public string ConnectionString { get; set; }

        public Logger _logger = new();

        //Odczyt pliku konfiguracyjnego z connection stringiem
        public string ReadConfFile(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                ConnectionString = System.IO.File.ReadAllText(filepath);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie mozna otworzyć pliku konfiguracyjnego bazy danych");
               _logger.LogMessage("Nie mozna otworzyc pliku konfiguracyjnego bazy danych");
            }
            return ConnectionString;
        }

    }
}
