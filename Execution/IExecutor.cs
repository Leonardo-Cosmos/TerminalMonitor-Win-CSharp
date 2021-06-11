/* 2021/6/11 */

namespace TerminalMonitor.Execution
{
    public interface IExecutor
    {
        event ExecutionInfoEventHandler ExecutionStarted;

        event ExecutionInfoEventHandler ExecutionExited;
    }
}
