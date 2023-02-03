/* 2023/2/3 */
using System;
using System.Windows;

namespace TerminalMonitor.Models
{
    public class ColumnHeaderStyle : ICloneable
    {
        public static ColumnHeaderStyle Empty = new()
        {
        };

        public TextColorConfig Foreground { get; set; }

        public TextColorConfig Background { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public TextAlignment? TextAlignment { get; set; }

        public object Clone()
        {
            return new ColumnHeaderStyle()
            {
                Foreground = (TextColorConfig)this.Foreground?.Clone(),
                Background = (TextColorConfig)this.Background?.Clone(),
                HorizontalAlignment = this.HorizontalAlignment,
                TextAlignment = this.TextAlignment,
            };
        }
    }
}
