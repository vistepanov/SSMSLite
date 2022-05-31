using System;

namespace SsmsLite.Db.DbUpdate.Entities
{
    public class BuildVersionScript
    {
        public int BuildNumber { get; set; }
        public string ScriptName { get; set; }
        public string ScriptContent { get; set; }
        public DateTime InstallDateUtc { get; set; }

        public static BuildVersionScript CreateNow(int buildNumber, string scriptName, string scriptContent)
        {
            var instance = new BuildVersionScript
            {
                BuildNumber = buildNumber,
                ScriptName = scriptName,
                ScriptContent = scriptContent,
                InstallDateUtc = DateTime.UtcNow
            };
            return instance;
        }
    }
}
