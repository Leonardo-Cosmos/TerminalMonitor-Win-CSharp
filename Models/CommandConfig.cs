/* 2021/5/30 */

using System;

namespace TerminalMonitor.Models
{
    public class CommandConfig
    {
        /// <summary>
        /// Unique ID of command.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of command. This value is required and unique.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The file to start command. This value is required.
        /// </summary>
        public string? StartFile { get; set; }

        /// <summary>
        /// The arguments of command. This value is optional.
        /// </summary>
        public string? Arguments { get; set; }

        /// <summary>
        /// The work directory path of command execution. This value is optional.
        /// </summary>
        public string? WorkDirectory { get; set; }
    }
}
