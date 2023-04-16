/* 2023/2/3 */
using System;
using System.Windows;
using System.Windows.Media;

namespace TerminalMonitor.Models
{
    public class ColumnHeaderStyle : ICloneable
    {
        public static ColumnHeaderStyle Empty => new()
        {
        };

        public Color? Foreground { get; set; }

        public Color? Background { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public VerticalAlignment? VerticalAlignment { get; set; }

        public TextAlignment? TextAlignment { get; set; }

        public object Clone()
        {
            return new ColumnHeaderStyle()
            {
                Foreground = this.Foreground,
                Background = this.Background,
                HorizontalAlignment = this.HorizontalAlignment,
                VerticalAlignment = this.VerticalAlignment,
                TextAlignment = this.TextAlignment,
            };
        }
    }
}
