using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HMIApp.Components.UserAdministration
{
    public class UserAdministration : iUserAdministration
    {
        //konstruktor bezparametrowy
        public UserAdministration()
        {

        }
        //konstruktor z parametrem Form1 obj
        public UserAdministration(Form1 obj)
        {
            this.obj = obj;
        }
        //zmienne
        public Form1 obj;
        public string NrofCard;
        public string Name;
        public string UserRights;
        public int id=1;
       // public bool UserIsLoggedIn;
        public int Interval = 100000/1000;

        public bool UserIsLoggedIn { get; set; }

        public void Run()
        {
            XDocument document = LoadFromXML("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\document.xml");
            DisplayValuesFromXML(document);
            FindUserinXML();
        }

        public void EnabledObjects()
        {
            if ( UserIsLoggedIn )
            {
                Form1._Form1.buttonUsunRef.Enabled = true;
                Form1._Form1.button_ZamknijApke.Enabled = true;
                Form1._Form1.buttonDodajUzytkownika.Enabled = true;
                Form1._Form1.buttonEdytujUzytkownika.Enabled = true;
                Form1._Form1.buttonDodajNowaRef.Enabled = true;
                Form1._Form1.buttonWczytajRef.Enabled = true;
            }
            else
            {
                Form1._Form1.buttonUsunRef.Enabled = false;
                Form1._Form1.button_ZamknijApke.Enabled = false;
                Form1._Form1.buttonDodajUzytkownika.Enabled = false;
                Form1._Form1.buttonEdytujUzytkownika.Enabled = false;
                Form1._Form1.buttonDodajNowaRef.Enabled = false;
                Form1._Form1.buttonWczytajRef.Enabled = false;
            }

        }

        public void ClearUserFromDisplay()
        {
            Form1._Form1.label_Imie.Text = "";
            Form1._Form1.label_NrKarty.Text = "";
            Form1._Form1.label_Uprawnienia.Text = "";
            UserIsLoggedIn = false;
        }

        public void FindUserinXML()
        {
            //Załaduj plik przed sprawdzeniem czy jest uzytkownik w nim
            XDocument document = LoadFromXML("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\document.xml");

            //Dane wejsciowe 
            NrofCard = Form1._Form1.textBox_MiejsceNaNrKarty_Zaloguj.Text;

            if (NrofCard != "")
            {
                try
                {
                    var names = document.Element("Użytkownicy")?
                    .Elements("Użytkownik")
                    .Where(x => x.Attribute("Numer_karty")?.Value == NrofCard)
                    .Single();

                    UserIsLoggedIn = (names != null) ? true : false;

                    Form1._Form1.label_Imie.Text = names.Attribute("Numer_karty").Value;
                    Form1._Form1.label_NrKarty.Text = names.Attribute("Nazwa_użytkownika").Value;
                    switch (names.Attribute("Uprawnienia").Value)
                    {
                        case "0":
                            Form1._Form1.label_Uprawnienia.Text = "Operator";
                            break;
                        case "1":
                            Form1._Form1.label_Uprawnienia.Text = "Lider";
                            break;
                        case "2":
                            Form1._Form1.label_Uprawnienia.Text = "Technolog";
                            break;
                        default:
                            Form1._Form1.label_Uprawnienia.Text = "Operator";
                            break;
                    }
                    Form1._Form1.StatusyLogowania.Text = "Użytkownik zalogowany";
                    //Obsluga odliczania czasu do wylogowania
                    Form1._Form1.TimeoutWylogowania.Enabled = true;
                    Form1._Form1.label13.Text = Interval.ToString();
                    Form1._Form1.OdliczaSekunde.Enabled = true;
                }
                catch (Exception e)
                {
                    Form1._Form1.StatusyLogowania.Text = "Nie znaleziono takiego użytkownika";                    
                }
            }
            else
            {
                ClearUserFromDisplay();
            }
        }
        public void SaveToXML()
        {
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\document.xml");

            id = document.Element("Użytkownicy").Elements("Użytkownik").Count();
            id += 1;
            NrofCard = Form1._Form1.textbox_NumerKarty_DodajUzytk.Text;
            Name = Form1._Form1.textBoxImie_DodajUzytk.Text;
            UserRights = Form1._Form1.comboBox_ListaUprawnien_DodajUzytk.SelectedIndex.ToString();
            if (NrofCard != "" && Name != "" && UserRights != "")
            {
                //Zabezpieczenie przed dodanie usera o tym samym numerze karty
                var names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Numer_karty")?.Value == NrofCard)
                .Single();

                if (NrofCard != names.Attribute("Numer_karty").Value)
                {
                    document.Element("Użytkownicy").Add
                        (new XElement("Użytkownik",
                        new XAttribute("ID", id),
                        new XAttribute("Numer_karty", NrofCard),
                     new XAttribute("Nazwa_użytkownika", Name),
                     new XAttribute("Uprawnienia", UserRights)));
                    //Zapis ppliku
                    document.Save("document.xml");
                    //Komunikat dla usera
                    Form1._Form1.StatusyLogowania.Text = ("Użytkownik dodany");

                    //Wyczyszczenie i nastepnie update listy w combobox
                    ClearListInComboBox();
                    DisplayValuesFromXML(document);
                }
                else
                {
                    //Komunikat dla usera
                    Form1._Form1.StatusyLogowania.Text = ("Użytkownik już istnieje!");
                }
            }
            else
            {
                //Komunikat dla usera
                Form1._Form1.StatusyLogowania.Text = ("Niepoprawne dane");
            }

            Form1._Form1.CzyszczenieStatusówLogowania.Enabled = true;

        }

        public void EditXML()
        {
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\document.xml");

            //Dane wejsciowe do edycji
            NrofCard = Form1._Form1.textBox_NumerKarty_Edycja.Text;
            Name = Form1._Form1.textBox_Imie_Edycja.Text;
            UserRights = Form1._Form1.comboBox_ListaUprawnien_Edycja.SelectedIndex.ToString();
            int.TryParse(Form1._Form1.textBox_ID_Edycja.Text, out id);
            if (NrofCard != "" && Name != "" && UserRights != "")
            {
                //Edytowanie danych użytkownika
                var names = document.Element("Użytkownicy")?
            .Elements("Użytkownik")
            .Where(x => x.Attribute("ID")?.Value == id.ToString())
            .Single();
                names.Attribute("Numer_karty").Value = Form1._Form1.textBox_NumerKarty_Edycja.Text;
                names.Attribute("Nazwa_użytkownika").Value = Form1._Form1.textBox_Imie_Edycja.Text;
                names.Attribute("Uprawnienia").Value = Form1._Form1.comboBox_ListaUprawnien_Edycja.SelectedIndex.ToString();

                //Zapis ppliku
                document.Save("document.xml");

                //Komunikat dla usera
                Form1._Form1.StatusyLogowania.Text = ("Użytkownik zedytowany");

                //Wyczyszczenie i nastepnie update listy w combobox
                ClearListInComboBox();
                DisplayValuesFromXML(document);
            }
            else
            {
                //Komunikat dla usera
                Form1._Form1.StatusyLogowania.Text = ("Niepoprawne dane");
            }
            Form1._Form1.CzyszczenieStatusówLogowania.Enabled = true;

        }

        public XDocument LoadFromXML(string filepath)
        {
            return XDocument.Load(filepath);
        }

        //kasowanie listy celem pozniejszego update'u
        public void ClearListInComboBox()
        {
            Form1._Form1.comboBox_ListaUzytkWBazie.Items.Clear();
        }

        //Metoda do wysietlania wybranego uzytkownika do edycji z parametrem opcjonalnym string
        public void DisplayValuesFromXML(XDocument document, string Name = "")
        {
            if (Name == "")
            {
                var names = document.Element("Użytkownicy")? //jeżeli on istnieje to wyciagnij elementy z nazwa Uzytkownik "?" oznacza "czy istnieje"
                .Elements("Użytkownik")
                .Select(x => x.Attribute("Nazwa_użytkownika")?.Value);
                foreach (var item in names)
                {
                        Form1._Form1.comboBox_ListaUzytkWBazie.Items.Add(item);
                }
            }
            if (Name != "")
            {
                var names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Nazwa_użytkownika")?.Value == Name)
                .Select(x => x.Attribute("ID")?.Value);
                foreach (var item in names)
                {
                    Form1._Form1.textBox_ID_Edycja.Text = item;
                }

                names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Nazwa_użytkownika")?.Value == Name)
                .Select(x => x.Attribute("Numer_karty")?.Value);
                foreach (var item in names)
                {
                    Form1._Form1.textBox_NumerKarty_Edycja.Text = item;
                }

                names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Nazwa_użytkownika")?.Value == Name)
                .Select(x => x.Attribute("Uprawnienia")?.Value);
                foreach (var item in names)
                {
                    int test = Convert.ToInt16(item);
                    Form1._Form1.comboBox_ListaUprawnien_Edycja.SelectedIndex = test;
                }
            }
        }

        public void LoadFromXML()
        {
            throw new NotImplementedException();
        }

        public void UpdateDisplayValuesFromXML()
        {
            throw new NotImplementedException();
        }

        public void ClearListinComboBox()
        {
            throw new NotImplementedException();
        }
    }
}
