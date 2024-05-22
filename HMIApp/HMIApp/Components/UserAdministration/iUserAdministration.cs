
using System.Xml.Linq;

namespace HMIApp.Components.UserAdministration
{
    public interface IUserAdministration
    {
        bool UserIsLoggedIn { get; }
        void Run();
        void SaveToXML();
        XDocument LoadFromXML(string filepath);
        void ClearListInComboBox();
        void DisplayValuesFromXML(XDocument document, string Name = "");
        void EnabledObjects();
        void ClearUserFromDisplay();
        void FindUserinXML();
        void EditXML();
    }
}
