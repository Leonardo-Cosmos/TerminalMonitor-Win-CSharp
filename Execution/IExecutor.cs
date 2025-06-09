/* 2021/6/11 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    public interface IExecutor
    {
        Task Execute(CommandConfig commandConfig);

        Task Terminate(Guid executionId);

        Task Restart(Guid executionId);

        Task TerminateAll(HashSet<Guid> executionIds);

        Task TerminateAll(Guid commandId);

        Task TerminateAll();

        Task Shutdown();

        event ExecutionInfoEventHandler? ExecutionStarted;

        event ExecutionInfoEventHandler? ExecutionExited;

        event CommandInfoEventHandler? CommandFirstExecutionStarted;

        event CommandInfoEventHandler? CommandLastExecutionExited;
    }
}
