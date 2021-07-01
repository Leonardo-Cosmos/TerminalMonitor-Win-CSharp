/* 2021/6/11 */

using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    public interface IExecutor
    {
        void Execute(CommandConfig commandConfig);

        void Terminate(string executionName);

        event ExecutionInfoEventHandler ExecutionStarted;

        event ExecutionInfoEventHandler ExecutionExited;
    }
}
