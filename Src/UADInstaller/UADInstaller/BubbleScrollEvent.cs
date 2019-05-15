using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace UADInstaller
{
    public sealed class BubbleScrollEvent : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            var scrollViewerParent = FindParent<ScrollViewer>(sender as DependencyObject);
            if (scrollViewerParent != null)
            {
                scrollViewerParent.RaiseEvent(e2);
            }
        }

        public static T FindParent<T>(DependencyObject element) where T : FrameworkElement

        {

            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)

            {

                T correctlyTyped = parent as T;

                if (correctlyTyped != null)

                {

                    return correctlyTyped;

                }

                return FindParent<T>(parent);

            }

            return null;

        }
    }

}
