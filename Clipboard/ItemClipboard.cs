/* 2021/8/27 */
using System;

namespace TerminalMonitor.Clipboard
{
    public class ItemClipboard<T>
    {
        private T[] items;

        private ItemClipboardStatus status = ItemClipboardStatus.Empty;

        public void Cut(params T[] items)
        {
            this.items = items;
            status = ItemClipboardStatus.Move;

            OnItemCut();
        }

        public void Copy(params T[] items)
        {
            this.items = items;
            status = ItemClipboardStatus.Copy;

            OnItemCopied();
        }

        public (T[], ItemClipboardStatus status) Paste()
        {
            T[] results;
            var pasteStatus = status;
            switch (status)
            {
                case ItemClipboardStatus.Move:
                    status = ItemClipboardStatus.Empty;
                    results = items;
                    break;

                case ItemClipboardStatus.Copy:
                    results = items;
                    break;

                case ItemClipboardStatus.Empty:
                    results = default;
                    break;

                default:
                    results = default;
                    break;
            }
            return (results, pasteStatus);
        }

        protected void OnItemCut()
        {
            ItemCut?.Invoke(this, EventArgs.Empty);
        }

        protected void OnItemCopied()
        {
            ItemCopied?.Invoke(this, EventArgs.Empty);
        }

        protected void OnItemPasted()
        {
            ItemPasted?.Invoke(this, EventArgs.Empty);
        }

        public bool ContainsItem
        {
            get => status != ItemClipboardStatus.Empty;
        }

        public event EventHandler ItemCut;

        public event EventHandler ItemCopied;

        public event EventHandler ItemPasted;
    }
}
