/* 2023/8/16 */
using System;

namespace TerminalMonitor.Windows.Helpers
{
    public static class Win32
    {
        public const int WM_MOUSEHWHEEL = 0x020E;

        public static int GetIntUnchecked(IntPtr value)
        {
            return IntPtr.Size == 8 ? unchecked((int)value.ToInt64()) : value.ToInt32();
        }

        public static int HiWord(IntPtr ptr)
        {
            return unchecked((short)((uint)GetIntUnchecked(ptr) >> 16));
        }
    }
}
