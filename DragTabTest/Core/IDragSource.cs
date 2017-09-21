using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragTabTest.Core
{
    public interface IDragSource : IDragDropHandle
    {
        /// <summary>
        /// Source
        /// </summary>
        object DragSource { get; set; }

    }
}
