/* 2021/6/8 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    class Execution
    {
        private bool started = false;

        private readonly object startLock = new();

        private Process? process;

        public Guid Id { get; init; }

        public CommandConfig CommandConfig { get; init; }

        public bool IsCompleted { get; private set; }

        internal Execution(CommandConfig commandConfig)
        {
            Id = Guid.NewGuid();
            CommandConfig = commandConfig;
            IsCompleted = false;
        }

        public Task Start()
        {
            Debug.WriteLine($"Execution (id: {Id}) is starting");

            Monitor.Enter(startLock);
            if (started)
            {
                Monitor.Exit(startLock);
                throw new InvalidOperationException("It has been started already.");
            }

            started = true;

            return Task.Run(() => {
                try
                {
                    Start(CommandConfig.StartFile,
                        CommandConfig.Arguments,
                        CommandConfig.WorkDirectory);
                }
                catch (Exception ex)
                {
                    OnExited(ex);
                }
                finally
                {
                    Monitor.Exit(startLock);
                }
            });
        }

        public Task Kill()
        {
            Debug.WriteLine($"Execution (id: {Id}) is terminating");

            Monitor.Enter(startLock);
            if (!started)
            {
                Monitor.Exit(startLock);
                throw new InvalidOperationException("It is not running.");
            }
            Monitor.Exit(startLock);

            return Task.Run(() =>
            {
                process?.Kill();
            });
        }

        private void Start(string? startFile, string? arguments = null, string? workingDirectory = null)
        {
            if (String.IsNullOrEmpty(startFile))
            {
                OnExited(null);
                return;
            }

            process = new Process();
            process.StartInfo.FileName = startFile;
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
                    OnLineReceived(e.Data);
                }
            };

            process.StartInfo.RedirectStandardError = true;
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    OnLineReceived(e.Data);
                }
            };

            process.Exited += (sender, e) =>
            {
                process.Close();

                OnExited(null);
            };

            process.StartInfo.CreateNoWindow = true;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Debug.WriteLine($"Execution (id: {Id}) is started");

            //await process.WaitForExitAsync();
        }

        protected void OnLineReceived(string line)
        {
            ProcessOutputEventArgs e = new()
            {
                Text = line,
            };
            LineReceived?.Invoke(this, e);
        }

        protected void OnExited(Exception? exception)
        {
            process = null;

            ProcessExitedEventArgs e = new()
            {
                Exception = exception,
            };
            Exited?.Invoke(this, e);

            IsCompleted = true;
        }

        public event ProcessOutputEventHandler? LineReceived;

        public event ProcessExitedEventHandler? Exited;
    }
}
