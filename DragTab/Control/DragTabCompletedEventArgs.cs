using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DragTab.Control
{
    public class DragTabCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 被拖拽的tabitem
        /// </summary>
        public TabItem TabItem { get; internal set; }
        /// <summary>
        /// 是否被拖拽进入一个DragTabControl中
        /// </summary>
        public bool IsAtTargetDragTabControl { get; internal set; }

        /// <summary>
        /// Tabitem 之前所在TabControl
        /// </summary>
        public DragTabControl SourceTabControl { get; internal set; }

        /// <summary>
        /// Tabitem被拖拽进入的TabControl,如果不存在则为null
        /// </summary>
        public DragTabControl TargetTabControl { get; internal set; }

        public Point MouseLastPoint { get; set; }

        /// <summary>
        /// Mouse在DragTabItem的初始位置
        /// </summary>
        public Point StartPointInElement { get; set; }
    }
}
