using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Database.Entities;

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
/*
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
*/
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
                //do
                //{
                point.CharLeft(1);
                var t2 = point.GetText(rPoint);
                //    _lim.Contains(t2[0]);
                //} while()

                return txt.Contains(" ") ? point.GetText(cursor).Trim() : txt;
            }
        }
/*
        public DbToken FindCurrentObj()
        {
            string text;
            string server = "";
            string database = "";
            string schema = "dbo";
            string obj;
            var cursor = TextDocument.Selection.ActivePoint;
            var point = cursor.CreateEditPoint();
            point.WordLeft(1);
            text = point.GetText(cursor).Trim();

            var rPoint = cursor.CreateEditPoint();
            rPoint.WordRight(1);
            var txt = point.GetText(rPoint).Trim(); // тут мы почти всегда получим имя 

            point.CharLeft(1);
            var t2 = point.GetText(rPoint);
            //    _lim.Contains(t2[0]);
            //} while()

            obj = txt.Contains(" ") ? point.GetText(cursor).Trim() : txt;
            return new DbToken(obj, schema);
        }
*/
    }
}