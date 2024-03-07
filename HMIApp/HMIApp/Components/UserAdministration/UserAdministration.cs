using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HMIApp.Components.UserAdministration
{
    public class UserAdministration
    {
        public UserAdministration()
        {

        }
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
        public bool UserIsLoggedIn;
        public int Interval = 10000/1000;

        public void Run()
        {
            XDocument document = LoadFromXML("document.xml");
            DisplayValuesFromXML(document);
            FindUserinXML();
        }

        public void ClearUserFromDisplay()
        {
            Form1._Form1.label60.Text = "";
            Form1._Form1.label61.Text = "";
            Form1._Form1.label62.Text = "";
            UserIsLoggedIn = false;
        }

        public void FindUserinXML()
        {
            //Załaduj plik przed sprawdzeniem czy jest uzytkownik w nim
            XDocument document = LoadFromXML("document.xml");

            //Dane wejsciowe 
            NrofCard = Form1._Form1.textBox6.Text;

            if (NrofCard != "")
            {
                var names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Numer_karty")?.Value == NrofCard)
                .Single();

                UserIsLoggedIn = (names != null) ? true : false;

                Form1._Form1.label60.Text = names.Attribute("Numer_karty").Value;
                Form1._Form1.label61.Text = names.Attribute("Nazwa_użytkownika").Value;
                switch (names.Attribute("Uprawnienia").Value)
                {
                    case "0":
                        Form1._Form1.label62.Text = "Operator";
                        break;
                    case "1":
                        Form1._Form1.label62.Text = "Lider";
                        break;
                    case "2":
                        Form1._Form1.label62.Text = "Technolog";
                        break;
                    default:
                        Form1._Form1.label62.Text = "Operator";
                        break;
                }
                //Obsluga odliczania czasu do wylogowania
                Form1._Form1.timer3.Enabled = true;
                Form1._Form1.label63.Text = Interval.ToString();
                Form1._Form1.timer4.Enabled = true;
            }
            else
            {
                ClearUserFromDisplay();
            }
        }
        public void SaveToXML()
        {
            //ZROBIC ZABEZPIECZENIE ZE NIE MOZESZ DODACC UZYTKOWNIKA BEZ NAZWY I BEZ NR KARTY !
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("document.xml");

            id = document.Element("Użytkownicy").Elements("Użytkownik").Count();
            id += 1;
            NrofCard = Form1._Form1.textBox11.Text;
            Name = Form1._Form1.textBox12.Text;
            UserRights = Form1._Form1.comboBox3.SelectedIndex.ToString();
            if (NrofCard != "" && Name != "" && UserRights != "")
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
                Form1._Form1.listBox2.Items.Add("Użytkownik dodany");

                //Wyczyszczenie i nastepnie update listy w combobox
                ClearListInComboBox();
                DisplayValuesFromXML(document);
            }
            else
            {
                //Komunikat dla usera
                Form1._Form1.listBox2.Items.Add("Niepoprawne dane");
            }

            Form1._Form1.timer2.Enabled = true;

        }

        public void EditXML()
        {
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("document.xml");

            //Dane wejsciowe do edycji
            NrofCard = Form1._Form1.textBox16.Text;
            Name = Form1._Form1.textBox15.Text;
            UserRights = Form1._Form1.comboBox4.SelectedIndex.ToString();
            int.TryParse(Form1._Form1.textBox1.Text, out id);
            if (NrofCard != "" && Name != "" && UserRights != "")
            {
                //Edytowanie danych użytkownika
                var names = document.Element("Użytkownicy")?
            .Elements("Użytkownik")
            .Where(x => x.Attribute("ID")?.Value == id.ToString())
            .Single();
                names.Attribute("Numer_karty").Value = Form1._Form1.textBox16.Text;
                names.Attribute("Nazwa_użytkownika").Value = Form1._Form1.textBox15.Text;
                names.Attribute("Uprawnienia").Value = Form1._Form1.comboBox4.SelectedIndex.ToString();

                //Zapis ppliku
                document.Save("document.xml");

                //Komunikat dla usera
                Form1._Form1.listBox2.Items.Add("Użytkownik zedytowany");

                //Wyczyszczenie i nastepnie update listy w combobox
                ClearListInComboBox();
                DisplayValuesFromXML(document);
            }
            else
            {
                //Komunikat dla usera
                Form1._Form1.listBox2.Items.Add("Niepoprawne dane");
            }
            Form1._Form1.timer2.Enabled = true;

        }

        public XDocument LoadFromXML(string filepath)
        {
            return XDocument.Load(filepath);
        }

        //kasowanie listy celem pozniejszego update'u
        public void ClearListInComboBox()
        {
            Form1._Form1.comboBox2.Items.Clear();
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
                        Form1._Form1.comboBox2.Items.Add(item);
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
                    Form1._Form1.textBox1.Text = item;
                }

                names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Nazwa_użytkownika")?.Value == Name)
                .Select(x => x.Attribute("Numer_karty")?.Value);
                foreach (var item in names)
                {
                    Form1._Form1.textBox16.Text = item;
                }

                names = document.Element("Użytkownicy")?
                .Elements("Użytkownik")
                .Where(x => x.Attribute("Nazwa_użytkownika")?.Value == Name)
                .Select(x => x.Attribute("Uprawnienia")?.Value);
                foreach (var item in names)
                {
                    int test = Convert.ToInt16(item);
                    Form1._Form1.comboBox4.SelectedIndex = test;
                }
            }
        }
    }
}
