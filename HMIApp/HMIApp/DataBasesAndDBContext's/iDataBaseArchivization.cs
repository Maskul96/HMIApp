
using HMIApp.Components.DataBase;
using System.Collections.Generic;

namespace HMIApp.Data
{
    public interface iDataBaseArchivization
    {
        string ReadConfFile(string filepath);
        void Run();
        void CountRowsAndDeleteAllData();
        void InsertToDataBase(string message, int nrofshift);

#nullable enable
        ArchivizationModelExtendedDataBase? ReadFirst();
#nullable disable
        List<ArchivizationModelExtendedDataBase> SelectFromDataBase(string DateTimeStart, string DateTimeEnd, int HourStart = 0, int HourEnd = 0);
        void Delete();

    }
}
