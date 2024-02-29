using System.Collections.Generic;
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
            EditUserFromXML(document);

        }
        public void SaveToXML()
        {
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

            EditUserFromXML(document);
        }

        public XDocument LoadFromXML(string filepath)
        {
            return XDocument.Load(filepath);
        }

        public void EditUserFromXML(XDocument document)
        {
            var names = document.Element("Użytkownicy")? //jeżeli on istnieje to wyciagnij elementy z nazwa Uzytkownik "?" oznacza "czy istnieje"
            .Elements("Użytkownik")
            .Select(x => x.Attribute("Numer_karty")?.Value);
            foreach (var item in names)
            {
                Form1._Form1.comboBox2.Items.Add(item);
            }
            string test;
            if (Form1._Form1.comboBox2.SelectedIndex == 1)
            {
                Form1._Form1.textBox16.Text = Form1._Form1.comboBox2.SelectedItem.ToString();
                test = Form1._Form1.textBox16.Text;
            }

            //var namescd = document.Element("Użytkownicy")? //jeżeli on istnieje to wyciagnij elementy z nazwa Uzytkownik "?" oznacza "czy istnieje"
            //.Elements("Użytkownik")
            //.Where(x => x.Attribute("Nazwa_użytkownika")?.Value == test)
            //.Select(x => x.Attribute("Nazwa_użytkownika")?.Value);

        }
    }
}
