using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SsmsLite.Core.App;
using SsmsLite.Core.Database.Entities;
using SsmsLite.Db.DbUpdate.Entities;
using SSMSPlusHistory.Entities;
using SSMSPlusSearch.Entities.Persisted;

namespace SsmsLite.Db.DbUpdate
{
    public class DbUpdater
    {
        private readonly IVersionProvider _versionProvider;
        private readonly Assembly _resourcesAssembly;
        private readonly ILogger<DbUpdater> _logger;
        private readonly Core.Database.Db _db;


        public DbUpdater(
            ILogger<DbUpdater> logger
            , IVersionProvider versionProvider
            , Core.Database.Db db
        )
        {
            _logger = logger;
            _versionProvider = versionProvider;
            _resourcesAssembly = typeof(DbUpdater).Assembly;
            _db = db ?? throw new ArgumentException(nameof(db));
        }

        public void UpdateDb()
        {
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.ExecutionDateUtc);
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.Database);
            _db.GetCollection<QueryItem>().EnsureIndex(o => o.Server);

            _db.GetCollection<AppVersion>().EnsureIndex(o => o.BuildNumber);

            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.DbName);
            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.Server);
            _db.GetCollection<DbDefinition>().EnsureIndex(o => o.DbId);

            _db.GetCollection<DbObject>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbColumn>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbIndex>().EnsureIndex(o => o.DbId);
            _db.GetCollection<DbIndexColumn>().EnsureIndex(o => o.DbId);
        }

        #region OLD

        public void UpdateDbOld()
        {
            var targetBuild = _versionProvider.GetBuild();
            var currentBuild = GetCurrentBuild();

            if (targetBuild < currentBuild)
            {
                var exception =
                    new Exception(
                        $"Target Build version is lower than current, current {currentBuild}, target {targetBuild}");
                _logger.LogError(exception, "Cannot Update DB");
                throw exception;
            }

            if (targetBuild == currentBuild)
            {
                _logger.LogInformation("Build version is up to date, current {@currentBuild}, target {@targetBuild}",
                    currentBuild, targetBuild);
                return;
            }

            _logger.LogInformation("Build version is out of date, current {@currentBuild}, target {@targetBuild}",
                currentBuild, targetBuild);

            const string resourcesPrefix = "SSMSPlusDb.DbUpdate.Versions.";
            var resNames = _resourcesAssembly.GetManifestResourceNames().Where(p => p.StartsWith(resourcesPrefix))
                .OrderBy(p => p).ToArray();

            do
            {
                currentBuild++;
                var buildPrefix = resourcesPrefix + "V" + currentBuild.ToString("0000") + ".";
                var buildResources = resNames.Where(p => p.StartsWith(buildPrefix)).ToArray();
                UpdateVersion(currentBuild, buildResources);
            } while (currentBuild < targetBuild);

            _logger.LogInformation("Build version update {@currentBuild} (finished)", currentBuild);
        }

        private int GetCurrentBuild()
        {
            var collection = _db.GetCollection<AppVersion>();
            var appVersion = collection.Query()
                .OrderByDescending(t => t.BuildNumber)
                .FirstOrDefault();

            return appVersion.BuildVersion == 0 ? 0 : appVersion.BuildNumber;
        }


        private void UpdateVersion(int buildNumber, string[] resourceNames)
        {
            _logger.LogInformation("Updating to {@buildNumber}", buildNumber);

            _db.Command(db =>
                {
                    var buildVersionScriptsLog = new List<BuildVersionScript>();
                    foreach (var resourceName in resourceNames)
                    {
                        _logger.LogInformation("Executing resource {@resourceName}", resourceName);

                        var sql = GetEmbeddedResourceContent(resourceName);
                        db.Execute(sql);

                        // connection.Execute(sql, transaction: transaction);

                        buildVersionScriptsLog.Add(BuildVersionScript.CreateNow(buildNumber, resourceName, sql));
                        _logger.LogInformation("Executing resource {@resourceName} (finished)", resourceName);
                    }

                    HistorizeBuildVersion(buildNumber, buildVersionScriptsLog, db);

                    return 0;
                }, null
                , ex =>
                {
                    _logger.LogError(ex, "Error while updating to {@buildNumber}", buildNumber);
                    return -1;
                }
            );

            _logger.LogInformation("Updating to {@buildNumber} (finished)", buildNumber);
        }

        private static void HistorizeBuildVersion(int buildNumber, IEnumerable<BuildVersionScript> scriptsLog, Core.Database.Db db)
        {
            db.GetCollection<AppVersion>()
                .Insert(new AppVersion
                    {
                        BuildVersion = buildNumber, BuildNumber = buildNumber, Utc = DateTime.UtcNow
                    }
                );
            db.GetCollection<BuildVersionScript>()
                .InsertBulk(scriptsLog);
        }

        private static string GetEmbeddedResourceContent(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                using (var source = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    return source.ReadToEnd();
                }
            }
        }

        #endregion
    }
}