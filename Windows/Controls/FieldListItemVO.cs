/* 2021/5/12 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    class FieldListItemVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static FieldListItemVO Create(FieldDisplayDetail fieldDetail)
        {
            return new()
            {
                Id = fieldDetail.Id,
                FieldKey = fieldDetail.FieldKey,
                HeaderName = fieldDetail.HeaderName,
                Hidden = fieldDetail.Hidden,
            };
        }

        public static void Update(FieldListItemVO itemVO, FieldDisplayDetail fieldDetail)
        {
            itemVO.FieldKey = fieldDetail.FieldKey;
            itemVO.HeaderName = fieldDetail.HeaderName;
            itemVO.Hidden = fieldDetail.Hidden;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public required string Id { get; init; }

        private string? fieldKey;

        public required string FieldKey
        {
            get { return fieldKey!; }

            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private string? headerName;

        public required string? HeaderName
        {
            get { return headerName; }
            set
            {
                headerName = value;
                OnPropertyChanged();
            }
        }

        private bool hidden;

        public required bool Hidden
        {
            get { return hidden; }

            set
            {
                hidden = value;
                OnPropertyChanged();
            }
        }
    }
}
