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
        public static TerminalLineDto ParseTerminalLine(string text, string execution)
        {
            var id = Guid.NewGuid().ToString();
            var timestamp = DateTime.Now;

            var jsonDict = JsonParser.ParseTerminalLine(text);
            var jsonProperties = JsonParser.FlattenJsonPath(jsonDict);

            Dictionary<string, object> systemFieldDict = new()
            {
                { "id", id },
                { "timestamp", timestamp },
                { "execution", execution },
                { "plainText", text },
            };

            Dictionary<string, TerminalLineFieldDto> lineFields = new();
            MergeKeyValuePairs(lineFields, systemFieldDict, "system.");
            MergeKeyValuePairs(lineFields, jsonProperties, "json.");

            return new()
            {
                Id = id,
                Timestamp = timestamp,
                PlainText = text,
                JsonObjectDict = jsonDict,
                LineFieldDict = lineFields,
            };
        }

        private static void MergeKeyValuePairs(Dictionary<string, TerminalLineFieldDto> unionDict, Dictionary<string, object> partialDict, string keyPrefix)
        {
            foreach (var kvPair in partialDict!)
            {
                unionDict!.Add($"{keyPrefix!}{kvPair.Key}", new TerminalLineFieldDto()
                {
                    Key = kvPair.Key,
                    FieldKey = $"{keyPrefix!}{kvPair.Key}",
                    Value = kvPair.Value,
                    Text = kvPair.Value.ToString(),
                });
            }
        }
    }
}
