using HMIApp.Components.CSVReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp.Components
{
    public interface iCSVReader
    {

        List<DBTag> DBStructure(string filePath);
        List<DBTagAlarms> DBAlarmStructure(string filePath);

    }
}
