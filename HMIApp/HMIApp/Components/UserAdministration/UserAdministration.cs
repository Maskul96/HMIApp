using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace HMIApp.Components.UserAdministration
{
    public class UserAdministration
    {
        public UserAdministration()
        {
                
        }

        public string NrofCard = "12345";
        public string Name = "Jan Kowalski";


        public void Run()
        {
            LoadFromXML();
        }
        public void SaveToXML()
        {
            //Tworzymy dokumnet XML
            var document = new XDocument();
            //Tworzymy nowy XElement
            var element = new XElement("Użytkownicy",
                new XElement("Użytkownik",
                new XAttribute("Numer_karty", NrofCard ),
                new XAttribute("Nazwa_użytkownika", Name)));
            document.Add(element);
            document.Save("document.xml");
        }

        public void LoadFromXML()
        {
            var document = XDocument.Load("document.xml");
            var names = document.Element("Użytkownicy")? //jeżeli on istnieje to wyciagnij elementy z nazwa Uzytkownik "?" oznacza "czy istnieje"
                .Elements("Użytkownik")
                .Select(x=>x.Attribute("Nazwa_użytkownika")?.Value);

        }
    }
}
