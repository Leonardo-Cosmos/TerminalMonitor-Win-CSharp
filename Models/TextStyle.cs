/* 2021/5/22 */
using System;
using System.Windows;

namespace TerminalMonitor.Models
{
    public class TextStyle : ICloneable
    {
        public static TextStyle Empty => new()
        {
        };

        public TextColorConfig Foreground { get; set; }

        public TextColorConfig Background { get; set; }

        public TextColorConfig CellBackground { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public VerticalAlignment? VerticalAlignment { get; set; }

        public TextAlignment? TextAlignment { get; set; }

        public object Clone()
        {
            return new TextStyle()
            {
                Foreground = this.Foreground,
                Background = this.Background,
                CellBackground = this.CellBackground,
                HorizontalAlignment = this.HorizontalAlignment,
                VerticalAlignment = this.VerticalAlignment,
                TextAlignment = this.TextAlignment,
            };
        }
    }
}
