using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SsmsLite.Core.Integration.Connection;
using SsmsLite.Core.Messaging;
using SsmsLite.Core.Utils.IO;

namespace SsmsLite.Document.Services
{
    public static class FileExporter
    {
        public static async Task ExportFilesAsync(DbConnectionString dbConnectionString
            , string sqlQuery
            , string folderPath
            , CancellationToken ct
            , IProgress<ReportMessage> progress)
        {
            using (var dbCon = new SqlConnection(dbConnectionString.ConnectionString))
            {
                await dbCon.OpenAsync(ct);
                using (var cmd = new SqlCommand(sqlQuery, dbCon))
                {
                    // 10 hours
                    cmd.CommandTimeout = 60 * 60 * 10;
                    using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        try
                        {
                            while (await reader.ReadAsync(ct))
                            {
                                ct.ThrowIfCancellationRequested();

                                var fileName = Convert.ToString(reader.GetValue(0));
                                var validFileName = MakeValidFileName(progress, fileName);
                                var bytes = (byte[])reader.GetValue(1);
                                var fullPath = Path.Combine(folderPath, validFileName);
                                File.WriteAllBytes(fullPath, bytes);

                                progress.Report(ReportMessage.Standard("Exported: " + validFileName));
                            }
                        }
                        finally
                        {
                            cmd.Cancel();
                            reader.Close();
                        }
                    }
                }
            }
        }

        private static string MakeValidFileName(IProgress<ReportMessage> progress, string fileName)
        {
            var validFileName = FileExtensions.MakeValidFileName(fileName, out var changed, (invalidChar) => "_");
            if (!changed) return validFileName;
            progress.Report(ReportMessage.Warning("Invalid filename: " + fileName));
            progress.Report(ReportMessage.Warning("Replacement: " + validFileName));

            return validFileName;
        }
    }
}
