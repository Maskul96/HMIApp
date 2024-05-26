
using System.Xml.Linq;

namespace HMIApp.Components.UserAdministration
{
    public interface IUserAdministration
    {
        bool UserIsLoggedIn { get; }
        void Run();
        void SaveToXML();
        void ClearListInComboBox();
        void DisplayValuesFromXML(XDocument document, string Name = "");
        void EnabledObjects();
        void ClearUserFromDisplay();
        void FindUserinXML();
        void EditXML();
    }
}
