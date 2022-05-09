using System;
using System.ComponentModel.Design;

namespace SsmsLite.Core.Integration
{
    public static class MenuHelper
    {
        private static readonly Guid CommandSet = new Guid("26d56063-673e-4de6-b6bd-8ebf0f367895");

        public static void AddMenuCommand(PackageProvider provider, EventHandler handler, int cmdId)
        {
            var menuItem = new MenuCommand(handler, new CommandID(CommandSet, cmdId));
            provider.CommandService.AddCommand(menuItem);
        }
    }
}
