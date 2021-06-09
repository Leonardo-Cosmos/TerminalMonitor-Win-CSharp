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

        private Task processTask;

        internal Execution(CommandConfig commandConfig)
        {
            this.commandConfig = commandConfig;
        }

        public void Execute()
        {
            if (started)
            {
                throw new InvalidOperationException("It has been started already.");
            }

            processTask = Start(commandConfig.StartFile, commandConfig.Arguments, commandConfig.WorkDirectory);
            started = true;
        }

        private async Task Start(string startFile, string arguments = null, string workDirectory = null)
        {
            if (!String.IsNullOrEmpty(startFile))
            {
                var process = new Process();
                process.StartInfo.FileName = startFile;
                if (!String.IsNullOrWhiteSpace(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                if (!String.IsNullOrWhiteSpace(workDirectory))
                {
                    process.StartInfo.WorkingDirectory = workDirectory;
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) =>
                {

                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        OnLineReceived(new()
                        {
                            Line = e.Data
                        });
                    }

                };

                process.Start();

                process.BeginOutputReadLine();

                await process.WaitForExitAsync();
                process.Close();

                OnCompleted(EventArgs.Empty);
            }
        }

        protected void OnLineReceived(TerminalLineEventArgs e)
        {
            LineReceived?.Invoke(this, e);
        }

        protected void OnCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        public TerminalLineEventHandler LineReceived;

        public EventHandler Completed;

        public bool IsCompleted
        {
            get
            {
                return processTask.IsCompleted;
            }
        }
    }
    
}
