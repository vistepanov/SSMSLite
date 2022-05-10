using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Messaging;
using SsmsLite.Core.Ui;
using SsmsLite.Core.Ui.Commands;
using SsmsLite.Core.Ui.Text;
using SsmsLite.Core.Utils;
using SsmsLite.Document.Services;

namespace SsmsLite.Document.UI
{
    public class ExportDocumentsControlVm : ViewModelBase
    {
        private DbConnectionString _dbConnectionString;

        public Command ChooseFolderCmd { get; private set; }
        public AsyncCommand ExportFilesCmd { get; private set; }
        public Command CancelExportFilesCmd { get; private set; }

        public CancellationTokenSource CancelToken { get; private set; }
        public RunStream ConsoleOutput { get; private set; }

        public ExportDocumentsControlVm()
        {
            ChooseFolderCmd = new Command(OnChooseFolder, null, HandleError);
            ExportFilesCmd = new AsyncCommand(OnExportFilesAsync, null, HandleError);
            CancelExportFilesCmd = new Command(OnCancelExportFiles, null, HandleError);
            ConsoleOutput = new RunStream();
        }

        public void InitializeDb(DbConnectionString cnxStr)
        {
            _dbConnectionString = cnxStr;
            DbDisplayName = _dbConnectionString.DisplayName;
        }

        private async Task OnExportFilesAsync()
        {
            this.CancelToken = new CancellationTokenSource();
            IsExporting = true;
            ConsoleOutput.SendStandard("Starting Export");
            var exportedProgress = new Progress<ReportMessage>(OnExportProgress);
            try
            {
                await FileExporter.ExportFilesAsync(_dbConnectionString, SqlQuery, FolderPath, this.CancelToken.Token,
                    exportedProgress);
                ConsoleOutput.SendSuccess("Export finished");
            }
            catch (OperationCanceledException)
            {
                Message = "Cancelled";
                ConsoleOutput.SendStandard("Export cancelled");
            }
            finally
            {
                IsExporting = false;
            }
        }

        private void OnExportProgress(ReportMessage obj)
        {
            switch (obj.Level)
            {
                case ReportMessageLevel.Warning:
                    ConsoleOutput.SendWarning(obj.Message);
                    break;
                case ReportMessageLevel.Error:
                    ConsoleOutput.SendError(obj.Message);
                    break;
                default:
                    ConsoleOutput.SendStandard(obj.Message);
                    break;
            }
        }

        private void OnCancelExportFiles()
        {
            this.CancelToken.Cancel();
        }

        private void OnChooseFolder()
        {
            OpenFileDialog folderBrowser = new OpenFileDialog
            {
                // not let you select "Folder Selection."
                // Set validate names and check file exists to false otherwise windows will
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                // Always default to Folder Selection.
                FileName = "Folder Selection."
            };
            if (folderBrowser.ShowDialog() != true) return;
            FolderPath = Path.GetDirectoryName(folderBrowser.FileName);
            IsValidFolderPath = true;
        }

        private void HandleError(Exception ex)
        {
            ConsoleOutput.SendError(ex.GetFullStackTraceWithMessage());
        }

        private string _sqlQuery = "Select top 10 FileName, FileContent FROM Files";
        public string SqlQuery { get => _sqlQuery; set => SetField(ref _sqlQuery, value); }

        private string _message;
        public string Message { get => _message; set => SetField(ref _message, value); }

        private string _folderPath;
        public string FolderPath { get => _folderPath; set => SetField(ref _folderPath, value); }

        private bool _isValidFolderPath;
        public bool IsValidFolderPath { get => _isValidFolderPath;
            set
            {
                SetField(ref _isValidFolderPath, value);
                RaisePropertyChanged(nameof(CanExport));
            }
        }

        private bool _isExporting;
        public bool IsExporting { get => _isExporting;
            set
            {
                SetField(ref _isExporting, value);
                RaisePropertyChanged(nameof(CanExport));
            }
        }

        public bool CanExport => !IsExporting && IsValidFolderPath;

        private string _dbDisplayName;
        public string DbDisplayName { get => _dbDisplayName; set => SetField(ref _dbDisplayName, value); }

        public void Free()
        {
            CancelToken?.Cancel();
        }
    }
}