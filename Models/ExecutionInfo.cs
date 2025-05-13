/* 2021/6/11 */

namespace TerminalMonitor.Models
{
    public class ExecutionInfo
    {
        public required string Id { get; init; }

        public required string Name { get; init; }

        public required ExecutionStatus Status { get; init; }
    }
}
