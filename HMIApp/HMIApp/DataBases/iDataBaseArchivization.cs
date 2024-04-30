
using HMIApp.Components.DataBase;

namespace HMIApp.Data
{
    public interface iDataBaseArchivization
    {
        string ReadConfFile(string filepath);
        void Run();
        void CountRowsAndDeleteAllData();
        void InsertToDataBase(string message);

#nullable enable
        ArchivizationModelExtendedDataBase? ReadFirst();
#nullable disable
        void SelectFromDataBase(string DateTimeStart, string DateTimeEnd);
        void Delete();

    }
}
