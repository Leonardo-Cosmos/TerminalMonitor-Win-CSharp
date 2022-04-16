/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Matchers.Models
{
    public abstract class Condition : ICloneable
    {
        protected Condition()
        {

        }

        protected Condition(Condition obj)
        {
            Id = Guid.NewGuid().ToString();
            Name = obj.Name;
            IsInverted = obj.IsInverted;
            DefaultResult = obj.DefaultResult;
            IsDisabled = obj.IsDisabled;
        }

        public abstract object Clone();

        public string Id { get; init; }

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whehter the match result of this condition is inverted.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Gets or sets default result of matching when required value is not presented
        /// (e.g. no specified field, group is empty).
        /// </summary>
        public bool DefaultResult { get; set; }

        /// <summary>
        /// Gets or sets whether this condition is temporarily excluded from group.
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
