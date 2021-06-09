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
    class CommandExecutor : ITerminalLineProducer
    {
        private readonly List<string> executionNames = new();

        private readonly Dictionary<string, Execution> executionDict = new();

        private readonly ConcurrentQueue<string> terminalLineQueue = new();

        public CommandExecutor()
        {

        }

        public void Execute(CommandConfig commandConfig)
        {
            if (executionNames.Count == 0)
            {
                OnStarted(EventArgs.Empty);
            }

            Execution execution = new(commandConfig);
            var name = GetValidExecutionName(commandConfig.Name);
            AddExecution(name, execution);

            execution.LineReceived += (sender, e) =>
            {
                terminalLineQueue.Enqueue(e.Line);
            };

            execution.Exited += (sender, e) =>
            {
                RemoveExecution(name);

                Debug.Print($"Executor {name} completed.");

                if (executionNames.Count == 0)
                {
                    OnCompleted(EventArgs.Empty);
                }
            };

            Debug.Print($"Executor {name} is started");
            execution.Execute();
        }

        private string GetValidExecutionName(string configName)
        {
            if (!executionDict.ContainsKey(configName))
            {
                return configName;
            }

            var number = 0;
            string name;
            do
            {
                number++;
                name = $"{configName} {number}";
            } while (executionDict.ContainsKey(name));

            return name;
        }

        private void AddExecution(string name, Execution execution)
        {
            executionNames.Add(name);
            executionDict.Add(name, execution);
        }

        private void RemoveExecution(string name)
        {
            executionNames.Remove(name);
            executionDict.Remove(name);
        }

        protected void OnStarted(EventArgs e)
        {
            IsCompleted = false;
            Started?.Invoke(this, e);
        }

        protected void OnCompleted(EventArgs e)
        {
            IsCompleted = true;
            Completed?.Invoke(this, e);
        }

        public event EventHandler Started;

        public event EventHandler Completed;

        public IEnumerable<string> ReadTerminalLines()
        {
            List<string> lines = new();
            while (!terminalLineQueue.IsEmpty)
            {
                if (terminalLineQueue.TryDequeue(out var line))
                {
                    lines.Add(line);
                }
            }
            return lines.AsEnumerable();
        }
        public bool IsCompleted
        {
            get;
            private set;
        }

    }
}
