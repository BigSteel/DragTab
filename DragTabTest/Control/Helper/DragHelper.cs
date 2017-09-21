using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragTabTest.Control.Helper
{
    public static class DragHelper
    {
        public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent(
            "DragDelta", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragHelper));

        public static void AddDragDeltaHandler(DependencyObject d, RoutedEventHandler h)
        {
            UIElement e = d as UIElement;
            if (e != null)
            {
                e.AddHandler(DragDeltaEvent, h);
            }
        }

        public static void RemoveDragDeltaHandler(DependencyObject d, RoutedEventHandler h)
        {
            UIElement e = d as UIElement;
            if (e != null)
            {
                e.RemoveHandler(DragDeltaEvent, h);
            }
        }
    }

}
