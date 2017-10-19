using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using DragTab.Core;

namespace DragTab.Control.Helper
{
    public static class DragHelper
    {
        internal static event DragDeltaEventHandler DragDeltaEvent;
        public static event DragTabCompletedEventHandler DragCompletedEvent;

        internal static void RaiseDragDeltaEvent(this DragItemsControl sender, DragDeltaEventArgs h)
        {
            DragDeltaEvent?.Invoke(sender, h);
        }

        internal static void RaiseDragCompletedEvent(this DragItemsControl sender, DragTabCompletedEventArgs args)
        {
            if (DragCompletedEvent != null)
            {
                args.SourceTabControl.Items.Remove(args.TabItem);
            }

            DragCompletedEvent?.Invoke(sender, args);
        }
    }

    public delegate void DragDeltaEventHandler(object sender, DragDeltaEventArgs e);

    public delegate void DragTabCompletedEventHandler(object sender, DragTabCompletedEventArgs args);
}

