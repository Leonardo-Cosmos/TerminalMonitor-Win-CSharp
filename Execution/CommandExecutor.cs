/* 2021/5/9 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;
using static TerminalMonitor.Execution.ITerminalLineProducer;

namespace TerminalMonitor.Execution
{
    class CommandExecutor : IExecutor, ITerminalLineProducer
    {
        private readonly List<string> executionNames = new();

        private readonly Dictionary<string, Execution> executionDict = new();

        private readonly ConcurrentQueue<TerminalLine> terminalLineQueue = new();

        public CommandExecutor()
        {

        }

        public void Execute(CommandConfig commandConfig)
        {
            Execution execution = new(commandConfig);
            var name = GetValidExecutionName(commandConfig.Name);

            execution.LineReceived += (sender, e) =>
            {
                TerminalLine terminalLine = new(Text: e.Line, ExecutionName: name);
                terminalLineQueue.Enqueue(terminalLine);
            };

            execution.Exited += (sender, e) =>
            {
                RemoveExecution(name);

                Debug.Print($"Executor {name} completed.");
            };

            execution.Start();

            Debug.Print($"Executor {name} is started");
            AddExecution(name, execution);
        }

        public void Terminate(string executionName)
        {
            if (!executionDict.ContainsKey(executionName))
            {
                Debug.Print($"Executor {executionName} doesn't exist when terminate.");
                return;
            }

            var execution = executionDict[executionName];
            execution.Kill();

            RemoveExecution(executionName);
        }

        public void TerminateAll()
        {
            foreach (var executionName in executionNames)
            {
                var execution = executionDict[executionName];
                execution.Kill();
            }

            executionNames.Clear();
            executionDict.Clear();
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
            // Emit event when the first execution started.
            if (executionNames.Count == 0)
            {
                OnStarted();
            }

            executionNames.Add(name);
            executionDict.Add(name, execution);

            OnExecutionStarted(name);
        }

        private void RemoveExecution(string name)
        {
            executionNames.Remove(name);
            executionDict.Remove(name);

            OnExecutionExited(name);

            // Emit event when the last execution ended.
            if (executionNames.Count == 0)
            {
                OnCompleted();
            }
        }

        protected void OnExecutionStarted(string name)
        {
            ExecutionInfoEventArgs e = new()
            {
                Execution = new()
                {
                    Name = name,
                },
            };
            ExecutionStarted?.Invoke(this, e);
        }

        protected void OnExecutionExited(string name)
        {
            ExecutionInfoEventArgs e = new()
            {
                Execution = new()
                {
                    Name = name,
                },
            };
            ExecutionExited?.Invoke(this, e);
        }

        protected void OnStarted()
        {
            IsCompleted = false;
            Started?.Invoke(this, EventArgs.Empty);
        }

        protected void OnCompleted()
        {
            IsCompleted = true;
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public event ExecutionInfoEventHandler ExecutionStarted;

        public event ExecutionInfoEventHandler ExecutionExited;

        public event EventHandler Started;

        public event EventHandler Completed;

        public IEnumerable<TerminalLine> ReadTerminalLines()
        {
            List<TerminalLine> terminalLines = new();
            while (!terminalLineQueue.IsEmpty)
            {
                if (terminalLineQueue.TryDequeue(out var terminalLine))
                {
                    terminalLines.Add(terminalLine);
                }
            }
            return terminalLines.AsEnumerable();
        }

        public bool IsCompleted
        {
            get;
            private set;
        }
    }
}
