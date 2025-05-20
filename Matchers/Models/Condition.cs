/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Matchers.Models
{
    public abstract class Condition(string id, string? name) : ICloneable
    {
        private readonly string _id = id;

        private string? _name = name;

        private bool _isInverted = false;

        private bool _defaultResult = false;

        private bool _isDisabled = false;

        protected Condition(string? name) : this(Guid.NewGuid().ToString(), name)
        {

        }

        protected Condition() : this(name: null)
        {

        }

        protected Condition(Condition obj) : this(obj.Name)
        {
            _isInverted = obj.IsInverted;
            _defaultResult = obj.DefaultResult;
            _isDisabled = obj.IsDisabled;
        }

        public abstract object Clone();

        public string Id
        {
            get => _id;
        }

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Gets or sets whehter the match result of this condition is inverted.
        /// </summary>
        public bool IsInverted
        {
            get => _isInverted;
            set => _isInverted = value;
        }

        /// <summary>
        /// Gets or sets default result of matching when required value is not presented
        /// (e.g. no specified field, group is empty).
        /// </summary>
        public bool DefaultResult
        {
            get => _defaultResult;
            set => _defaultResult = value;
        }

        /// <summary>
        /// Gets or sets whether this condition is temporarily excluded from group.
        /// </summary>
        public bool IsDisabled
        {
            get => _isDisabled;
            set => _isDisabled = value;
        }
    }
}
