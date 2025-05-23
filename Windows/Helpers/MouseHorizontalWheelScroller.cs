﻿/* 2023/8/15 */
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

        private static readonly HashSet<int> controls = [];

        private static readonly HashSet<int> scrollViewers = [];

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
                        var helper = new MouseHorizontalWheelScrollHelper(scrollViewer, dependencyObject);
                        helper.AddEventHandler();
                    }
                };
            }
        }

        private sealed class MouseHorizontalWheelScrollHelper
        {
            private readonly ScrollViewer scrollViewer;

            private readonly HwndSource hwndSource;

            private readonly HwndSourceHook hook;

            private bool eventHandleradded = false;

            public MouseHorizontalWheelScrollHelper(ScrollViewer scrollViewer, DependencyObject dependencyObject)
            {
                this.scrollViewer = scrollViewer;

                if (PresentationSource.FromDependencyObject(dependencyObject) is not HwndSource hwndSource)
                {
                    throw new InvalidOperationException();
                }
                this.hwndSource = hwndSource;

                hook = WindowProc;
            }

            public void AddEventHandler()
            {
                hwndSource.AddHook(hook);
                eventHandleradded = true;

                scrollViewer.MouseEnter += (sender, e) =>
                {
                    if (eventHandleradded)
                    {
                        return;
                    }

                    hwndSource.AddHook(hook);
                    eventHandleradded = true;
                };

                scrollViewer.MouseLeave += (sender, e) =>
                {
                    if (!eventHandleradded)
                    {
                        return;
                    }

                    hwndSource.RemoveHook(hook);
                    eventHandleradded = false;
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
