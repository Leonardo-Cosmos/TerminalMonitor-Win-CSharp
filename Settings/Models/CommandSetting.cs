/* 2021/5/30 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record CommandSetting(string Name, string StartFile, string Arguments, string WorkDirectory);

    static class CommandConfigSettings
    {
        public static CommandSetting Save(CommandConfig obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new CommandSetting(
                Name: obj.Name,
                StartFile: obj.StartFile,
                Arguments: obj.Arguments,
                WorkDirectory: obj.WorkDirectory
                );
        }

        public static CommandConfig Load(CommandSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new CommandConfig()
            {
                Name = setting.Name,
                StartFile = setting.StartFile,
                Arguments = setting.Arguments,
                WorkDirectory = setting.WorkDirectory,
            };
        }
    }
}
