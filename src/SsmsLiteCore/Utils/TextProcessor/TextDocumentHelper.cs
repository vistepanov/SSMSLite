using EnvDTE;

namespace SsmsLite.Core.Utils.TextProcessor
{
    public static class TextDocumentHelper
    {
        public static string GetText(this TextDocument textDocument)
        {
            if (textDocument.Selection.IsEmpty)
                textDocument.Selection.SelectAll();

            var text = textDocument.Selection.Text;
            textDocument.Selection.Cancel();
            return text;
        }
    }
}