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
        private sealed record ExecutionText(string Text, string ExecutionName);

        private readonly CommandExecutorData executorData = new();

        private readonly BlockingCollection<ExecutionText> executionTextCollection = [];

        private readonly ConcurrentQueue<TerminalLine> terminalLineQueue = new();

        public CommandExecutor()
        {
            _ = Task.Run(ParseTerminalLine);
        }

        public Task Execute(CommandConfig commandConfig)
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
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                try
                {
                    await execution.Start();
                    Debug.WriteLine($"Execution (name: {uniqueExecutionName}, id: {execution.Id}) is started");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error when start execution (name: {uniqueExecutionName}, id: {execution.Id}). {ex}");
                    RemoveExecution(execution.Id, ex);
                }
            });
        }

        public Task Terminate(Guid executionId)
        {
            var executionName = executorData.GetExecutionName(executionId);
            var execution = executorData.GetExecution(executionId);
            if (executionName == null || execution == null)
            {
                Debug.WriteLine($"Cannot find detail of execution (id: {executionId})");
                return Task.CompletedTask;
            }

            Debug.WriteLine($"Terminating execution (name: {executionName}, id: {executionId})");

            return Task.Run(async () =>
            {
                try
                {
                    await execution.Kill();
                    Debug.WriteLine($"Execution (name: {executionName}, id: {executionId}) is terminated");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error when terminate execution (name: {executionName}, id: {executionId}). {ex}");
                    RemoveExecution(executionId, ex);
                }
            });
        }

        public Task Restart(Guid executionId)
        {
            var execution = executorData.GetExecution(executionId);
            if (execution == null)
            {
                Debug.WriteLine($"Cannot find detail of execution (id: {executionId})");
                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                await Terminate(executionId);
                await Execute(execution.CommandConfig);
            });
        }

        public Task TerminateAll(HashSet<Guid> executionIds)
        {
            List<Task> tasks = [];
            foreach (var executionId in executionIds)
            {
                tasks.Add(Terminate(executionId));
            }

            if (tasks.Count == 0)
            {
                return Task.CompletedTask;
            }

            return Task.WhenAll(tasks);
        }

        public Task TerminateAll(Guid commandId)
        {
            var executionIds = executorData.GetExecutionIds(commandId);
            if (executionIds == null)
            {
                Debug.WriteLine($"Cannot find execution set of command (id: {commandId})");
                return Task.CompletedTask;
            }

            return TerminateAll(executionIds);
        }

        public Task TerminateAll()
        {
            var executionIds = executorData.GetExecutionIds();
            return TerminateAll(executionIds);
        }

        public Task Shutdown()
        {
            return Task.Run(async () =>
            {
                await TerminateAll();
                executionTextCollection.CompleteAdding();
            });
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

        private void RemoveExecution(Guid executionId, Exception? exception = null)
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
                return;
            }


            // Emit event when the last execution ended.
            if (executorData.IsEmpty)
            {
                OnCompleted();
            }
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

                TerminalLine terminalLine =
                    TerminalLineParser.ParseTerminalLine(executionText.Text, executionText.ExecutionName);
                terminalLineQueue.Enqueue(terminalLine);
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
