/* 2023/8/14 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace TerminalMonitor.Windows.Helpers
{
    public static class MouseHorizontalWheelEnabler
    {
        /// <summary>
        ///   When true it will try to enable Horizontal Wheel support on parent windows/popups/context menus automatically
        ///   so the programmer does not need to call it.
        ///   Defaults to true.
        /// </summary>
        public static readonly bool AutoEnableMouseHorizontalWheelSupport = true;

        private static readonly HashSet<IntPtr> hookedWindows = new();

        /// <summary>
        ///   Enable Horizontal Wheel support for all the controls inside the window.
        ///   This method does not need to be called if AutoEnableMouseHorizontalWheelSupport is true.
        ///   This does not include popups or context menus.
        ///   If it was already enabled it will do nothing.
        /// </summary>
        /// <param name="window">Window to enable support for.</param>
        public static void EnableMouseHorizontalWheelSupport([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (window.IsLoaded)
            {
                // handle should be available at this level
                IntPtr handle = new WindowInteropHelper(window).Handle;
                EnableMouseHorizontalWheelSupport(handle);
            }
            else
            {
                window.Loaded += (sender, args) =>
                {
                    IntPtr handle = new WindowInteropHelper(window).Handle;
                    EnableMouseHorizontalWheelSupport(handle);
                };
            }
        }

        /// <summary>
        ///   Enable Horizontal Wheel support for all the controls inside the popup.
        ///   This method does not need to be called if AutoEnableMouseHorizontalWheelSupport is true.
        ///   This does not include sub-popups or context menus.
        ///   If it was already enabled it will do nothing.
        /// </summary>
        /// <param name="popup">Popup to enable support for.</param>
        public static void EnableMouseHorizontalWheelSupport([NotNull] Popup popup)
        {
            if (popup == null)
            {
                throw new ArgumentNullException(nameof(popup));
            }

            if (popup.IsOpen)
            {
                // handle should be available at this level
                // ReSharper disable once PossibleInvalidOperationException
                EnableMouseHorizontalWheelSupport(GetObjectParentHandle(popup.Child).Value);
            }

            // also hook for IsOpened since a new window is created each time
            popup.Opened += (sender, args) =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                EnableMouseHorizontalWheelSupport(GetObjectParentHandle(popup.Child).Value);
            };
        }

        /// <summary>
        ///   Enable Horizontal Wheel support for all the controls inside the context menu.
        ///   This method does not need to be called if AutoEnableMouseHorizontalWheelSupport is true.
        ///   This does not include popups or sub-context menus.
        ///   If it was already enabled it will do nothing.
        /// </summary>
        /// <param name="contextMenu">Context menu to enable support for.</param>
        public static void EnableMouseHorizontalWheelSupport([NotNull] ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                throw new ArgumentNullException(nameof(contextMenu));
            }

            if (contextMenu.IsOpen)
            {
                // handle should be available at this level
                // ReSharper disable once PossibleInvalidOperationException
                EnableMouseHorizontalWheelSupport(GetObjectParentHandle(contextMenu).Value);
            }

            // also hook for IsOpened since a new window is created each time
            contextMenu.Opened += (sender, args) =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                EnableMouseHorizontalWheelSupport(GetObjectParentHandle(contextMenu).Value);
            };
        }

        /// <summary>
        ///   Enable Horizontal Wheel support for all the controls inside the HWND.
        ///   This method does not need to be called if AutoEnableMouseHorizontalWheelSupport is true.
        ///   This does not include popups or sub-context menus.
        ///   If it was already enabled it will do nothing.
        /// </summary>
        /// <param name="handle">HWND handle to enable support for.</param>
        /// <returns>True if it was enabled or already enabled, false if it couldn't be enabled.</returns>
        public static bool EnableMouseHorizontalWheelSupport(IntPtr handle)
        {
            if (hookedWindows.Contains(handle))
            {
                return true;
            }

            hookedWindows.Add(handle);
            HwndSource source = HwndSource.FromHwnd(handle);
            if (source == null)
            {
                return false;
            }

            source.AddHook(WndProcHook);
            return true;
        }

        private static IntPtr? GetObjectParentHandle([NotNull] DependencyObject depObj)
        {
            if (depObj == null)
            {
                throw new ArgumentNullException(nameof(depObj));
            }

            var presentationSource = PresentationSource.FromDependencyObject(depObj) as HwndSource;
            return presentationSource?.Handle;
        }

        /// <summary>
        ///   Disable Horizontal Wheel support for all the controls inside the HWND.
        ///   This method does not need to be called in most cases.
        ///   This does not include popups or sub-context menus.
        ///   If it was already disabled it will do nothing.
        /// </summary>
        /// <param name="handle">HWND handle to disable support for.</param>
        /// <returns>True if it was disabled or already disabled, false if it couldn't be disabled.</returns>
        public static bool DisableMouseHorizontalWheelSupport(IntPtr handle)
        {
            if (!hookedWindows.Contains(handle))
            {
                return true;
            }

            HwndSource source = HwndSource.FromHwnd(handle);
            if (source == null)
            {
                return false;
            }

            source.RemoveHook(WndProcHook);
            hookedWindows.Remove(handle);
            return true;
        }

        /// <summary>
        ///   Disable Horizontal Wheel support for all the controls inside the window.
        ///   This method does not need to be called in most cases.
        ///   This does not include popups or sub-context menus.
        ///   If it was already disabled it will do nothing.
        /// </summary>
        /// <param name="window">Window to disable support for.</param>
        /// <returns>True if it was disabled or already disabled, false if it couldn't be disabled.</returns>
        public static bool DisableMouseHorizontalWheelSupport([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            IntPtr handle = new WindowInteropHelper(window).Handle;
            return DisableMouseHorizontalWheelSupport(handle);
        }

        /// <summary>
        ///   Disable Horizontal Wheel support for all the controls inside the popup.
        ///   This method does not need to be called in most cases.
        ///   This does not include popups or sub-context menus.
        ///   If it was already disabled it will do nothing.
        /// </summary>
        /// <param name="popup">Popup to disable support for.</param>
        /// <returns>True if it was disabled or already disabled, false if it couldn't be disabled.</returns>
        public static bool DisableMouseHorizontalWheelSupport([NotNull] Popup popup)
        {
            if (popup == null)
            {
                throw new ArgumentNullException(nameof(popup));
            }

            IntPtr? handle = GetObjectParentHandle(popup.Child);
            if (handle == null)
            {
                return false;
            }

            return DisableMouseHorizontalWheelSupport(handle.Value);
        }

        /// <summary>
        ///   Disable Horizontal Wheel support for all the controls inside the context menu.
        ///   This method does not need to be called in most cases.
        ///   This does not include popups or sub-context menus.
        ///   If it was already disabled it will do nothing.
        /// </summary>
        /// <param name="contextMenu">Context menu to disable support for.</param>
        /// <returns>True if it was disabled or already disabled, false if it couldn't be disabled.</returns>
        public static bool DisableMouseHorizontalWheelSupport([NotNull] ContextMenu contextMenu)
        {
            if (contextMenu == null)
            {
                throw new ArgumentNullException(nameof(contextMenu));
            }

            IntPtr? handle = GetObjectParentHandle(contextMenu);
            if (handle == null)
            {
                return false;
            }

            return DisableMouseHorizontalWheelSupport(handle.Value);
        }


        /// <summary>
        ///   Enable Horizontal Wheel support for all that control and all controls hosted by the same window/popup/context menu.
        ///   This method does not need to be called if AutoEnableMouseHorizontalWheelSupport is true.
        ///   If it was already enabled it will do nothing.
        /// </summary>
        /// <param name="uiElement">UI Element to enable support for.</param>
        public static void EnableMouseHorizontalWheelSupportForParentOf(UIElement uiElement)
        {
            // try to add it right now
            if (uiElement is Window window)
            {
                EnableMouseHorizontalWheelSupport(window);
            }
            else if (uiElement is Popup popup)
            {
                EnableMouseHorizontalWheelSupport(popup);
            }
            else if (uiElement is ContextMenu contextMenu)
            {
                EnableMouseHorizontalWheelSupport(contextMenu);
            }
            else
            {
                IntPtr? parentHandle = GetObjectParentHandle(uiElement);
                if (parentHandle != null)
                {
                    EnableMouseHorizontalWheelSupport(parentHandle.Value);
                }

                // and in the rare case the parent window ever changes...
                PresentationSource.AddSourceChangedHandler(uiElement, PresenationSourceChangedHandler);
            }
        }

        private static void PresenationSourceChangedHandler(object sender, SourceChangedEventArgs sourceChangedEventArgs)
        {
            var src = sourceChangedEventArgs.NewSource as HwndSource;
            if (src != null)
            {
                EnableMouseHorizontalWheelSupport(src.Handle);
            }
        }

        private static IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // transform horizontal mouse wheel messages 
            switch (msg)
            {
                case Win32.WM_MOUSEHWHEEL:
                    HandleMouseHorizontalWheel(wParam);
                    break;
            }
            return IntPtr.Zero;
        }

        private static void HandleMouseHorizontalWheel(IntPtr wParam)
        {
            int tilt = -Win32.HiWord(wParam);
            if (tilt == 0)
            {
                return;
            }

            IInputElement element = Mouse.DirectlyOver;
            if (element == null)
            {
                return;
            }

            if (element is not UIElement)
            {
                element = VisualTreeHelpers.FindAncestor<UIElement>(element as DependencyObject);
            }
            if (element == null)
            {
                return;
            }

            var eventArgs = new MouseHorizontalWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, tilt)
            {
                RoutedEvent = PreviewMouseHorizontalWheelEvent
                //Source = handledWindow
            };

            // first raise preview
            element.RaiseEvent(eventArgs);
            if (eventArgs.Handled)
            {
                return;
            }

            // then bubble it
            eventArgs.RoutedEvent = MouseHorizontalWheelEvent;
            element.RaiseEvent(eventArgs);
        }

        #region MouseWheelHorizontal Event

        public static readonly RoutedEvent MouseHorizontalWheelEvent =
          EventManager.RegisterRoutedEvent("MouseHorizontalWheel", RoutingStrategy.Bubble, typeof(MouseMouseHorizontalWheelEventHandler),
            typeof(MouseHorizontalWheelEnabler));

        public static void AddMouseHorizontalWheelHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement != null)
            {
                uiElement.AddHandler(MouseHorizontalWheelEvent, handler);

                if (AutoEnableMouseHorizontalWheelSupport)
                {
                    EnableMouseHorizontalWheelSupportForParentOf(uiElement);
                }
            }
        }

        public static void RemoveMouseHorizontalWheelHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var uiElement = dependencyObject as UIElement;
            uiElement?.RemoveHandler(MouseHorizontalWheelEvent, handler);
        }

        #endregion

        #region PreviewMouseWheelHorizontal Event

        public static readonly RoutedEvent PreviewMouseHorizontalWheelEvent =
          EventManager.RegisterRoutedEvent("PreviewMouseHorizontalWheel", RoutingStrategy.Tunnel, typeof(RoutedEventHandler),
            typeof(MouseHorizontalWheelEnabler));

        public static void AddPreviewMouseHorizontalWheelHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement != null)
            {
                uiElement.AddHandler(PreviewMouseHorizontalWheelEvent, handler);

                if (AutoEnableMouseHorizontalWheelSupport)
                {
                    EnableMouseHorizontalWheelSupportForParentOf(uiElement);
                }
            }
        }

        public static void RemovePreviewMouseHorizontalWheelHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            var uiElement = dependencyObject as UIElement;
            uiElement?.RemoveHandler(PreviewMouseHorizontalWheelEvent, handler);
        }

        #endregion
    }

    public class MouseHorizontalWheelEventArgs : MouseEventArgs
    {
        public int HorizontalDelta { get; }

        public MouseHorizontalWheelEventArgs(MouseDevice mouse, int timestamp, int horizontalDelta)
          : base(mouse, timestamp)
        {
            HorizontalDelta = horizontalDelta;
        }
    }

    public delegate void MouseMouseHorizontalWheelEventHandler(object sender, MouseEventArgs e);
}
