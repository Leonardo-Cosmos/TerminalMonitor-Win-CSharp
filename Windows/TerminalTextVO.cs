﻿/* 2021/4/20 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Checkers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    class TerminalTextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string PlainText { get; set; }

        public List<TerminalTextFieldVO> ParsedFields { get; set; }

        public bool Hidden { get; private set; }

        public void Filter(IList<FilterCondition> filterConditions)
        {
            if (filterConditions == null)
            {
                return;
            }

            bool included = filterConditions
                .Where(filterCondition => !filterCondition.Excluded)
                .All(filterCondition => Match(filterCondition.Condition));

            bool excluded = filterConditions
                .Where(filterConditions => filterConditions.Excluded)
                .Any(filterCondition => Match(filterCondition.Condition));

            var hidden = included && !excluded;

            if (hidden != Hidden)
            {
                Hidden = hidden;
            }
        }

        private bool Match(TextCondition condition)
        {
            if (condition == null)
            {
                return false;
            }
            var field = (ParsedFields ?? new()).FirstOrDefault(field => field.Key == condition.FieldKey);
            return TextChecker.Check(field.Value, condition.TargetValue, condition.CheckOperator);
        }

    }
}
