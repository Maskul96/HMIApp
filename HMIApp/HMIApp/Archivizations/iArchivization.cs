using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMIApp.Archivizations
{
    public interface iArchivization
    {
        delegate void ArchiveEvents(object sender, EventArgs args, string message);

        void Run();
        void _Archive_ArchiveEvent(object sender, EventArgs args, string message);

        void ArchivizationCsvFileHandlingForBasicModel();
        void ArchivizationCsvFileHandlingForExtendedModel();
        int NumberOfProductionShift();
        void ClearListArchivizationModelBasic();
        void ClearListArchivizationModelExtended();
        void OnArchiveEventsMethod(string message);

        void AddingYearToComboBoxArchivizationToCSVForm1();
        void ExportToCSVButtonFromForm1();


    }
}
