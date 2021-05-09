/* 2021/5/9 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Execution
{
    class CommandExecutor
    {
        private readonly string command;

        private readonly string arguments = null;

        private readonly string workingDirectory = null;

        private readonly ConcurrentQueue<string> terminalTextQueue = new();

        private bool started = false;

        private class Execution : IExecution
        {
            private readonly CommandExecutor executor;

            private readonly Task processTask;

            internal Execution(CommandExecutor executor, Task processTask)
            {
                this.executor = executor;
                this.processTask = processTask;
            }

            public IEnumerable<string> ReadTerminalLines()
            {
                return executor.ReadTerminalLines();
            }

            public bool IsCompleted
            {
                get
                {
                    return processTask.IsCompleted;
                }
            }
        }

        public CommandExecutor(string command, string arguments = null, string workingDirectory = null)
        {
            this.command = command;
            this.arguments = arguments;
            this.workingDirectory = workingDirectory;
        }

        public IExecution Execute()
        {
            if (started)
            {
                throw new InvalidOperationException("It has been started already.");
            }

            var task = Start();
            started = true;
            return new Execution(this, task);
        }

        private async Task Start()
        {
            if (!String.IsNullOrEmpty(command))
            {
                var process = new Process();
                process.StartInfo.FileName = command;
                if (!String.IsNullOrWhiteSpace(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                if (!String.IsNullOrWhiteSpace(workingDirectory))
                {
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) =>
                {

                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        terminalTextQueue.Enqueue(e.Data);
                    }

                };

                process.Start();

                process.BeginOutputReadLine();

                await process.WaitForExitAsync();
                process.Close();
            }
        }

        IEnumerable<string> ReadTerminalLines()
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
