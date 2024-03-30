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
            Form1._Form1.DB666Tag16PassedValue.Text = Form1._Form1.DB666Tag16.Text;
            Form1._Form1.DB666Tag17PassedValue.Text = Form1._Form1.DB666Tag17.Text;
        }

    }
}
