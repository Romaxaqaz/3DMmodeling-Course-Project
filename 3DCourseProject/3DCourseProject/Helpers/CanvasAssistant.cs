using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace _3DCourseProject.Helpers
{
    internal static class CanvasAssistant
    {
        #region Dependency Properties

        public static readonly DependencyProperty BoundChildrenProperty =
            DependencyProperty.RegisterAttached("BoundChildren", typeof(object), typeof(CanvasAssistant),
                                                new FrameworkPropertyMetadata(null, OnBoundChildrenChanged));

        #endregion

        public static void SetBoundChildren(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(BoundChildrenProperty, value);
        }

        private static void OnBoundChildrenChanged(DependencyObject dependencyObject,
                                                   DependencyPropertyChangedEventArgs e)
        {

            var canvas = dependencyObject as Canvas;
            if (canvas == null) return;

            canvas.Children.Clear();

            var objects = (ObservableCollection<UIElement>)e.NewValue;
            if (objects == null)
            {
                canvas.Children.Clear();
                return;
            }

            objects.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                    foreach (var item in args.NewItems)
                    {
                        canvas.Children.Add((UIElement)item);
                    }
                if (args.Action == NotifyCollectionChangedAction.Remove)
                    foreach (var item in args.OldItems)
                    {
                        canvas.Children.Remove((UIElement)item);
                    }
            };

            foreach (var item in objects)
            {
                canvas.Children.Add(item);
            }
        }
    }
}
