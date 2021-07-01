/* 2021/6/8 */
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
    class Execution
    {
        private readonly CommandConfig commandConfig;

        private bool started = false;

        private Process process;

        private Task processTask;

        internal Execution(CommandConfig commandConfig)
        {
            this.commandConfig = commandConfig;
        }

        public void Start()
        {
            if (started)
            {
                throw new InvalidOperationException("It has been started already.");
            }

            processTask = Start(commandConfig.StartFile, commandConfig.Arguments, commandConfig.WorkDirectory);
            started = true;
        }

        public void Kill()
        {
            if (!started)
            {
                throw new InvalidOperationException("It is not running.");
            }

            process.Kill();
        }

        private async Task Start(string startFile, string arguments = null, string workingDirectory = null)
        {
            if (!String.IsNullOrEmpty(startFile))
            {
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

                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
                process.Close();
                OnExited();
            }
        }

        protected void OnLineReceived(string line)
        {
            TerminalLineEventArgs e = new()
            {
                Line = line,
            };
            LineReceived?.Invoke(this, e);
        }

        protected void OnExited()
        {
            Exited?.Invoke(this, EventArgs.Empty);
        }

        public event TerminalLineEventHandler LineReceived;

        public event EventHandler Exited;

        public bool IsCompleted
        {
            get
            {
                return processTask.IsCompleted;
            }
        }
    }
    
}
