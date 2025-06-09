/* 2021/5/30 */
using System;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record CommandConfigSetting(string Id, string Name, string? StartFile, string? Arguments, string? WorkDirectory);

    static class CommandConfigSettings
    {
        public static CommandConfigSetting? Save(CommandConfig? obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new CommandConfigSetting(
                Id: obj.Id.ToString(),
                Name: obj.Name,
                StartFile: obj.StartFile,
                Arguments: obj.Arguments,
                WorkDirectory: obj.WorkDirectory
                );
        }

        public static CommandConfig? Load(CommandConfigSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new CommandConfig()
            {
                Id = Guid.TryParse(setting.Id, out Guid result) ? result : Guid.NewGuid(),
                Name = setting.Name,
                StartFile = setting.StartFile,
                Arguments = setting.Arguments,
                WorkDirectory = setting.WorkDirectory,
            };
        }
    }
}
