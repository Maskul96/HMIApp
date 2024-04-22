//metoda zawierajaca przekazanie wartosci z kontrolki do innej kontrolki bezposrednio w przypadku kiedy koniecznosc jest wyswietlenia tej samej wartosci w kilku miejsach
namespace HMIApp.Components
{
    public class PassedValueControls
    {
        public PassedValueControls()
        {
            
        }
        public PassedValueControls(Form1 obj)
        {
            this.obj = obj;
        }
        public Form1 obj;

        //metoda statyczna do wywolywania przepisania danych
        public static void Run()
        {
            //przepisywanie numeru referencji i klienta do wyswietlenia dla operatora
            Form1._Form1.DB666NrReferencePassedValue.Text = Form1._Form1.DB666NrReference.Text;
            Form1._Form1.DB666NameOfClientPassedValue.Text = Form1._Form1.DB666NameOfClient.Text;
            //
            Form1._Form1.DB667PozycjaOslonki_PassedValue.Text = Form1._Form1.DB667PozycjaOslonki.Text;
            Form1._Form1.DB667PozycjaGniazdoDolne_PassedValue.Text = Form1._Form1.DB667PozycjaGniazdoDolne.Text;
            Form1._Form1.DB667AktSila_PassedValue.Text = Form1._Form1.DB667AktSila.Text; 
            Form1._Form1.DB667PozycjaPrzeciskanie_PassedValue.Text = Form1._Form1.DB667PozycjaPrzeciskanie.Text;

            //IO
            Form1._Form1.DB669Input49PassedValue.BackColor = Form1._Form1.DB669Input49.BackColor;
            Form1._Form1.DB669Input50PassedValue.BackColor = Form1._Form1.DB669Input50.BackColor;
            Form1._Form1.DB669Input10PassedValue.BackColor = Form1._Form1.DB669Input10.BackColor;
            Form1._Form1.DB669Input14PassedValue.BackColor = Form1._Form1.DB669Input14.BackColor;
            Form1._Form1.DB669Input15PassedValue.BackColor = Form1._Form1.DB669Input15.BackColor;
            Form1._Form1.DB669Input19PassedValue.BackColor = Form1._Form1.DB669Input19.BackColor;
            Form1._Form1.DB669Input22PassedValue.BackColor = Form1._Form1.DB669Input22.BackColor;
            Form1._Form1.DB669Input23PassedValue.BackColor = Form1._Form1.DB669Input23.BackColor;
            Form1._Form1.DB669Input24PassedValue.BackColor = Form1._Form1.DB669Input24.BackColor;
        }

    }
}
