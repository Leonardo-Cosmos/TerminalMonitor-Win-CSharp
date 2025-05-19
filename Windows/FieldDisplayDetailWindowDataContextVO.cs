/* 2021/5/24 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    class FieldDisplayDetailWindowDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? fieldKey;

        public required string FieldKey
        {
            get => fieldKey!;
            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private bool hidden;

        public bool Hidden
        {
            get => hidden;
            set
            {
                hidden = value;
                OnPropertyChanged();
            }
        }

        private string? headerName;

        public string? HeaderName
        {
            get => headerName;
            set
            {
                headerName = value;
                OnPropertyChanged();
            }
        }

        private bool customizeHeaderStyle;

        public bool CustomizeHeaderStyle
        {
            get => customizeHeaderStyle;
            set
            {
                customizeHeaderStyle = value;
                OnPropertyChanged();
            }
        }

        private ColumnHeaderStyle? headerStyle;

        public required ColumnHeaderStyle HeaderStyle
        {
            get => headerStyle!;
            set
            {
                headerStyle = value;
                OnPropertyChanged();
            }
        }

        private bool customizeStyle;

        public bool CustomizeStyle
        {
            get => customizeStyle;
            set
            {
                customizeStyle = value;
                OnPropertyChanged();
            }
        }

        private TextStyle? style;

        public required TextStyle Style
        {
            get => style!;
            set
            {
                style = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyConditionSelected;

        public bool IsAnyConditionSelected
        {
            get => isAnyConditionSelected;
            set
            {
                isAnyConditionSelected = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyConditionInClipboard;

        public bool IsAnyConditionInClipboard
        {
            get => isAnyConditionInClipboard;
            set
            {
                isAnyConditionInClipboard = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyConditionCutInClipboard;

        public bool IsAnyConditionCutInClipboard
        {
            get => isAnyConditionCutInClipboard;
            set
            {
                isAnyConditionCutInClipboard = value;
                OnPropertyChanged();
            }
        }

        public required ICommand AddCommand { get; init; }

        public required ICommand RemoveCommand { get; init; }

        public required ICommand MoveUpCommand { get; init; }

        public required ICommand MoveDownCommand { get; init; }

        public required ICommand CutCommand { get; init; }

        public required ICommand CopyCommand { get; init; }

        public required ICommand PasteCommnad { get; init; }
    }
}
