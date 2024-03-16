using HMIApp.Components.DataBase;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace HMIApp.Data
{
    public class DataBase : iDataBase
    {
        public string ConnectionString = "";
        private readonly HMIAppDBContext _hmiAppDbContext;

        //konstruktor do wstrzykiwania DBContext
        public DataBase(HMIAppDBContext hmiAppDbContext)
        {
            _hmiAppDbContext = hmiAppDbContext;
            //w momencie wywolania konstruktora sprawdzamy czy nasza baza danych jest stworzona - jesli nie jest stworzona to ponizsza metoda ja utworzy
            _hmiAppDbContext.Database.EnsureCreated();
        }
        //konstruktor bezparametrowy
        public DataBase()
        {
            
        }

        //Odczyt pliku konfiguracyjnego z connection stringiem
        public string ReadConfFile(string filepath)
        {
            if (System.IO.File.Exists(filepath)) 
            {
                ConnectionString = System.IO.File.ReadAllText(filepath);
            }
            else
            {
                MessageBox.Show("Nie mozna otworzyć pliku konfiguracyjnego bazy danych");
            }
            return ConnectionString;
        }

        public void Run()
        {
            ReadConfFile("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\DataBaseConfiguration.txt");
        }

        //Metoda inserta do bazy
        public void SaveToDataBase()
        {
            //Zapis do bazy danych - narazie testowo
            _hmiAppDbContext.References.Add(new Reference()
            {
                ReferenceNumber = "123",
                NameOfClient = "Klient",
                ParameterCyklC1 = true,
                ParameterP1 = 3.5f,
                ForceF1 = 10000
            });

            _hmiAppDbContext.SaveChanges();
        }

        //metoda selecta z bazy
        public void ReadFromDataBase()
        {
            //odczyt tez narazie testowo
            //odczyt z bazy bedzie dopiero jak damy metode ToList()!
            var ReferencesFromDataBase = _hmiAppDbContext.References.ToList();
            foreach(var item in ReferencesFromDataBase) 
            {
                MessageBox.Show($"{item.ReferenceNumber}, {item.NameOfClient}, {item.ParameterCyklC1}, {item.ParameterP1}, {item.ForceF1}");
            }
        }
    }
}
