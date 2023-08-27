/* 2023/8/15 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;
using System.Diagnostics;

namespace TerminalMonitor.Windows.Helpers
{
    public static class MouseHorizontalWheelScroller
    {
        public static bool GetScrollingEnabled(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ScrollingEnabledProperty);
        }
        public static void SetScrollingEnabled(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ScrollingEnabledProperty, value);
        }

        public static readonly DependencyProperty ScrollingEnabledProperty =
                DependencyProperty.RegisterAttached("ScrollingEnabled",
                    typeof(bool), typeof(MouseHorizontalWheelScroller),
                    new UIPropertyMetadata(false, OnScrollingEnabledChanged));

        private static readonly HashSet<int> controls = new();

        private static readonly HashSet<int> scrollViewers = new();

        public static void OnScrollingEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            if (GetScrollingEnabled(dependencyObject) &&
                dependencyObject is Control control &&
                controls.Add(control.GetHashCode()))
            {
                control.MouseEnter += (sender, e) =>
                {
                    var scrollViewer = dependencyObject.FindChildOfType<ScrollViewer>();
                    if (scrollViewer != null && scrollViewers.Add(scrollViewer.GetHashCode()))
                    {
                        new MouseHorizontalWheelScrollHelper(scrollViewer, dependencyObject);
                    }
                };
            }
        }

        private class MouseHorizontalWheelScrollHelper
        {
            private readonly ScrollViewer scrollViewer;

            private readonly HwndSource hwndSource;

            private readonly HwndSourceHook hook;

            public MouseHorizontalWheelScrollHelper(ScrollViewer scrollViewer, DependencyObject dependencyObject)
            {
                this.scrollViewer = scrollViewer;
                hwndSource = PresentationSource.FromDependencyObject(dependencyObject) as HwndSource;
                if (hwndSource == null)
                {
                    return;
                }

                hook = WindowProc;

                scrollViewer.MouseEnter += (sender, e) =>
                {
                    hwndSource.AddHook(hook);
                };

                scrollViewer.MouseLeave += (sender, e) =>
                {
                    hwndSource.RemoveHook(hook);
                };
            }

            private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                switch (msg)
                {
                    case Win32.WM_MOUSEHWHEEL:
                        Scroll(wParam);
                        handled = true;
                        break;
                }
                return IntPtr.Zero;
            }

            private void Scroll(IntPtr wParam)
            {
                int delta = Win32.HiWord(wParam);
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + delta);
            }
        }
    }
}
