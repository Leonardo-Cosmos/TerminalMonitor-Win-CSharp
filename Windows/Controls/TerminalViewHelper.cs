/* 2021/10/10 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.Converters;

namespace TerminalMonitor.Windows.Controls
{
    static class TerminalViewHelper
    {
        public record RoutedEventHandlerRecord(RoutedEvent RoutedEvent, Delegate Handler);

        private static readonly IntToHorizontalAlignmentConverter horizontalAlignmentConverter = new();

        private static readonly IntToVerticalAlignmentConverter verticalAlignmentConverter = new();

        private static readonly IntToTextAlignmentConverter textAlignmentConverter = new();

        private static object ConvertColorToBrush(object color) => new SolidColorBrush((Color)color);

        private static Color GenerateColorByHash(object value)
        {
            int hashCode = value?.GetHashCode() ?? 0;

            byte red = (byte)hashCode;
            byte green = (byte)(hashCode >> 8);
            byte blue = (byte)(hashCode >> 16);

            return Color.FromRgb(red, green, blue);
        }

        private static Color ToInvertedColor(Color color)
        {
            byte red = color.R;
            byte green = color.G;
            byte blue = color.B;

            red ^= 0xFF;
            green ^= 0xFF;
            blue ^= 0xFF;

            return Color.FromRgb(red, green, blue);
        }

        private static Color ToSymmetricColor(Color color)
        {
            byte red = color.R;
            byte green = color.G;
            byte blue = color.B;

            red -= 0x80;
            green -= 0x80;
            blue -= 0x80;

            return Color.FromRgb(red, green, blue);
        }

        private static object ConvertTextColorToBrush(object textColor, object value)
        {
            object color = ((TextColorConfig)textColor).Mode switch
            {
                TextColorMode.Static => ((TextColorConfig)textColor).Color,
                TextColorMode.Hash => GenerateColorByHash(value),
                TextColorMode.HashInverted => ToInvertedColor(GenerateColorByHash(value)),
                TextColorMode.HashSymmetric => ToSymmetricColor(GenerateColorByHash(value)),
                _ => null,
            };
            return ConvertColorToBrush(color);
        }

        public static DataTemplate BuildFieldDataTemplate(FieldDisplayDetail visibleField, DataTable terminalDataTable,
            IEnumerable<RoutedEventHandlerRecord> eventHandlerRecords)
        {
            string fieldId = visibleField.Id;
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(Brush), GetForegroundColumnName);
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(Brush), GetBackgroundColumnName);
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(Brush), GetCellBackgroundColumnName);
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(HorizontalAlignment), GetHorizontalAlignmentColumnName);
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(VerticalAlignment), GetVertialAlignmentColumnName);
            SetStyleDataColumn(fieldId, terminalDataTable.Columns, typeof(TextAlignment), GetTextAlignmentColumnName);

            FrameworkElementFactory textBlockElement = new(typeof(TextBlock));

            Binding textBinding = new();
            textBinding.Path = new PropertyPath(visibleField.Id, Array.Empty<object>());
            textBlockElement.SetBinding(TextBlock.TextProperty, textBinding);

            SetElementStyleProperty(visibleField, textBlockElement, TextBlock.ForegroundProperty,
                null, ConvertColorToBrush,
                textStyle => textStyle.Foreground?.Color, GetForegroundColumnName);

            SetElementStyleProperty(visibleField, textBlockElement, TextBlock.BackgroundProperty,
                null, ConvertColorToBrush,
                textStyle => textStyle.Background?.Color, GetBackgroundColumnName);

            SetElementStyleProperty(visibleField, textBlockElement, TextBlock.HorizontalAlignmentProperty,
                horizontalAlignmentConverter, null,
                textStyle => textStyle.HorizontalAlignment, GetHorizontalAlignmentColumnName);

            SetElementStyleProperty(visibleField, textBlockElement, TextBlock.VerticalAlignmentProperty,
                verticalAlignmentConverter, null,
                textStyle => textStyle.VerticalAlignment, GetVertialAlignmentColumnName);

            SetElementStyleProperty(visibleField, textBlockElement, TextBlock.TextAlignmentProperty,
                textAlignmentConverter, null,
                textStyle => textStyle.TextAlignment, GetTextAlignmentColumnName);

            FrameworkElementFactory panelElement = new(typeof(DockPanel));

            SetElementStyleProperty(visibleField, panelElement, Panel.BackgroundProperty,
                null, ConvertColorToBrush,
                textStyle => textStyle.CellBackground?.Color, GetCellBackgroundColumnName);

            if (eventHandlerRecords != null)
            {
                foreach (var eventHandlerRecord in eventHandlerRecords)
                {
                    panelElement.AddHandler(eventHandlerRecord.RoutedEvent, eventHandlerRecord.Handler);
                }
            }

            panelElement.AppendChild(textBlockElement);

            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = panelElement;
            return dataTemplate;
        }

        private static string GetForegroundColumnName(string columnName)
        {
            return $"{columnName!}__Foreground";
        }

        private static string GetBackgroundColumnName(string columnName)
        {
            return $"{columnName!}__Background";
        }

        private static string GetCellBackgroundColumnName(string columnName)
        {
            return $"{columnName!}__CellBackground";
        }

        private static string GetHorizontalAlignmentColumnName(string columnName)
        {
            return $"{columnName!}__HorizontalAlignment";
        }

        private static string GetVertialAlignmentColumnName(string columnName)
        {
            return $"{columnName!}__VerticalAlignment";
        }

        private static string GetTextAlignmentColumnName(string columnName)
        {
            return $"{columnName!}__TextAlignment";
        }

        private static void SetStyleDataColumn(string fieldId, DataColumnCollection columns, Type dataType,
            Func<string, string> getStyleColumnName)
        {
            DataColumn styleColumn = new(getStyleColumnName(fieldId));
            styleColumn.DataType = dataType;
            columns.Add(styleColumn);
        }

        private static void SetElementStyleProperty(FieldDisplayDetail visibleField, FrameworkElementFactory elementFactory,
            DependencyProperty dependencyProperty, IValueConverter bindingConverter, Func<object, object> convertToValue,
            Func<TextStyle, object> getStyleProperty, Func<string, string> getStyleColumnName)
        {
            object staticStyleProperty = GetStaticStyleProperty(visibleField, getStyleProperty);
            if (staticStyleProperty == null)
            {
                Binding binding = new();
                binding.Path = new PropertyPath(getStyleColumnName(visibleField.Id), Array.Empty<object>());
                if (bindingConverter != null)
                {
                    binding.Converter = bindingConverter;
                }
                elementFactory.SetBinding(dependencyProperty, binding);
            }
            else
            {
                if (convertToValue == null)
                {
                    elementFactory.SetValue(dependencyProperty, staticStyleProperty);
                }
                else
                {
                    elementFactory.SetValue(dependencyProperty, convertToValue(staticStyleProperty));
                }
            }
        }

        private static object GetStaticStyleProperty(FieldDisplayDetail visibleField,
            Func<TextStyle, object> getStyleProperty)
        {
            object defaultStyle = getStyleProperty(visibleField.Style);
            if (defaultStyle == null)
            {
                return null;
            }

            bool anyConditionalStyle = (visibleField.Conditions ?? Array.Empty<TextStyleCondition>())
                .Select(textStyleCondition => textStyleCondition.Style)
                .Any(style => getStyleProperty(style) != null);

            if (anyConditionalStyle)
            {
                return null;
            }
            else
            {
                return defaultStyle;
            }
        }

        public static void BuildDataRowStyleCells(DataRow row, FieldDisplayDetail visibleField,
            TextStyle matchedTextStyle)
        {
            var fieldId = visibleField.Id;
            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.Foreground, GetForegroundColumnName, row,
                textColor => ConvertTextColorToBrush(textColor, row[fieldId]));
            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.Background, GetBackgroundColumnName, row,
                textColor => ConvertTextColorToBrush(textColor, row[fieldId]));
            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.CellBackground, GetCellBackgroundColumnName, row,
                textColor => ConvertTextColorToBrush(textColor, row[fieldId]));

            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.HorizontalAlignment, GetHorizontalAlignmentColumnName, row, null);
            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.VerticalAlignment, GetVertialAlignmentColumnName, row, null);
            BuildFinalStyleCell(fieldId, visibleField.Style, matchedTextStyle,
                textStyle => textStyle.TextAlignment, GetTextAlignmentColumnName, row, null);
        }

        private static void BuildFinalStyleCell(string fieldId, TextStyle defaultStyle, TextStyle conditionalStyle,
            Func<TextStyle, object> getStyleProperty, Func<string, string> getStyleColumnName, DataRow dataRow,
            Func<object, object> convert)
        {
            object finalStyleProperty;
            object defaultStyleProperty = getStyleProperty(defaultStyle);
            if (conditionalStyle == null)
            {
                finalStyleProperty = defaultStyleProperty;
            }
            else
            {
                object conditionalStyleProperty = getStyleProperty(conditionalStyle);
                if (conditionalStyleProperty == null)
                {
                    finalStyleProperty = defaultStyleProperty;
                }
                else
                {
                    finalStyleProperty = conditionalStyleProperty;
                }
            }

            if (finalStyleProperty == null)
            {
                return;
            }

            if (convert == null)
            {
                dataRow[getStyleColumnName(fieldId)] = finalStyleProperty;
            }
            else
            {
                dataRow[getStyleColumnName(fieldId)] = convert(finalStyleProperty);
            }
        }

        public static DataTemplate BuildColumnHeaderTemplate(FieldDisplayDetail visibleField)
        {
            string columnHeader = visibleField.HeaderName ?? visibleField.FieldKey;

            FrameworkElementFactory textBlockElement = new(typeof(TextBlock));
            textBlockElement.SetValue(TextBlock.TextProperty, columnHeader);

            var headerStyle = visibleField.HeaderStyle;

            if (headerStyle.Foreground != null)
            {
                textBlockElement.SetValue(TextBlock.ForegroundProperty, ConvertColorToBrush(headerStyle.Foreground));
            }

            if (headerStyle.Background != null)
            {
                textBlockElement.SetValue(TextBlock.BackgroundProperty, ConvertColorToBrush(headerStyle.Background));
            }

            if (headerStyle.HorizontalAlignment != null)
            {
                textBlockElement.SetValue(TextBlock.HorizontalAlignmentProperty, headerStyle.HorizontalAlignment);
            }

            if (headerStyle.VerticalAlignment != null)
            {
                textBlockElement.SetValue(TextBlock.VerticalAlignmentProperty, headerStyle.VerticalAlignment);
            }

            if (headerStyle.TextAlignment != null)
            {
                textBlockElement.SetValue(TextBlock.TextAlignmentProperty, headerStyle.TextAlignment);
            }

            FrameworkElementFactory panelElement = new(typeof(DockPanel));
            if (headerStyle.CellBackground != null)
            {
                panelElement.SetValue(Panel.BackgroundProperty, ConvertColorToBrush(headerStyle.CellBackground));
            }

            panelElement.AppendChild(textBlockElement);

            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = panelElement;
            return dataTemplate;
        }

        public static DataTemplate BuildDefaultColumnHeaderTemplate(FieldDisplayDetail visibleField)
        {
            string columnHeader = visibleField.HeaderName ?? visibleField.FieldKey;

            FrameworkElementFactory textBlockElement = new(typeof(TextBlock));
            textBlockElement.SetValue(TextBlock.TextProperty, columnHeader);
            textBlockElement.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

            FrameworkElementFactory panelElement = new(typeof(DockPanel));
            panelElement.AppendChild(textBlockElement);

            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = panelElement;
            return dataTemplate;
        }
    }
}
