/* 2021/6/22 */

using System;

namespace TerminalMonitor.Matchers.Models
{
    [Obsolete]
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
