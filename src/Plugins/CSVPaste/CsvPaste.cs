using System;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.Integration;
using SsmsLite.CsvPaste.Helpers;

namespace SsmsLite.CsvPaste
{
    public class CsvPaste
    {
        public const int MenuCommandId = 0x0202;
        private bool _isRegistered;
        private readonly PackageProvider _packageProvider;
        private readonly ILogger<CsvPaste> _logger;

        public CsvPaste(PackageProvider packageProvider, ILogger<CsvPaste> logger)
        {
            _packageProvider = packageProvider;
            _logger = logger;
        }

        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("CsvPaste is already registered");
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
            var clipboardText = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(clipboardText))
                return;

            var formattedText = Text.GetFormattedText(clipboardText);
            if (string.IsNullOrWhiteSpace(formattedText))
                return;

            var textDocument = _packageProvider.GetTextDocument(true);
            if (textDocument == null )
                return;
            // textDocument.Selection.Delete();
            // пробовал через CreateEditPoint - вставка идёт в начало документа.
            textDocument.Selection.Insert(formattedText);
        }
    }
}