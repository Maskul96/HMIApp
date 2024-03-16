
namespace HMIApp.Data
{
    public interface iDataBase
    {
        string ReadConfFile(string filepath);
        void Run();

        void SaveToDataBase();

        void ReadFromDataBase();
    }
}
