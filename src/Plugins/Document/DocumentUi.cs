using Microsoft.VisualStudio.Shell.Interop;
using SSMSPlusDocument.UI;
using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;

namespace SSMSPlusDocument
{
    public class DocumentUi
    {
        public const int MenuCommandId = 1201;

        private bool isRegistred = false;
        private int id;

        private PackageProvider _packageProvider;
        DbConnectionProvider _dbConnectionProvider;

        public DocumentUi(PackageProvider packageProvider, DbConnectionProvider dbConnectionProvider)
        {
            _packageProvider = packageProvider;
            _dbConnectionProvider = dbConnectionProvider;
        }

        public void Register()
        {
            if (isRegistred)
            {
                throw new Exception("DocumentUi is already registred");
            }

            isRegistred = true;

            var menuItem = new MenuCommand(this.ExecuteFromMenu, new CommandID(MenuHelper.CommandSet, MenuCommandId));
            _packageProvider.CommandService.AddCommand(menuItem);
        }

        private void ExecuteFromMenu(object sender, EventArgs e)
        {
            var cnx = _dbConnectionProvider.GetFromSelectedDatabase();
            if (cnx == null)
            {
                cnx = _dbConnectionProvider.GetFromActiveConnection();
                if (cnx == null)
                {
                    System.Windows.MessageBox.Show(
@"Please select a user database in object explorer
Or 
Connect to a user database", "SSMS plus");
                    return;
                }
            }

            Launch(cnx);
        }

        private void Launch(DbConnectionString dbConnectionString)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var toolWindow = _packageProvider.AsyncPackage.FindToolWindow(typeof(ExportDocumentsWindow), id++, true) as ExportDocumentsWindow;
            toolWindow?.Intialize(dbConnectionString);
            var frame = (IVsWindowFrame)toolWindow?.Frame;
            frame?.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
            if (frame != null) Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
