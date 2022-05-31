using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SsmsLite.Core.Integration
{
    public class PackageProvider
    {
        public DTE2 Dte2 { get; }
        public AsyncPackage AsyncPackage { get; }
        public OleMenuCommandService CommandService { get; }

        public PackageProvider(DTE2 dte2, AsyncPackage asyncPackage, OleMenuCommandService commandService)
        {
            Dte2 = dte2;
            AsyncPackage = asyncPackage;
            CommandService = commandService;
        }

        public string DocumentFullName => Dte2.ActiveDocument?.FullName ?? "";
        public TextDocument TextDocument => GetTextDocument();

        public bool CanExecute()
        {
            return IsCurrentDocumentExtension("sql")
                   && AllText.Length > 0;
        }

        public void SetStatus(string message, params object[] args)
        {
            Dte2.StatusBar.Text = string.Format(message, args);
        }

        public TextDocument GetTextDocument(bool writeMode = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var document = Dte2.ActiveDocument;
            if (document == null || (document.ReadOnly && writeMode)) return null;
            return (TextDocument)document.Object("TextDocument");
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


        public bool IsCurrentDocumentExtension(string extension)
        {
            return Path.GetExtension(DocumentFullName).Equals(
                $".{extension}", StringComparison.CurrentCultureIgnoreCase
            );
        }

        public string AllText
        {
            get
            {
                var point = TextDocument.StartPoint.CreateEditPoint();
                point.StartOfDocument();
                return point.GetText(TextDocument.EndPoint);
            }
        }


        public string CurrentWord
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                var rPoint = cursor.CreateEditPoint();
                rPoint.WordRight();
                point.WordLeft();
                var txt = point.GetText(rPoint).Trim();
                point.CharLeft();
                var t2 = point.GetText(rPoint);

                return txt.Contains(" ") ? point.GetText(cursor).Trim() : txt;
            }
        }

    }
}