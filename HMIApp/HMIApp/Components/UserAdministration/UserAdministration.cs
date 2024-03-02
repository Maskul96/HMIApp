using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
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
        

        public void Run()
        {
            XDocument document = LoadFromXML("document.xml");
           UpdateFromXML(document);

        }
        public void SaveToXML()
        {
            //ZROBIC ZABEZPIECZENIE ZE NIE MOZESZ DODACC UZYTKOWNIKA BEZ NAZWY I BEZ NR KARTY !
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("document.xml");

            NrofCard = Form1._Form1.textBox11.Text;
            Name = Form1._Form1.textBox12.Text;
            UserRights = Form1._Form1.comboBox3.SelectedIndex.ToString();

            document.Element("Użytkownicy").Add
                (new XElement("Użytkownik",
                new XAttribute("Numer_karty", NrofCard),
             new XAttribute("Nazwa_użytkownika", Name),
             new XAttribute("Uprawnienia", UserRights)));

            document.Save("document.xml");
            //Dodac event na dodanie uzytkownika do listy i swyswietlic jako komunikat
            Form1._Form1.listBox2.Items.Add("Użytkownik dodany");
            Form1._Form1.timer2.Enabled = true;

            UpdateFromXML(document);
        }

        public void EditXML()
        {
            //Załaduj plik przed dodaniem nowych danych i zapisaniem
            XDocument document = LoadFromXML("document.xml");

            NrofCard = Form1._Form1.textBox16.Text;
            Name = Form1._Form1.textBox15.Text;
            UserRights = Form1._Form1.comboBox4.SelectedIndex.ToString();


            document.Save("document.xml");
            //Dodac event na dodanie uzytkownika do listy i swyswietlic jako komunikat
            Form1._Form1.listBox2.Items.Add("Użytkownik zedytowany");
            Form1._Form1.timer2.Enabled = true;

            UpdateFromXML(document);
        }

        public XDocument LoadFromXML(string filepath)
        {
            return XDocument.Load(filepath);
        }

        //Metoda do wysietlania wybranego uzytkownika do edycji z parametrem opcjonalnym string
        public void UpdateFromXML(XDocument document, string Name = "")
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
