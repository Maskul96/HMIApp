using HMIApp.Components.CSVReader.Models;
using System.Collections.Generic;

namespace HMIApp.Components
{
    public interface iCSVReader
    {

        List<DBTag> DBStructure(string filePath);
        List<DBTagAlarms> DBAlarmStructure(string filePath);

    }
}
