/* 2021/5/9 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TerminalMonitor.Models;
using TerminalMonitor.Parsers;

namespace TerminalMonitor.Execution
{
    class CommandExecutor : IExecutor, ITerminalLineProducer
    {
        private record ExecutionText(string Text, string ExecutionName);

        private readonly CommandExecutorData executorData = new();

        private readonly BlockingCollection<ExecutionText> executionTextCollection = [];

        private readonly ConcurrentQueue<TerminalLine> terminalLineQueue = new();

        public CommandExecutor()
        {
            _ = Task.Run(ParseTerminalLine);
        }

        public Task? Execute(CommandConfig commandConfig)
        {
            Debug.WriteLine($"Executing command (name: {commandConfig.Name}, id: {commandConfig.Id})");

            Execution execution = new(commandConfig);

            var uniqueExecutionName = executorData.GetUniqueExecutionName(commandConfig.Name);

            execution.LineReceived += (sender, e) =>
            {
                ExecutionText executionText = new(Text: e.Text, ExecutionName: uniqueExecutionName);
                Task.Run(() =>
                {
                    executionTextCollection.Add(executionText);
                });
            };

            execution.Exited += (sender, e) =>
            {
                RemoveExecution(execution.Id, e.Exception);

                Debug.WriteLine($"Execution (name: {uniqueExecutionName}, id: {execution.Id}) completed.");
            };

            var added = AddExecution(uniqueExecutionName, execution);
            if (!added)
            {
                return null;
            }

            return Task.Run(() =>
            {
                try
                {
                    execution.Start().Wait();
                    Debug.WriteLine($"Execution (name: {uniqueExecutionName}, id: {execution.Id}) is started");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error when start execution (name: {uniqueExecutionName}, id: {execution.Id}). {ex}");
                    RemoveExecution(execution.Id, ex);
                }
            });
        }

        public void Terminate(string executionName)
        {
            if (!executionDict.TryGetValue(executionName, out Execution? execution))
            {
                Debug.WriteLine($"Execution {executionName} doesn't exist when terminate it.");
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

        private bool AddExecution(string executionName, Execution execution)
        {
            // Emit event when the first execution started.
            if (executorData.IsEmpty)
            {
                OnStarted();
            }

            var commandConfig = execution.CommandConfig;
            if (executorData.GetExecutionIds(commandConfig.Id) == null)
            {
                OnCommandFirstExecutionStarted(commandConfig.Name, commandConfig.Id);
            }

            try
            {
                executorData.AddExecution(executionName, execution);

                OnExecutionStarted(executionName, execution.Id);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when add execution (name: {executionName}, id: {execution.Id}). {ex}");
                return false;
            }
        }

        private bool RemoveExecution(Guid executionId, Exception? exception = null)
        {
            try
            {
                var (executionName, execution) = executorData.RemoveExecution(executionId);

                OnExecutionExited(executionName, executionId, exception);

                var commandConfig = execution.CommandConfig;
                if (executorData.GetExecutionIds(commandConfig.Id) == null)
                {
                    OnCommandLastExecutionExited(commandConfig.Name, commandConfig.Id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when remove execution (id: {executionId}). {ex}");
                return false;
            }


            // Emit event when the last execution ended.
            if (executorData.IsEmpty)
            {
                OnCompleted();
            }

            return true;
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

        protected void OnExecutionStarted(string name, Guid id)
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

        protected void OnExecutionExited(string name, Guid id, Exception? exception)
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

        protected void OnCommandFirstExecutionStarted(string name, Guid id)
        {
            CommandInfoEventArgs e = new()
            {
                Command = new()
                {
                    Id = id,
                    Name = name,
                },
            };
            CommandFirstExecutionStarted?.Invoke(this, e);
        }

        protected void OnCommandLastExecutionExited(string name, Guid id)
        {
            CommandInfoEventArgs e = new()
            {
                Command = new()
                {
                    Id = id,
                    Name = name,
                },
            };
            CommandLastExecutionExited?.Invoke(this, e);
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

        public event CommandInfoEventHandler? CommandFirstExecutionStarted;

        public event CommandInfoEventHandler? CommandLastExecutionExited;

        public event EventHandler? Started;

        public event EventHandler? Completed;

        public bool IsCompleted
        {
            get;
            private set;
        }
    }
}
