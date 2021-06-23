/* 2021/6/22 */

namespace TerminalMonitor.Models
{
    public class FieldKey
    {
        public static FieldKey Empty => new()
        {
            Type = FieldKeyType.None,
            Path = null,
        };

        public FieldKeyType Type { get; init; }

        public string Path { get; init; }
    }
}
