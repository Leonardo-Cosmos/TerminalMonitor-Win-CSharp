/* 2021/5/9 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    class CommandExecutor
    {
        private readonly List<string> executionNames = new();

        private readonly Dictionary<string, Execution> executionDict = new();

        private readonly ConcurrentQueue<string> terminalTextQueue = new();

        public CommandExecutor()
        {
            
        }

        public void Execute(CommandConfig commandConfig)
        {
            Execution execution = new(commandConfig);
            var name = GetValidExecutionName(commandConfig.Name);
            executionNames.Add(name);
            executionDict.Add(name, execution);

            
        }

        private string GetValidExecutionName(string configName)
        {
            if (!executionDict.ContainsKey(configName))
            {
                return configName;
            }

            int number = 0;
            string name;
            do
            {
                number++;
                name = $"{configName} {number}";
            } while (executionDict.ContainsKey(name));

            return name;
        }

        public IEnumerable<string> ReadTerminalLines()
        {
            List<string> lines = new();
            while (!terminalTextQueue.IsEmpty)
            {
                if (terminalTextQueue.TryDequeue(out var line))
                {
                    lines.Add(line);
                }
            }
            return lines.AsEnumerable();
        }

    }
}
