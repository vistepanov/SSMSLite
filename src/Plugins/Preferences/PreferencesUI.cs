using SSMSPlusPreferences.UI;
using System;
using SsmsLite.Core.Integration;
using SsmsLite.Core.Ui.Extensions;

namespace SSMSPlusPreferences
{
    public class PreferencesUI
    {
        public const int MenuCommandId = 2001;

        private bool _isRegistered;
        private PreferencesWindow _window;

        private readonly PackageProvider _packageProvider;

        public PreferencesUI(PackageProvider packageProvider)
        {
            _packageProvider = packageProvider;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("PreferencesUI is already registered");
            }

            _isRegistered = true;
            MenuHelper.AddMenuCommand(_packageProvider, ExecuteFromMenu, MenuCommandId);
        }

        private void ExecuteFromMenu(object sender, EventArgs e)
        {
            if (_window == null)
                _window = new PreferencesWindow();

            _window.ShowAndActivate();
        }
    }
}