/* 2021/4/20 */

namespace TerminalMonitor.Models
{
    public class TerminalLineField
    {
        /// <summary>
        /// Original key in parsed result.
        /// </summary>
        public required string Key { get; init; }

        /// <summary>
        /// The full path that starts with category prefix. It is unique in a terminal line.
        /// </summary>
        public required string FieldKey { get; init; }

        /// <summary>
        /// Original value in parsed result.
        /// </summary>
        public required object? Value { get; init; }

        /// <summary>
        /// String representation of the value.
        /// </summary>
        public required string Text { get; init; }
    }
}
