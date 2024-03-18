using HMIApp.Components.DataBase;
using Mono.TextTemplating;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace HMIApp.Data
{
    public class DataBase : iDataBase
    {
        public string ConnectionString = "";
        private readonly HMIAppDBContext _hmiAppDbContext;
        public Form1 obj;

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
        //Ponizej konstruktor z parametrem obj do przekazywania obiektow z Form1 do wnętrza klasy
        public DataBase(Form1 obj)
        {
            this.obj = obj;
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
        public void InsertToDataBase()
        {
            var query = _hmiAppDbContext.References.Where(x => x.ReferenceNumber == Form1._Form1.DB666Tag16PassedValue.Text).SingleOrDefault();
            if (query == null)
            {
                _hmiAppDbContext.References.Add(new Reference()
                {
                    ReferenceNumber = Form1._Form1.DB666Tag16PassedValue.Text,
                    NameOfClient = Form1._Form1.DB666Tag17PassedValue.Text,
                    ParameterCyklC1 = Form1._Form1.DB666Tag0PassedValue.Checked,
                    ParameterCyklC2 = Form1._Form1.DB666Tag1PassedValue.Checked,
                    ParameterCyklC3 = Form1._Form1.DB666Tag8PassedValue.Checked,
                    ParameterCyklC4 = Form1._Form1.DB666Tag9PassedValue.Checked,
                    ParameterCyklC5 = Form1._Form1.DB666Tag10PassedValue.Checked,
                    ParameterCyklC6 = Form1._Form1.DB666Tag11PassedValue.Checked,
                    ParameterCyklC7 = Form1._Form1.DB666Tag12PassedValue.Checked,
                    ParameterCyklC8 = Form1._Form1.DB666Tag13PassedValue.Checked,
                    ParameterP1 = float.Parse(Form1._Form1.DB666Tag2PassedValue.Text),
                    ParameterP2 = float.Parse(Form1._Form1.DB666Tag3PassedValue.Text),
                    ParameterP3 = float.Parse(Form1._Form1.DB666Tag4PassedValue.Text),
                    ParameterP4 = float.Parse(Form1._Form1.DB666Tag5PassedValue.Text),
                    ParameterP5 = float.Parse(Form1._Form1.DB666Tag6PassedValue.Text),
                    ParameterP6 = float.Parse(Form1._Form1.DB666Tag15PassedValue.Text),
                    ParameterP7 = float.Parse(Form1._Form1.DB666Tag7PassedValue.Text),
                    ParameterP8 = float.Parse(Form1._Form1.DB666Tag14PassedValue.Text),
                    ForceF1 = int.Parse(Form1._Form1.DB666Tag18PassedValue.Text),
                    ForceF2 = int.Parse(Form1._Form1.DB666Tag19PassedValue.Text),
                    ForceF3 = int.Parse(Form1._Form1.DB666Tag20PassedValue.Text),
                    ForceF4 = int.Parse(Form1._Form1.DB666Tag21PassedValue.Text),
                    ForceF5 = int.Parse(Form1._Form1.DB666Tag22PassedValue.Text)
                });

                _hmiAppDbContext.SaveChanges();
                SelectFromDbToComboBox();
            }
            else
            {
                MessageBox.Show("Istnieje juz taka referencja");
            }
        }

        //metoda select z uzyciem LINQ
        public void SelectFromDataBase(string referencenumber)
        {

            var query = _hmiAppDbContext.References.Where(x => x.ReferenceNumber == referencenumber)
                .Select(x => new
                {
                    x.ReferenceNumber,
                    x.NameOfClient,
                    x.ParameterCyklC1,
                    x.ParameterCyklC2,
                    x.ParameterCyklC3,
                    x.ParameterCyklC4,
                    x.ParameterCyklC5,
                    x.ParameterCyklC6,
                    x.ParameterCyklC7,
                    x.ParameterCyklC8,
                    x.ParameterP1,
                    x.ParameterP2,
                    x.ParameterP3,
                    x.ParameterP4,
                    x.ParameterP5,
                    x.ParameterP6,
                    x.ParameterP7,
                    x.ParameterP8,
                    x.ForceF1,
                    x.ForceF2,
                    x.ForceF3,
                    x.ForceF4,
                    x.ForceF5
                     })
                    .ToList();

            foreach (var item in query)
            {
                Form1._Form1.DB666Tag16PassedValue.Text = item.ReferenceNumber;
                Form1._Form1.DB666Tag17PassedValue.Text = item.NameOfClient;
                Form1._Form1.DB666Tag0PassedValue.Checked = item.ParameterCyklC1;
                Form1._Form1.DB666Tag1PassedValue.Checked = item.ParameterCyklC2;
                Form1._Form1.DB666Tag8PassedValue.Checked = item.ParameterCyklC3;
                Form1._Form1.DB666Tag9PassedValue.Checked = item.ParameterCyklC4;
                Form1._Form1.DB666Tag10PassedValue.Checked = item.ParameterCyklC5;
                Form1._Form1.DB666Tag11PassedValue.Checked = item.ParameterCyklC6;
                Form1._Form1.DB666Tag12PassedValue.Checked = item.ParameterCyklC7;
                Form1._Form1.DB666Tag13PassedValue.Checked = item.ParameterCyklC8;
                Form1._Form1.DB666Tag2PassedValue.Text = item.ParameterP1.ToString();
                Form1._Form1.DB666Tag3PassedValue.Text = item.ParameterP2.ToString();
                Form1._Form1.DB666Tag4PassedValue.Text = item.ParameterP3.ToString();
                Form1._Form1.DB666Tag5PassedValue.Text = item.ParameterP4.ToString();
                Form1._Form1.DB666Tag6PassedValue.Text = item.ParameterP5.ToString();
                Form1._Form1.DB666Tag15PassedValue.Text = item.ParameterP6.ToString();
                Form1._Form1.DB666Tag7PassedValue.Text = item.ParameterP7.ToString();
                Form1._Form1.DB666Tag14PassedValue.Text = item.ParameterP8.ToString();
                Form1._Form1.DB666Tag18PassedValue.Text = item.ForceF1.ToString();
                Form1._Form1.DB666Tag19PassedValue.Text = item.ForceF2.ToString();
                Form1._Form1.DB666Tag20PassedValue.Text = item.ForceF3.ToString();
                Form1._Form1.DB666Tag21PassedValue.Text = item.ForceF4.ToString();
                Form1._Form1.DB666Tag22PassedValue.Text = item.ForceF5.ToString();

            }
        }

        //Metoda do wyswietlenia referencji w combobox5
        public void SelectFromDbToComboBox()
        {
            var References = _hmiAppDbContext.References.ToList();
            foreach(var Reference in References)
            {
                if (!Form1._Form1.comboBox5.Items.Contains(Reference.ReferenceNumber))
                {
                    Form1._Form1.comboBox5.Items.Add(Reference.ReferenceNumber);
                }
            }
        }
        //Update danych "?" zezwala na null - operator warunkowego dostepu
        //Metoda do zwrocenia wyszukiwanego przez nas pola w bazie
#nullable enable
        public Reference? ReadFirst(string referencenumber)
        {
            return _hmiAppDbContext.References.FirstOrDefault(x => x.ReferenceNumber == referencenumber);
#nullable disable
        }

        public void UpdateDb(string referencenumber)
        {
            //    ////Update danych cd
            var ref1 = ReadFirst(referencenumber);
            ref1.ReferenceNumber = Form1._Form1.DB666Tag16PassedValue.Text;
            ref1.NameOfClient = Form1._Form1.DB666Tag17PassedValue.Text;
            ref1.ParameterCyklC1 = Form1._Form1.DB666Tag0PassedValue.Checked;
            ref1.ParameterCyklC2 = Form1._Form1.DB666Tag1PassedValue.Checked;
            ref1.ParameterCyklC3 = Form1._Form1.DB666Tag8PassedValue.Checked;
            ref1.ParameterCyklC4 = Form1._Form1.DB666Tag9PassedValue.Checked;
            ref1.ParameterCyklC5 = Form1._Form1.DB666Tag10PassedValue.Checked;
            ref1.ParameterCyklC6 = Form1._Form1.DB666Tag11PassedValue.Checked;
            ref1.ParameterCyklC7 = Form1._Form1.DB666Tag12PassedValue.Checked;
            ref1.ParameterCyklC8 = Form1._Form1.DB666Tag13PassedValue.Checked;
            ref1.ParameterP1 = float.Parse(Form1._Form1.DB666Tag2PassedValue.Text);
            ref1.ParameterP2 = float.Parse(Form1._Form1.DB666Tag3PassedValue.Text);
            ref1.ParameterP3 = float.Parse(Form1._Form1.DB666Tag4PassedValue.Text);
            ref1.ParameterP4 = float.Parse(Form1._Form1.DB666Tag5PassedValue.Text);
            ref1.ParameterP5 = float.Parse(Form1._Form1.DB666Tag6PassedValue.Text);
            ref1.ParameterP6 = float.Parse(Form1._Form1.DB666Tag15PassedValue.Text);
            ref1.ParameterP7 = float.Parse(Form1._Form1.DB666Tag7PassedValue.Text);
            ref1.ParameterP8 = float.Parse(Form1._Form1.DB666Tag14PassedValue.Text);
            ref1.ForceF1 = int.Parse(Form1._Form1.DB666Tag18PassedValue.Text);
            ref1.ForceF2 = int.Parse(Form1._Form1.DB666Tag19PassedValue.Text);
            ref1.ForceF3 = int.Parse(Form1._Form1.DB666Tag20PassedValue.Text);
            ref1.ForceF4 = int.Parse(Form1._Form1.DB666Tag21PassedValue.Text);
            ref1.ForceF5 = int.Parse(Form1._Form1.DB666Tag22PassedValue.Text);
            _hmiAppDbContext.SaveChanges();
        }

        public void Delete(string referencenumber)
        {

            //    ////usuwanie danych
            var ref2 = ReadFirst(referencenumber);
            _hmiAppDbContext.References.Remove(ref2);
            _hmiAppDbContext.SaveChanges();
        }
    }
}
