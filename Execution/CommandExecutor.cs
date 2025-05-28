/* 2021/5/9 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;
using TerminalMonitor.Parsers;
using static TerminalMonitor.Execution.ITerminalLineProducer;

namespace TerminalMonitor.Execution
{
    class CommandExecutor : IExecutor, ITerminalLineProducer
    {
        private record ExecutionText(string Text, string ExecutionName);

        /// <summary>
        /// A list of names of running executions.
        /// </summary>
        private readonly List<string> executionNames = [];

        /// <summary>
        /// A dictionary from execution name to execution detail.
        /// </summary>
        private readonly Dictionary<string, Execution> executionDict = [];

        private readonly BlockingCollection<ExecutionText> executionTextCollection = [];

        private readonly ConcurrentQueue<TerminalLine> terminalLineQueue = new();

        public CommandExecutor()
        {
            _ = Task.Run(ParseTerminalLine);
        }

        public void Execute(CommandConfig commandConfig)
        {
            Execution execution = new(commandConfig);
            var uniqueExecutionName = GetUniqueExecutionName(commandConfig.Name);

            execution.LineReceived += (sender, e) =>
            {
                ExecutionText executionText = new(Text: e.Text, ExecutionName: uniqueExecutionName);
                executionTextCollection.Add(executionText);
            };

            execution.Exited += (sender, e) =>
            {
                RemoveExecution(uniqueExecutionName, execution.Id, e.Exception);

                Debug.Print($"Execution {uniqueExecutionName} completed.");
            };

            AddExecution(uniqueExecutionName, execution);

            execution.Start();

            Debug.Print($"Execution {uniqueExecutionName} is started");
        }

        public void Terminate(string executionName)
        {
            if (!executionDict.TryGetValue(executionName, out Execution? execution))
            {
                Debug.Print($"Execution {executionName} doesn't exist when terminate it.");
                return;
            }

            try
            {
                execution.Kill();
            }
            catch (Exception ex)
            {
                RemoveExecution(executionName, execution.Id, ex);
            }
        }

        public void TerminateAll(Guid commandId)
        {

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

        public void Shutdown()
        {
            TerminateAll();
            executionTextCollection.CompleteAdding();
        }

        public IEnumerable<TerminalLine> ReadTerminalLines()
        {
            List<TerminalLine> terminalLines = [];
            while (!terminalLineQueue.IsEmpty)
            {
                if (terminalLineQueue.TryDequeue(out var terminalLine))
                {
                    terminalLines.Add(terminalLine);
                }
            }
            return terminalLines.AsEnumerable();
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

            OnExecutionStarted(name, execution.Id);
        }

        private void RemoveExecution(string name, string id, Exception? exception = null)
        {
            executionNames.Remove(name);
            executionDict.Remove(name);

            OnExecutionExited(name, id, exception);

            // Emit event when the last execution ended.
            if (executionNames.Count == 0)
            {
                OnCompleted();
            }
        }

        /// <summary>
        /// By default, the execution takes command's name,
        /// but when there are multiple executions from same command,
        /// each execution should have a unique name.
        /// </summary>
        private string GetUniqueExecutionName(string configName)
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

        private void ParseTerminalLine()
        {
            while (true)
            {
                ExecutionText executionText;
                try
                {
                    executionText = executionTextCollection.Take();
                }
                catch (InvalidOperationException)
                {
                    Debug.WriteLine("Parse task done");
                    break;
                }

                TerminalLine terminalLineDto =
                    TerminalLineParser.ParseTerminalLine(executionText.Text, executionText.ExecutionName);
                terminalLineQueue.Enqueue(terminalLineDto);
            }
        }

        protected void OnExecutionStarted(string name, string id)
        {
            ExecutionInfoEventArgs e = new()
            {
                Execution = new()
                {
                    Id = id,
                    Name = name,
                    Status = ExecutionStatus.Started,
                },
            };
            ExecutionStarted?.Invoke(this, e);
        }

        protected void OnExecutionExited(string name, string id, Exception? exception)
        {
            var status = exception == null ? ExecutionStatus.Completed : ExecutionStatus.Error;
            ExecutionInfoEventArgs e = new()
            {
                Execution = new()
                {
                    Id = id,
                    Name = name,
                    Status = status,
                },

                Exception = exception,
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

        public event ExecutionInfoEventHandler? ExecutionStarted;

        public event ExecutionInfoEventHandler? ExecutionExited;

        public event EventHandler? Started;

        public event EventHandler? Completed;

        public bool IsCompleted
        {
            get;
            private set;
        }
    }
}
