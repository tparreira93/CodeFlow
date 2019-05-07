using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeFlow.Editor
{
    internal class VS2019HostControl
    {
        public static void DebugShowParents(DependencyObject child, string caption)
        {
            string messageBoxText = "";
            for (DependencyObject parent = VisualTreeHelper.GetParent(child); parent != null; parent = VisualTreeHelper.GetParent(parent))
                messageBoxText = messageBoxText + parent.GetType().FullName + Environment.NewLine;
            if (messageBoxText.Length <= 0)
                return;
            int num = (int)MessageBox.Show(messageBoxText, caption);
        }

        public static UIElement GetTopParent(UIElement hostControl)
        {
            DependencyObject dependencyObject = (DependencyObject)null;
            for (DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)hostControl); parent != null; parent = VisualTreeHelper.GetParent(parent))
                dependencyObject = parent;
            if (dependencyObject != null && dependencyObject is UIElement)
                return dependencyObject as UIElement;
            return hostControl;
        }

        public static void RemoveParent(UIElement hostControl)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)hostControl);
            if (parent == null || !(parent is Decorator))
                return;
            (parent as Decorator).Child = (UIElement)null;
        }

        public static void HideNavigationBar(UIElement hostControl)
        {
            FrameworkElement element = VS2019HostControl.FindElement((Visual)hostControl, "DropDownBarMargin");
            if (element == null)
                return;
            element.Visibility = Visibility.Collapsed;
        }

        private static FrameworkElement FindElement(Visual v, string name)
        {
            if (v == null)
                return (FrameworkElement)null;
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject)v); ++childIndex)
            {
                Visual child = VisualTreeHelper.GetChild((DependencyObject)v, childIndex) as Visual;
                if (child != null)
                {
                    FrameworkElement frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name.Equals(name))
                        return frameworkElement;
                }
                FrameworkElement element = VS2019HostControl.FindElement(child, name);
                if (element != null)
                    return element;
            }
            return (FrameworkElement)null;
        }
    }
}
