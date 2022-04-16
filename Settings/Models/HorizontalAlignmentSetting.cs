/* 2021/10/11 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TerminalMonitor.Settings.Models
{
    static class HorizontalAlignmentSettings
    {
        private static readonly IReadOnlyDictionary<string, HorizontalAlignment> horizontalAlignmentDict
            = InitHorizontalAlignmentDict();

        private static IReadOnlyDictionary<string, HorizontalAlignment> InitHorizontalAlignmentDict()
        {
            Dictionary<string, HorizontalAlignment> dict = new();
            var values = Enum.GetValues(typeof(HorizontalAlignment));
            foreach (var item in values)
            {
                if (item is HorizontalAlignment value)
                {
                    dict.Add(value.ToString(), value);
                }
            }

            return new ReadOnlyDictionary<string, HorizontalAlignment>(dict);
        }

        static string HorizontalAlignmentToString(HorizontalAlignment horizontalAlignment) => horizontalAlignment.ToString();

        static HorizontalAlignment StringToHorizontalAlignment(string str)
        {
            if (str != null && horizontalAlignmentDict.ContainsKey(str))
            {
                return horizontalAlignmentDict[str];
            }
            else
            {
                return HorizontalAlignment.Left;
            }
        }

        public static string Save(HorizontalAlignment? obj)
        {
            if (obj.HasValue)
            {
                return HorizontalAlignmentToString(obj.Value);
            }
            else
            {
                return null;
            }
        }

        public static HorizontalAlignment? Load(string setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToHorizontalAlignment(setting);
        }
    }
}
