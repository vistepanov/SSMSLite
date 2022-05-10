using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SsmsLite.Core.Integration
{
    public class PackageProvider
    {
        public DTE2 Dte2 { get; private set; }
        public AsyncPackage AsyncPackage { get; private set; }
        public OleMenuCommandService CommandService { get; private set; }

        public PackageProvider(DTE2 dte2, AsyncPackage asyncPackage, OleMenuCommandService commandService)
        {
            Dte2 = dte2;
            AsyncPackage = asyncPackage;
            CommandService = commandService;
        }


        private Document GetActiveDocument()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Dte2.ActiveDocument;
        }

        public TextDocument GetTextDocument(bool writeMode = false)
        {
            var document = GetActiveDocument();
            if (document == null || (document.ReadOnly && writeMode)) return null;
            return (TextDocument)document?.Object("TextDocument");
        }

        /// <summary>
        /// Get script text from active window
        /// </summary>
        /// <returns></returns>
        public string GetQueryText()
        {
            var textDocument = GetTextDocument();
            if (textDocument == null) return null;
            var queryText = textDocument.Selection.Text;

            if (!string.IsNullOrEmpty(queryText)) return queryText;

            var startPoint = textDocument.StartPoint.CreateEditPoint();
            return startPoint.GetText(textDocument.EndPoint);
        }

        public EditPoint GetEditPoint()
        {
            return GetTextDocument().CreateEditPoint();
        }
    }
}
