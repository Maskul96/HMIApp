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
            var query = _hmiAppDbContext.References.Where(x => x.ReferenceNumber == Form1._Form1.DB666Tag16.Text).SingleOrDefault();
            if (query == null)
            {
                _hmiAppDbContext.References.Add(new Reference()
                {
                    ReferenceNumber = Form1._Form1.DB666Tag16.Text,
                    NameOfClient = Form1._Form1.DB666Tag17.Text,
                    ParameterCyklC1 = Form1._Form1.DB666Tag0.Checked,
                    ParameterCyklC2 = Form1._Form1.DB666Tag1.Checked,
                    ParameterCyklC3 = Form1._Form1.DB666Tag8.Checked,
                    ParameterCyklC4 = Form1._Form1.DB666Tag9.Checked,
                    ParameterCyklC5 = Form1._Form1.DB666Tag10.Checked,
                    ParameterCyklC6 = Form1._Form1.DB666Tag11.Checked,
                    ParameterCyklC7 = Form1._Form1.DB666Tag12.Checked,
                    ParameterCyklC8 = Form1._Form1.DB666Tag13.Checked,
                    ParameterP1 = float.Parse(Form1._Form1.DB666Tag2.Text),
                    ParameterP2 = float.Parse(Form1._Form1.DB666Tag3.Text),
                    ParameterP3 = float.Parse(Form1._Form1.DB666Tag4.Text),
                    ParameterP4 = float.Parse(Form1._Form1.DB666Tag5.Text),
                    ParameterP5 = float.Parse(Form1._Form1.DB666Tag6.Text),
                    ParameterP6 = float.Parse(Form1._Form1.DB666Tag15.Text),
                    ParameterP7 = float.Parse(Form1._Form1.DB666Tag7.Text),
                    ParameterP8 = float.Parse(Form1._Form1.DB666Tag14.Text),
                    ForceF1 = int.Parse(Form1._Form1.DB666Tag18.Text),
                    ForceF2 = int.Parse(Form1._Form1.DB666Tag19.Text),
                    ForceF3 = int.Parse(Form1._Form1.DB666Tag20.Text),
                    ForceF4 = int.Parse(Form1._Form1.DB666Tag21.Text),
                    ForceF5 = int.Parse(Form1._Form1.DB666Tag22.Text)
                });

                _hmiAppDbContext.SaveChanges();
                ClearCombobox();
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
            if(referencenumber == "")
            {
                MessageBox.Show("Nie podano referencji");
            }
            if (referencenumber != "")
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
                    Form1._Form1.DB666Tag16.Text = item.ReferenceNumber;
                    Form1._Form1.DB666Tag17.Text = item.NameOfClient;
                    Form1._Form1.DB666Tag0.Checked = item.ParameterCyklC1;
                    Form1._Form1.DB666Tag1.Checked = item.ParameterCyklC2;
                    Form1._Form1.DB666Tag8.Checked = item.ParameterCyklC3;
                    Form1._Form1.DB666Tag9.Checked = item.ParameterCyklC4;
                    Form1._Form1.DB666Tag10.Checked = item.ParameterCyklC5;
                    Form1._Form1.DB666Tag11.Checked = item.ParameterCyklC6;
                    Form1._Form1.DB666Tag12.Checked = item.ParameterCyklC7;
                    Form1._Form1.DB666Tag13.Checked = item.ParameterCyklC8;
                    Form1._Form1.DB666Tag2.Text = item.ParameterP1.ToString();
                    Form1._Form1.DB666Tag3.Text = item.ParameterP2.ToString();
                    Form1._Form1.DB666Tag4.Text = item.ParameterP3.ToString();
                    Form1._Form1.DB666Tag5.Text = item.ParameterP4.ToString();
                    Form1._Form1.DB666Tag6.Text = item.ParameterP5.ToString();
                    Form1._Form1.DB666Tag15.Text = item.ParameterP6.ToString();
                    Form1._Form1.DB666Tag7.Text = item.ParameterP7.ToString();
                    Form1._Form1.DB666Tag14.Text = item.ParameterP8.ToString();
                    Form1._Form1.DB666Tag18.Text = item.ForceF1.ToString();
                    Form1._Form1.DB666Tag19.Text = item.ForceF2.ToString();
                    Form1._Form1.DB666Tag20.Text = item.ForceF3.ToString();
                    Form1._Form1.DB666Tag21.Text = item.ForceF4.ToString();
                    Form1._Form1.DB666Tag22.Text = item.ForceF5.ToString();

                }
            }
        }

        //Metoda do wyswietlenia referencji w combobox5
        public void SelectFromDbToComboBox()
        {
            var References = _hmiAppDbContext.References.ToList();
            foreach(var Reference in References)
            {
                if (!Form1._Form1.comboBoxListaReferencji.Items.Contains(Reference.ReferenceNumber))
                {
                    Form1._Form1.comboBoxListaReferencji.Items.Add(Reference.ReferenceNumber);
                }
            }
        }

        public void ClearCombobox()
        {
            Form1._Form1.comboBoxListaReferencji.Items.Clear();
            Form1._Form1.comboBoxListaReferencji.Text = "";
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
            bool blockade = false;
            //    ////Update danych cd
            var ref1 = ReadFirst(referencenumber);
            if(referencenumber == Form1._Form1.DB666Tag16.Text)
            {
                blockade = true;
            }
            ref1.ReferenceNumber = Form1._Form1.DB666Tag16.Text;
            ref1.NameOfClient = Form1._Form1.DB666Tag17.Text;
            ref1.ParameterCyklC1 = Form1._Form1.DB666Tag0.Checked;
            ref1.ParameterCyklC2 = Form1._Form1.DB666Tag1.Checked;
            ref1.ParameterCyklC3 = Form1._Form1.DB666Tag8.Checked;
            ref1.ParameterCyklC4 = Form1._Form1.DB666Tag9.Checked;
            ref1.ParameterCyklC5 = Form1._Form1.DB666Tag10.Checked;
            ref1.ParameterCyklC6 = Form1._Form1.DB666Tag11.Checked;
            ref1.ParameterCyklC7 = Form1._Form1.DB666Tag12.Checked;
            ref1.ParameterCyklC8 = Form1._Form1.DB666Tag13.Checked;
            ref1.ParameterP1 = float.Parse(Form1._Form1.DB666Tag2.Text);
            ref1.ParameterP2 = float.Parse(Form1._Form1.DB666Tag3.Text);
            ref1.ParameterP3 = float.Parse(Form1._Form1.DB666Tag4.Text);
            ref1.ParameterP4 = float.Parse(Form1._Form1.DB666Tag5.Text);
            ref1.ParameterP5 = float.Parse(Form1._Form1.DB666Tag6.Text);
            ref1.ParameterP6 = float.Parse(Form1._Form1.DB666Tag15.Text);
            ref1.ParameterP7 = float.Parse(Form1._Form1.DB666Tag7.Text);
            ref1.ParameterP8 = float.Parse(Form1._Form1.DB666Tag14.Text);
            ref1.ForceF1 = int.Parse(Form1._Form1.DB666Tag18.Text);
            ref1.ForceF2 = int.Parse(Form1._Form1.DB666Tag19.Text);
            ref1.ForceF3 = int.Parse(Form1._Form1.DB666Tag20.Text);
            ref1.ForceF4 = int.Parse(Form1._Form1.DB666Tag21.Text);
            ref1.ForceF5 = int.Parse(Form1._Form1.DB666Tag22.Text);
            _hmiAppDbContext.SaveChanges();
            //Dorobić taką opcję, że czyscimy comboboxa tylko wtedy kiedy zmienimy samą nazwe
            if(blockade == false)
            {
                ClearCombobox();
            }
            SelectFromDbToComboBox();
            blockade = false;
        }

        public void Delete(string referencenumber)
        {

            //    ////usuwanie danych
            if (referencenumber != "")
            {
                var ref2 = ReadFirst(referencenumber);
                _hmiAppDbContext.References.Remove(ref2);
                _hmiAppDbContext.SaveChanges();
            }
            ClearCombobox();
            SelectFromDbToComboBox();
        }
    }
}
