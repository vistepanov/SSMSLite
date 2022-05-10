using System;
using EnvDTE;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Integration;

namespace SsmsLite.Sync
{
    public class Sync
    {
        public const int MenuCommandId = 0x0203;
        private bool _isRegistered;
        private readonly PackageProvider _packageProvider;
        private readonly ILogger<Sync> _logger;

        public Sync(PackageProvider packageProvider, ILogger<Sync> logger)
        {
            _packageProvider = packageProvider;
            _logger = logger;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("Sync is already registered");
            }

            _isRegistered = true;

            MenuHelper.AddMenuCommand(_packageProvider, MenuItemCallback, MenuCommandId);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var document = _packageProvider.GetTextDocument();
            if (document == null) return;
            var world= LookupObject(document.Selection);
            document.Selection.Cancel();
        }

        private string LookupObject(TextSelection point)
        {
            if (point.IsEmpty)
            {
                point.WordLeft();
                point.WordRight(true);
            }

            return point.Text;

        }

    }
}