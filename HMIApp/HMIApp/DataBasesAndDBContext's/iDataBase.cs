
using HMIApp.Components.DataBase;

namespace HMIApp.Data
{
    public interface IDataBase
    {
        string ReadConfFile(string filepath);
        void Run();

        void InsertToDataBase();

        void SelectFromDataBase(string referencenumber);
        void SelectFromDbToComboBox();
#nullable enable
        Reference? ReadFirst(string referencenumber);
#nullable disable
        void UpdateDb(string referencenumber);
        void Delete(string referencenumber);

    }
}
