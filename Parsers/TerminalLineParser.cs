/* 2021/6/22 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.Controls;

namespace TerminalMonitor.Parsers
{
    static class TerminalLineParser
    {
        private const string keySeparator = ".";

        public static TerminalLine ParseTerminalLine(string text, string execution)
        {
            var id = Guid.NewGuid().ToString();
            var timestamp = DateTime.Now;

            Dictionary<string, object> jsonDict;
            Dictionary<string, object> jsonProperties;
            var trimmedText = text.Trim();
            if ((trimmedText.StartsWith('{') && trimmedText.EndsWith('}')) ||
                (trimmedText.StartsWith('[') && trimmedText.EndsWith(']')))
            {
                jsonDict = JsonParser.ParseTerminalLine(text);
                jsonProperties = JsonParser.FlattenJsonPath(jsonDict);
            }
            else
            {
                jsonDict = [];
                jsonProperties = [];
            }

            Dictionary<string, object> systemFieldDict = new()
            {
                { "id", id },
                { "timestamp", timestamp },
                { "execution", execution },
                { "plainText", text },
            };

            Dictionary<string, TerminalLineField> lineFields = [];
            MergeKeyValuePairs(lineFields, systemFieldDict, "system");
            MergeKeyValuePairs(lineFields, jsonProperties, "json");

            return new()
            {
                Id = id,
                Timestamp = timestamp,
                PlainText = text,
                JsonObjectDict = jsonDict,
                LineFieldDict = lineFields,
            };
        }

        private static void MergeKeyValuePairs(Dictionary<string, TerminalLineField> unionDict,
            Dictionary<string, object> partialDict, string keyPrefix)
        {
            foreach (var kvPair in partialDict!)
            {
                var mergedFieldKey = $"{keyPrefix}{keySeparator}{kvPair.Key}";
                unionDict.Add(mergedFieldKey, new TerminalLineField()
                {
                    Key = kvPair.Key,
                    FieldKey = mergedFieldKey,
                    Value = kvPair.Value,
                    Text = ToValueString(kvPair.Value),
                });
            }
        }

        private static string ToValueString(object? value)
        {
            return value?.ToString() ?? "%null%";
        }
    }
}
