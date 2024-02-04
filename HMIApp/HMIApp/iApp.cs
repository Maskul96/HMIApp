using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp
{
    public interface iApp
    {
        void RunInitPLC();
        void RunReadFromCSVFileandReadFromDB();

        void WriteToDB(string DataTypeofTag, int NrOfByteinDB, int LengthofDataType);

    }
}
