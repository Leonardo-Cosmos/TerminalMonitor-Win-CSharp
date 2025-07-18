﻿/* 2021/6/11 */

using System;

namespace TerminalMonitor.Models
{
    public class ExecutionInfo
    {
        public required Guid Id { get; init; }

        public required string Name { get; init; }

        public required ExecutionStatus Status { get; init; }
    }
}
