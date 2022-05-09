using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Integration;
using SsmsLite.CsvPaste.Helpers;

namespace SsmsLite.CsvPaste
{
    public class CsvPaste
    {
        public const int MenuCommandId = 0x0202;
        private bool isRegistred;
        private PackageProvider _packageProvider;
        private readonly ILogger<CsvPaste> _logger;

        public CsvPaste(PackageProvider packageProvider, ILogger<CsvPaste> logger)
        {
            _packageProvider = packageProvider;
            _logger = logger;
        }

        public void Register()
        {
            if (isRegistred)
            {
                throw new Exception("CsvPaste is already registred");
            }

            isRegistred = true;

            var menuItem = new MenuCommand(this.MenuItemCallback, new CommandID(MenuHelper.CommandSet, MenuCommandId));
            _packageProvider.CommandService.AddCommand(menuItem);
        }

        // private PasteCommand()
        // {
            //OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            //if (commandService != null)
            //{
            //    var menuItem = new MenuCommand(this.MenuItemCallback, new CommandID(CommandSet, MenuCommandId));
            //    commandService.AddCommand(menuItem);
            //}
        // }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var document = _packageProvider.Dte2.ActiveDocument;
            if (document == null)
                return;
            var textDocument = (TextDocument)document.Object("TextDocument");
            var queryText = textDocument.Selection.Text;
            textDocument.Selection.Delete();
            var point = textDocument.CreateEditPoint();


            // IWpfTextView wpfTextView = WpfTextViewHelper.GetWpfTextView(_packageProvider.AsyncPackage);//ServiceProvider
            // int caretPosition = WpfTextViewHelper.GetCaretPosition(wpfTextView);

            var clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText))
                return;

            var formatedText = Text.GetFormattedText(clipboardText);
            if (string.IsNullOrWhiteSpace(formatedText))
                return;

            //var edit = wpfTextView.TextBuffer.CreateEdit();
            //            edit.Insert(caretPosition, formatedText);
            //            edit.Apply();
            textDocument.Selection.Insert(formatedText);

        }

    }
}