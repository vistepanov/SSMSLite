using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Integration.Connection;
using SSMSPlusSearch.UI;

namespace SSMSPlusSearch
{
    public class SearchUi
    {
        public const int MenuCommandId = 1101;

        private PackageProvider _packageProvider;
        DbConnectionProvider _dbConnectionProvider;

        private bool isRegistred = false;


        public SearchUi(PackageProvider packageProvider, DbConnectionProvider dbConnectionProvider)
        {
            _packageProvider = packageProvider;
            _dbConnectionProvider = dbConnectionProvider;
        }

        public void Register()
        {
            if (isRegistred)
            {
                throw new Exception("SearchUi is already registred");
            }

            isRegistred = true;

            var menuItem = new MenuCommand(this.ExecuteFromMenu, new CommandID(MenuHelper.CommandSet, MenuCommandId));
            _packageProvider.CommandService.AddCommand(menuItem);
        }

        private int id;

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

            var toolWindow = _packageProvider.AsyncPackage.FindToolWindow(typeof(SearchToolWindow), id++, true) as SearchToolWindow;
            toolWindow?.Intialize(dbConnectionString);

            var frame = (IVsWindowFrame)toolWindow?.Frame;
            frame?.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
            if (frame != null) Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}