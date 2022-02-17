/* 2021/8/27 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Clipboard
{
    public class ItemClipboard<T>
    {
        private T[] items;

        private ItemClipboardStatus status = ItemClipboardStatus.Empty;

        public void Cut(params T[] items)
        {
            if (items == null)
            {
                return;
            }

            this.items = items;
            status = ItemClipboardStatus.Move;

            OnItemCut();
        }

        public void Cut(IEnumerable<T> items)
        {
            Cut(items?.ToArray());
        }

        public void Copy(params T[] items)
        {
            if (items == null)
            {
                return;
            }

            this.items = items;
            status = ItemClipboardStatus.Copy;

            OnItemCopied();
        }

        public void Copy(IEnumerable<T> items)
        {
            Copy(items?.ToArray());
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

            OnItemPasted();

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

        public ItemClipboardStatus Status => status;

        public bool ContainsItem => status != ItemClipboardStatus.Empty;

        public event EventHandler ItemCut;

        public event EventHandler ItemCopied;

        public event EventHandler ItemPasted;
    }
}
