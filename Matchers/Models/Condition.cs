/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Matchers.Models
{
    public abstract class Condition
    {
        public string Name { get; set; }

        /// <summary>
        /// The match result of this condition is negative.
        /// </summary>
        public bool NegativeMatch { get; set; }

        /// <summary>
        /// The match result by default. When this condition is dismissed,
        /// specified field is not found or group list is empty, this value
        /// is applied.
        /// </summary>
        public bool DefaultMatch { get; set; }

        /// <summary>
        /// The indicator that determines whether this condition always 
        /// have default match result.
        /// </summary>
        public bool DismissMatch { get; set; }
    }
}
