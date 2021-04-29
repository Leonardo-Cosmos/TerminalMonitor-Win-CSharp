/* 2021/4/20 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Windows;

namespace TerminalMonitor.Parsers
{
    static class JsonParser
    {

        public static TerminalTextVO ParseTerminalLine(string text)
        {
            List<TerminalTextFieldVO> parsedFields;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                parsedFields = dict.OrderBy(kvPair => kvPair.Key)
                    .Select(kvPair => new TerminalTextFieldVO()
                    {
                        Key = kvPair.Key,
                        Value = kvPair.Value.ToString(),
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                parsedFields = new();
            }

            return new TerminalTextVO()
            {
                PlainText = text,
                ParsedFields = parsedFields,
            };
        }

    }
}
