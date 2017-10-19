using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragTab.Core
{
    internal class DragChache
    {
        public static object DragingSource { get; set; }
        public static Point PointInElement { get; set; }
    }
}
