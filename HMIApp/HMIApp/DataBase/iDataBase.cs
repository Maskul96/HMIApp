
using HMIApp.Components.DataBase;

namespace HMIApp.Data
{
    public interface iDataBase
    {
        string ReadConfFile(string filepath);
        void Run();

        void InsertToDataBase();

        void SelectFromDataBase(string referencenumber);
        void SelectFromDbToComboBox();
        Reference? ReadFirst(string referencenumber);
        void UpdateDb(string referencenumber);
        void Delete(string referencenumber);

    }
}
