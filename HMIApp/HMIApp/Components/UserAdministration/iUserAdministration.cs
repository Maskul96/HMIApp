
namespace HMIApp.Components.UserAdministration
{
    public interface iUserAdministration
    {
        bool UserIsLoggedIn { get; }
        void Run();
        void SaveToXML();
        void LoadFromXML();
        void UpdateDisplayValuesFromXML();
        void ClearListinComboBox();

        void EnabledObjects();
        void ClearUserFromDisplay();
        void FindUserinXML();
        void EditXML();
    }
}
