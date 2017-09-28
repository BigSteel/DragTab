using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragTab.Core
{
    public delegate void DragStartedEventHandle(object sender, DragDropEventArgs args);
    public delegate void DragDeltaEventHandle(object sender, DragDropEventArgs args);
    public delegate void DragCompletedEventHandle(object sender, DragDropEventArgs args);
    public interface IDragDropHandle
    {
        IDragSource DragSource { get; set; }
        event DragStartedEventHandle DragStartedEvent;
        event DragDeltaEventHandle DragDeltaEvent;
        event DragCompletedEventHandle DragCompeletedEvent;

        void OnDragStarted();
        void OnDragDelta();

        void DragCompleted();
    }

}
