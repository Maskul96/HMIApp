
using HMIApp.Components.DataBase;

namespace HMIApp.Data
{
    public interface iDataBaseArchivization
    {
        string ReadConfFile(string filepath);
        void Run();

        void InsertToDataBase(string message);

#nullable enable
        ArchivizationModelExtendedDataBase? ReadFirst(string referencenumber);
#nullable disable

        void Delete(string referencenumber);

    }
}
