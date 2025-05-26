/* 2023/8/11 */
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TerminalMonitor.Windows.Helpers
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Returns the first ancestor of specified type
        /// </summary>
        public static T? FindAncestor<T>(this DependencyObject current) where T : DependencyObject
        {
            current = GetVisualOrLogicalParent(current);

            while (current != null)
            {
                if (current is T element)
                {
                    return element;
                }
                current = GetVisualOrLogicalParent(current);
            }

            return null;
        }

        private static DependencyObject GetVisualOrLogicalParent(DependencyObject obj)
        {
            if (obj is Visual || obj is Visual3D)
            {
                return VisualTreeHelper.GetParent(obj);
            }
            return LogicalTreeHelper.GetParent(obj);
        }

        /// <summary>
        /// Finds first child of provided type. If child not found, null is returned
        /// </summary>
        /// <typeparam name="T">Type of chiled to be found</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T? FindChildOfType<T>(this DependencyObject source) where T : DependencyObject
        {
            T? result = source as T;
            DependencyObject child;
            if (source != null && result == null)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(source);
                for (int i = 0; i < childrenCount; i++)
                {
                    child = VisualTreeHelper.GetChild(source, i);
                    if (child != null)
                    {
                        if (child is T)
                        {
                            result = child as T;
                            break;
                        }
                        else
                        {
                            result = child.FindChildOfType<T>();
                            if (result != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
