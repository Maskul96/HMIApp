using System;
using HMIApp.Components.UserAdministration;

namespace HMIApp.Archivizations
{

    public class Archivization
    {
        public Form1 obj;
        public Archivization(Form1 obj)
        {
            this.obj = obj;
        }
        public Archivization()
        {

        }

        UserAdministration _users = new UserAdministration();
        App _app = new App();

        //Delegaty  - jest to referencja na metode
        public delegate void TriggerAfterAuto(string DateTime, string NrOfCard);
        public event TriggerAfterAuto TriggerAfterAutoEvent;

        public delegate void TriggerAfterMan(string DateTime, string NrOfCard);
        public event TriggerAfterMan TriggerAfterManEvent;

        public delegate void TriggerAfterLogIn(object sender, EventArgs args);
        public event TriggerAfterLogIn TriggerAfterLogInEvent;

        public delegate void TriggerAfterChangeOver(string DateTime, string NrOfCard);
        public event TriggerAfterChangeOver TriggerAfterChangeOverEvent;

        public void Run()
        {

        }

        public void OnLogInTrigger()
        {
            //Sprawdzamy w ifie czy ktos z zewnatrz (subscriber) podpiął się pod ten event i jak tak to dopiero odpalamy event
                if (TriggerAfterLogInEvent != null)
                {
                //odpalenie eventu
                    TriggerAfterLogInEvent(this, new EventArgs());
                } 
        }

    }
    //archiwizacje zrobic tj. logowanie, przejście manual/auto i przezbrojenie - przejscie manual/auto i przezbrojenie loguje poza data i godzina numer karty usera
    //nie logujemy parametrow poki co
    //Archiwizacja do bazy danych i moduł pozwalajacy wyciagnac z bazy danych eventy z danego dnia i wyeksportowac do csv
}
