
namespace HMIApp
{
    //nie ma czegos takiego jak public/private w interface'ach
    //Te rzeczy ktore sa zawsze publicznie widoczne - to okresla nam interface
    //dlatego np. private set co mam na propercjach to go tutaj nie ma
    //czy metod void AddContinuousWritingErrors() itd.
    //Tutaj sa tylko informacje publicznie w jaki sposob mozna korzystac z klasy implementujacej ten interface
    public interface iSiemensPLC
    {
        int Rack { get;}
        int Slot { get; }
        string IP { get; }
        int Port { get; }
        int Timeout_ms { get; }

        bool Init();

        bool Close();

        bool Read(int db, int start, int len, byte[] bytes);

        bool Write(int db, int start, int len, byte[] bytes);

        int continuous_reading_errors();

        int continuous_writing_errors();

    }
}
