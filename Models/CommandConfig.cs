/* 2021/5/30 */

namespace TerminalMonitor.Models
{
    public class CommandConfig
    {
        /// <summary>
        /// The name of command. This value is required and unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The file to start command. This value is required.
        /// </summary>
        public string StartFile { get; set; }

        /// <summary>
        /// The arguments of command. This value is optional.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// The work directory path of command execution. This value is optional.
        /// </summary>
        public string WorkDirectory { get; set; }
    }
}
