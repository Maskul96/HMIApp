
namespace HMIApp.Components.UserAdministration
{
    public interface iUserAdministration
    {
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
