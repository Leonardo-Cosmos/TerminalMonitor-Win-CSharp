/* 2021/8/27 */

namespace TerminalMonitor.Clipboard
{
    public class ItemClipboard<Item>
    {
        private Item item;

        private ItemClipboardStatus status = ItemClipboardStatus.Empty;

        public void Cut(Item item)
        {
            this.item = item;
            status = ItemClipboardStatus.Move;
        }

        public void Copy(Item item)
        {
            this.item = item;
            status = ItemClipboardStatus.Copy;
        }

        public Item Paste()
        {
            Item item;
            switch (status)
            {
                case ItemClipboardStatus.Move:
                    status = ItemClipboardStatus.Empty;
                    item = this.item;
                    break;

                case ItemClipboardStatus.Copy:
                    item = this.item;
                    break;

                case ItemClipboardStatus.Empty:
                    item = default;
                    break;

                default:
                    item = default;
                    break;
            }
            return item;
        }
    }
}
