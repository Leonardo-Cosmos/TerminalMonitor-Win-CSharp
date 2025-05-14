/* 2021/4/27 */
using System;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Matchers.Models
{
    public class FieldCondition : Condition, ICloneable
    {
        private string _fieldKey;

        private TextMatchOperator _matchOperator;

        private string _targetValue;

        public FieldCondition(string id, string? name, string fieldKey, TextMatchOperator matchOperator, string targetValue) : base(id, name)
        {
            _fieldKey = fieldKey;
            _matchOperator = matchOperator;
            _targetValue = targetValue;
        }

        public FieldCondition(string? name, string fieldKey, TextMatchOperator matchOperator, string targetValue) : base(name)
        {
            _fieldKey = fieldKey;
            _matchOperator = matchOperator;
            _targetValue = targetValue;
        }

        public FieldCondition(string fieldKey, TextMatchOperator matchOperator, string targetValue) : base()
        {
            _fieldKey = fieldKey;
            _matchOperator = matchOperator;
            _targetValue = targetValue;
        }

        protected FieldCondition(FieldCondition obj) : base(obj)
        {
            _fieldKey = obj.FieldKey;
            _matchOperator = obj.MatchOperator;
            _targetValue = obj.TargetValue;
        }

        public override object Clone()
        {
            return new FieldCondition(this);
        }

        public static FieldCondition Empty => new(
            String.Empty,
            TextMatchOperator.None,
            String.Empty
        );

        public string FieldKey
        {
            get => _fieldKey;
            set => _fieldKey = value;
        }

        public TextMatchOperator MatchOperator
        {
            get => _matchOperator;
            set => _matchOperator = value;
        }

        public string TargetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }
    }
}
