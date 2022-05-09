using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SSMSPlusHistory.UI;
using System;
using SsmsLite.Core.Integration;

namespace SSMSPlusHistory
{
    public class HistoryUi
    {
        public const int CommandId = 1001;

        private readonly PackageProvider _packageProvider;

        private IVsWindowFrame _window;
        private bool _isRegistered;

        public HistoryUi(PackageProvider packageProvider)
        {
            _packageProvider = packageProvider;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("HistoryUi is already registered");
            }

            _isRegistered = true;

            MenuHelper.AddMenuCommand(_packageProvider, Execute, CommandId);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var toolWindow = _packageProvider.AsyncPackage.FindToolWindow(typeof(HistoryToolWindow), 0, true);
            _window = (IVsWindowFrame)toolWindow.Frame;
            _window.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(_window.Show());
        }
    }
}
