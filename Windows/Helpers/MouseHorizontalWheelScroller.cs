/* 2023/8/15 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;

namespace TerminalMonitor.Windows.Helpers
{
    public static class MouseHorizontalWheelScroller
    {
        public static bool GetMouseHorizontalWheelScrollingEnabled(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(MouseHorizontalWheelScrollingEnabledProperty);
        }
        public static void SetMouseHorizontalWheelScrollingEnabled(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(MouseHorizontalWheelScrollingEnabledProperty, value);
        }

        public static readonly DependencyProperty MouseHorizontalWheelScrollingEnabledProperty =
                DependencyProperty.RegisterAttached("MouseHorizontalWheelScrollingEnabled",
                    typeof(bool), typeof(MouseHorizontalWheelScroller),
                    new UIPropertyMetadata(false, OnMouseHorizontalWheelScrollingEnabledChanged));

        private static readonly HashSet<int> controls = new();
        public static void OnMouseHorizontalWheelScrollingEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is Control control && GetMouseHorizontalWheelScrollingEnabled(dependencyObject)
                && controls.Add(control.GetHashCode()))
            {
                control.MouseEnter += (sender, e) =>
                {
                    var scrollViewer = dependencyObject.FindChildOfType<ScrollViewer>();
                    if (scrollViewer != null)
                    {
                        new MouseHorizontalWheelScrollHelper(scrollViewer, dependencyObject);
                    }
                };
            }
        }

        private class MouseHorizontalWheelScrollHelper
        {
            /// <summary>
            /// Multiplier of how far to scroll horizontally. Change as desired.
            /// </summary>
            private const int scrollFactor = 3;

            private const int WM_MOUSEHWEEL = 0x20e;

            private readonly ScrollViewer scrollViewer;

            private readonly HwndSource hwndSource;

            private readonly HwndSourceHook hook;

            private static readonly HashSet<int> scrollViewers = new();

            public MouseHorizontalWheelScrollHelper(ScrollViewer scrollViewer, DependencyObject dependencyObject)
            {
                this.scrollViewer = scrollViewer;
                hwndSource = PresentationSource.FromDependencyObject(dependencyObject) as HwndSource;
                hook = WindowProc;
                hwndSource?.AddHook(hook);
                if (scrollViewers.Add(scrollViewer.GetHashCode()))
                {
                    scrollViewer.MouseLeave += (sender, e) =>
                    {
                        hwndSource.RemoveHook(hook);
                    };
                }
            }

            IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                switch (msg)
                {
                    case WM_MOUSEHWEEL:
                        Scroll(wParam);
                        handled = true;
                        break;
                }
                return IntPtr.Zero;
            }

            private void Scroll(IntPtr wParam)
            {
                int delta = (HIWORD(wParam) > 0 ? 1 : -1) * scrollFactor;
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + delta);
            }

            private static int HIWORD(IntPtr ptr) => (short)((((int)ptr.ToInt64()) >> 16) & 0xFFFF);
        }
    }
}
