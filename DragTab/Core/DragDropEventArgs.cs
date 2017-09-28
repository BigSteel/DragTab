using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragTab.Core
{
    public class DragDropEventArgs : EventArgs
    {
        public IDragSource DragSource { get; }

        public DragDropEventArgs(IDragSource dragSource)
        {
            DragSource = dragSource;
        }
    }
}
