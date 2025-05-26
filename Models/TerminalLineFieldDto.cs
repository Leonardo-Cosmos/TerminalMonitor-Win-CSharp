/* 2021/4/20 */

namespace TerminalMonitor.Models
{
    public class TerminalLineFieldDto
    {
        public required string Key { get; init; }

        public required string FieldKey { get; init; }

        public required object? Value { get; init; }

        public required string Text { get; init; }

    }
}
