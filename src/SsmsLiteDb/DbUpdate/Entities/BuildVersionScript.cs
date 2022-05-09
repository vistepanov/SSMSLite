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
            var instance = new BuildVersionScript();
            instance.BuildNumber = buildNumber;
            instance.ScriptName = scriptName;
            instance.ScriptContent = scriptContent;
            instance.InstallDateUtc = DateTime.UtcNow;
            return instance;
        }
    }
}
