using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp.Components.UserAdministration
{
    public interface iUserAdministration
    {
        void Run();
        void SaveToXML();
        void LoadFromXML();
        void UpdateDisplayValuesFromXML();
        void ClearListinComboBox();

        void EditXML();
    }
}
