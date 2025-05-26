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

        public TextColorConfig? Foreground { get; set; }

        public TextColorConfig? Background { get; set; }

        public TextColorConfig? CellBackground { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public VerticalAlignment? VerticalAlignment { get; set; }

        public TextAlignment? TextAlignment { get; set; }

        public double? MaxWidth { get; set; }

        public double? MaxHeight { get; set; }

        public TextWrapping? TextWrapping { get; set; }

        public object Clone()
        {
            return new TextStyle()
            {
                Foreground = this.Foreground?.Clone() as TextColorConfig,
                Background = this.Background?.Clone() as TextColorConfig,
                CellBackground = this.CellBackground?.Clone() as TextColorConfig,
                HorizontalAlignment = this.HorizontalAlignment,
                VerticalAlignment = this.VerticalAlignment,
                TextAlignment = this.TextAlignment,
                MaxWidth = this.MaxWidth,
                MaxHeight = this.MaxHeight,
                TextWrapping = this.TextWrapping,
            };
        }
    }
}
