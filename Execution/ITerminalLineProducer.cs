/* 2021/5/9 */
using System.Collections.Generic;

namespace TerminalMonitor.Execution
{
    public interface ITerminalLineProducer
    {
        IEnumerable<string> ReadTerminalLines();

        bool IsCompleted
        {
            get;
        }
    }
}
