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
        void ReadActualValueFromDB(string filepath);
        bool GetBit(byte b, int bitNumber);
        void WriteToDB(string valuetoWrite, string DataTypeofTag);

    }
}
