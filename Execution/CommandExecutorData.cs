/* 2025/6/6 */
using System;
using System.Collections.Generic;

namespace TerminalMonitor.Execution
{
    internal readonly struct CommandExecutorData
    {
        private readonly object executionLock = new();

        /// <summary>
        /// A set of names of running executions.
        /// </summary>
        private readonly HashSet<string> executionNames = [];

        /// <summary>
        /// A dictionary from execution ID to execution name.
        /// </summary>
        private readonly Dictionary<Guid, string> executionNameDict = [];

        /// <summary>
        /// A dictionary from execution ID to execution detail.
        /// </summary>
        private readonly Dictionary<Guid, Execution> executionDict = [];

        /// <summary>
        /// A dictionary from command ID to execution ID set.
        /// </summary>
        private readonly Dictionary<Guid, HashSet<Guid>> commandExecutionsDict = [];

        public CommandExecutorData()
        {
        }

        public bool IsEmpty
        {
            get
            {
                lock (executionLock)
                {
                    return executionNames.Count == 0;
                }
            }
        }

        public void AddExecution(string executionName, Execution execution)
        {
            lock (executionLock)
            {
                if (executionNames.Contains(executionName))
                {
                    throw new ArgumentException($"Duplicated {nameof(executionName)} {executionName}");
                }

                if (executionDict.ContainsKey(execution.Id))
                {
                    throw new ArgumentException($"Duplicated {nameof(execution)} {execution.Id}");
                }

                executionNames.Add(executionName);
                executionNameDict.Add(execution.Id, executionName);
                executionDict.Add(execution.Id, execution);

                var commandConfig = execution.CommandConfig;
                if (commandExecutionsDict.TryGetValue(commandConfig.Id, out HashSet<Guid>? commandExecutions))
                {
                    commandExecutions.Add(execution.Id);
                }
                else
                {
                    commandExecutions = [execution.Id];
                    commandExecutionsDict.Add(commandConfig.Id, commandExecutions);
                }
            }
        }

        public (string, Execution) RemoveExecution(Guid executionId)
        {
            lock (executionLock)
            {
                if (!executionNameDict.TryGetValue(executionId, out string? executionName))
                {
                    throw new ArgumentException($"Not existing {nameof(executionId)} {executionId}");
                }

                executionNameDict.Remove(executionId);
                executionNames.Remove(executionName);

                var execution = executionDict[executionId];
                executionDict.Remove(executionId);

                var commandConfig = execution.CommandConfig;

                var commandExecutions = commandExecutionsDict[commandConfig.Id];
                commandExecutions.Remove(executionId);
                if (commandExecutions.Count == 0)
                {
                    commandExecutionsDict.Remove(commandConfig.Id);
                }

                return (executionName, execution);
            }
        }

        public Execution? GetExecution(Guid executionId)
        {
            return executionDict[executionId];
        }

        public string? GetExecutionName(Guid executionId)
        {
            return executionNameDict[executionId];
        }

        public HashSet<Guid> GetExecutionIds()
        {
            return [..executionDict.Keys];
        }

        public HashSet<Guid>? GetExecutionIds(Guid commandId)
        {
            return commandExecutionsDict.TryGetValue(commandId, out var executionIds) ? executionIds : null;
        }

        /// <summary>
        /// By default, the execution takes command's name,
        /// but when there are multiple executions from same command,
        /// each execution should have a unique name.
        /// </summary>
        public string GetUniqueExecutionName(string configName)
        {
            if (!executionNames.Contains(configName))
            {
                return configName;
            }

            var number = 0;
            string name;
            do
            {
                number++;
                name = $"{configName} {number}";
            } while (executionNames.Contains(name));

            return name;
        }
    }
}
