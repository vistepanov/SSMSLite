using Microsoft.VisualStudio.Shell.Interop;
using SSMSPlusDocument.UI;
using System;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;

namespace SSMSPlusDocument
{
    public class DocumentUi
    {
        public const int MenuCommandId = 1201;

        private bool _isRegistered;
        private int _id;

        private readonly PackageProvider _packageProvider;
        private readonly DbConnectionProvider _dbConnectionProvider;

        public DocumentUi(PackageProvider packageProvider, DbConnectionProvider dbConnectionProvider)
        {
            _packageProvider = packageProvider;
            _dbConnectionProvider = dbConnectionProvider;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("DocumentUi is already registered");
            }

            _isRegistered = true;

            MenuHelper.AddMenuCommand(_packageProvider, ExecuteFromMenu, MenuCommandId);
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

            var toolWindow = _packageProvider.AsyncPackage.FindToolWindow(typeof(ExportDocumentsWindow), _id++, true) as ExportDocumentsWindow;
            toolWindow?.Intialize(dbConnectionString);
            var frame = (IVsWindowFrame)toolWindow?.Frame;
            frame?.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
            if (frame != null) Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
