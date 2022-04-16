/* 2021/10/11 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TerminalMonitor.Settings.Models
{
    static class TextAlignmentSettings
    {
        private static readonly IReadOnlyDictionary<string, TextAlignment> textAlignmentDict
               = InitTextAlignmentDict();

        private static IReadOnlyDictionary<string, TextAlignment> InitTextAlignmentDict()
        {
            Dictionary<string, TextAlignment> dict = new();
            var values = Enum.GetValues(typeof(TextAlignment));
            foreach (var item in values)
            {
                if (item is TextAlignment value)
                {
                    dict.Add(value.ToString(), value);
                }
            }

            return new ReadOnlyDictionary<string, TextAlignment>(dict);
        }

        static string TextAlignmentToString(TextAlignment textAlignment) => textAlignment.ToString();

        static TextAlignment StringToTextAlignment(string str)
        {
            if (str != null && textAlignmentDict.ContainsKey(str))
            {
                return textAlignmentDict[str];
            }
            else
            {
                return TextAlignment.Left;
            }
        }

        public static string Save(TextAlignment? obj)
        {
            if (obj.HasValue)
            {
                return TextAlignmentToString(obj.Value);
            }
            else
            {
                return null;
            }
        }

        public static TextAlignment? Load(string setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToTextAlignment(setting);
        }
    }
}
