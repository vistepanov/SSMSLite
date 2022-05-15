using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Utils.TextProcessor;

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

        public string DocumentFullName => Dte2.ActiveDocument?.FullName ?? "";
        public TextDocument TextDocument => (TextDocument)Dte2.ActiveDocument.Object("TextDocument");

        private Document GetActiveDocument()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Dte2.ActiveDocument;
        }

        public TextDocument GetTextDocument(bool writeMode = false)
        {
            var document = GetActiveDocument();
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

        public void SetStatus(string message, params object[] args)
        {
            Dte2.StatusBar.Text = string.Format(message, args);
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

        public Cursor Cursor
        {
            get
            {
                var point = TextDocument.Selection.TopPoint;
                return new Cursor(point.DisplayColumn, point.Line);
            }
            set
            {
                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                TextDocument.Selection.MoveToPoint(point, false);
            }
        }

        public string CurrentLine
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var startPoint = cursor.CreateEditPoint();
                var endPoint = cursor.CreateEditPoint();
                startPoint.StartOfLine();
                endPoint.EndOfLine();
                return startPoint.GetText(endPoint);
            }
        }

        public string CurrentWord
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                var rPoint = cursor.CreateEditPoint();
                rPoint.WordRight(1);
                point.WordLeft(1);
                var txt = point.GetText(rPoint).Trim();
                return txt.Contains(" ") ? point.GetText(cursor).Trim() : txt;
            }
        }

    }
}