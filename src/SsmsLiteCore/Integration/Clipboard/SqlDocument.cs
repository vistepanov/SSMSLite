using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SsmsLite.Core.Integration.Clipboard
{
    public class SqlDocument
    {
        public static Document GetActiveDocument(PackageProvider packageProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return packageProvider.Dte2.ActiveDocument;
        }

        public static TextDocument GetTextDocument(PackageProvider packageProvider)
        {
            var document = GetActiveDocument(packageProvider);

            return (TextDocument)document?.Object("TextDocument");
        }

        /// <summary>
        /// Get script text from active window
        /// </summary>
        /// <returns></returns>
        public static string GetQueryText(PackageProvider packageProvider)
        {
            var textDocument = GetTextDocument(packageProvider);
            if (textDocument == null) return null;
            var queryText = textDocument.Selection.Text;

            if (!string.IsNullOrEmpty(queryText)) return queryText;

            var startPoint = textDocument.StartPoint.CreateEditPoint();
            return startPoint.GetText(textDocument.EndPoint);
        }

    }
}