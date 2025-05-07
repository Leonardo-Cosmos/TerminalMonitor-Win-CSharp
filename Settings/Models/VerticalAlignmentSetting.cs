/* 2021/10/11 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TerminalMonitor.Settings.Models
{
    static class VerticalAlignmentSettings
    {
        private static readonly IReadOnlyDictionary<string, VerticalAlignment> verticalAlignmentDict
               = InitVerticalAlignmentDict();

        private static IReadOnlyDictionary<string, VerticalAlignment> InitVerticalAlignmentDict()
        {
            Dictionary<string, VerticalAlignment> dict = new();
            var values = Enum.GetValues(typeof(VerticalAlignment));
            foreach (var item in values)
            {
                if (item is VerticalAlignment value)
                {
                    dict.Add(value.ToString(), value);
                }
            }

            return new ReadOnlyDictionary<string, VerticalAlignment>(dict);
        }

        static string VerticalAlignmentToString(VerticalAlignment verticalAlignment) => verticalAlignment.ToString();

        static VerticalAlignment StringToVerticalAlignment(string str)
        {
            if (str != null && verticalAlignmentDict.ContainsKey(str))
            {
                return verticalAlignmentDict[str];
            }
            else
            {
                return VerticalAlignment.Top;
            }
        }

        public static string? Save(VerticalAlignment? obj)
        {
            if (obj.HasValue)
            {
                return VerticalAlignmentToString(obj.Value);
            }
            else
            {
                return null;
            }
        }

        public static VerticalAlignment? Load(string? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToVerticalAlignment(setting);
        }
    }
}
