﻿using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using SsmsLite.Core.Integration.Connection;

namespace SsmsLite.Search.UI
{
    [Guid("ebbe0b2f-22b6-4bef-b9fe-f5b695f42be0")]
    public class SearchToolWindow : ToolWindowPane
    {
        public SchemaSearchControl SchemaSearchControl => Content as SchemaSearchControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowPane"/> class.
        /// </summary>
        public SearchToolWindow() : base(null)
        {
            Caption = "Schema Search";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new SchemaSearchControl();
        }

        public void Intialize(DbConnectionString cnxStr)
        {
            Caption = "Schema Search: " + cnxStr.Database;
            SchemaSearchControl.Initialize(cnxStr);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Content = null;
            GC.Collect();
        }
    }
}