using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Search.UI;

namespace SsmsLite.Search
{
    public class SearchUi
    {
        public const int MenuCommandId = 1101;

        private readonly PackageProvider _packageProvider;
        private readonly DbConnectionProvider _dbConnectionProvider;

        private bool _isRegistered;
        private int _id;

        public SearchUi(PackageProvider packageProvider, DbConnectionProvider dbConnectionProvider)
        {
            _packageProvider = packageProvider;
            _dbConnectionProvider = dbConnectionProvider;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("SearchUi is already registered");
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
Connect to a user database", "SsmsLite");
                    return;
                }
            }

            Launch(cnx);
        }

        private void Launch(DbConnectionString dbConnectionString)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var toolWindow =
                _packageProvider.AsyncPackage.FindToolWindow(typeof(SearchToolWindow), _id++, true) as SearchToolWindow;
            toolWindow?.Intialize(dbConnectionString);

            var frame = (IVsWindowFrame)toolWindow?.Frame;
            frame?.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
            if (frame != null) Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}