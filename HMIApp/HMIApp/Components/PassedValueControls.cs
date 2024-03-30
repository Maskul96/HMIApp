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
    }
}
