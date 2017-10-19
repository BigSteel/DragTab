using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DragTab.Control.Helper;
using DragTab.Core;
using DragTab.Util;

namespace DragTab.Control
{
    public class DragItemsControl : ItemsControl
    {
        private static readonly DependencyPropertyKey ItemsPresenterWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ItemsPresenterWidth", typeof(double), typeof(DragItemsControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ItemsPresenterWidthProperty =
            ItemsPresenterWidthPropertyKey.DependencyProperty;

        public double ItemsPresenterWidth
        {
            get { return (double)GetValue(ItemsPresenterWidthProperty); }
            private set { SetValue(ItemsPresenterWidthPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ItemsPresenterHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ItemsPresenterHeight", typeof(double), typeof(DragItemsControl),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ItemsPresenterHeightProperty =
            ItemsPresenterHeightPropertyKey.DependencyProperty;

        public double ItemsPresenterHeight
        {
            get { return (double)GetValue(ItemsPresenterHeightProperty); }
            private set { SetValue(ItemsPresenterHeightPropertyKey, value); }
        }
        static DragItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragItemsControl), new FrameworkPropertyMetadata(typeof(DragItemsControl)));
        }
        internal ContainerCustomisations ContainerCustomisations { get; set; }

        public DragItemsControl()
        {
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(ThumbDragStarted), true);
            AddHandler(Thumb.DragDeltaEvent, new System.Windows.Controls.Primitives.DragDeltaEventHandler(ThumbDragDelta), true);
            AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(ThumbDragCompleted), true);
            DragHelper.DragDeltaEvent += DragHelper_DragDeltaEvent;
        }

        private void DragHelper_DragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            var pos = Mouse.GetPosition(this).ToWpf();
            //鼠标没有在DragImtesControls范围内
            if (!PosInMeasureSize(pos)) return;

            if (DragChache.DragingSource != null)
            {
                //if (Items.Contains(DragChache.DragingSource)) return;
                int insertIndex = -1;
                foreach (var item in Items)
                {
                    var deobject = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(item);
                    var point = Mouse.GetPosition(deobject);

                    if (item == DragChache.DragingSource)
                    {
                        continue;
                    }
                    if (point.X < deobject.ActualWidth && point.X > 0)
                    {
                        int index = Items.IndexOf(item);
                        insertIndex = index;
                    }
                }
                if (insertIndex < 0) return;
                if (Items.Contains(DragChache.DragingSource))
                {
                    if (Items.Count > 1)
                    {
                        var tabitem = DragChache.DragingSource;
                        var items = this.TryFindParent<DragTabControl>().Items;
                        items.Remove(tabitem);

                        insertIndex = Math.Max(insertIndex, 0);
                        insertIndex = Math.Min(insertIndex, items.Count);
                        items.Insert(insertIndex, tabitem);
                        ThumbDragCompleted(this, null);
                    }
                }
                else
                {
                    var oldItemsControl = ((DependencyObject)DragChache.DragingSource).GetParentObject() as ItemsControl;
                    if (ItemsSource == null) return;
                    oldItemsControl.Items.Remove(DragChache.DragingSource);

                    ((ItemCollection)ItemsSource)?.Insert(insertIndex, DragChache.DragingSource);
                    if (oldItemsControl.Items.Count == 0)
                    {
                        Window.GetWindow(oldItemsControl)?.Close();
                    }
                    ThumbDragCompleted(null, null);
                    Window.GetWindow(this).Activate();
                }
            }
        }

        private void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            DragTabItem container = null;
            Thumb tb = e.OriginalSource as Thumb;

            if (tb != null)
            {
                container = tb.TryFindParent<DragTabItem>();
            }
            DragChache.DragingSource = container.Content;
            DragChache.PointInElement = Mouse.GetPosition(container);
            if (Items.Count == 1)
            {
                var win = Window.GetWindow(this);
                win.Opacity = 0;
                //win.Visibility = Visibility.Collapsed;
            }
            EffectWindow.ShowEffect(container);
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            DragTabItem container = null;
            Thumb tb = e.OriginalSource as Thumb;
            if (tb != null)
            {
                container = tb.TryFindParent<DragTabItem>();
            }
            var itemscontrol = this;
            if (!Items.Contains(container.Content)) return;
            this.RaiseDragDeltaEvent(e);
            EffectWindow.MoveLocation(e.HorizontalChange, e.VerticalChange);

            //if (itemscontrol.Items.Contains(container.Content))
            //{
            //if (container.ActualHeight <= e.VerticalChange || -e.VerticalChange >= container.ActualHeight)
            //{
            //DragChache.DragingSource = container.Content;
            //return;
            //}
            //var pos = Mouse.GetPosition(this).ToWpf();
            //鼠标没有在DragImtesControls范围内
            //if (!PosInMeasureSize(pos)) return;
            //if (container.ActualWidth / 2 < e.HorizontalChange)
            //{
            //    var items =
            //        itemscontrol.TryFindParent<DragTabControl>().Items;
            //    var len = items.Count;
            //    var index = items.IndexOf(container.Content);
            //    if (len == index + 1)
            //    {
            //        return;
            //    }
            //    var tabitem = container.Content;
            //    items.Remove(tabitem);
            //    items.Insert(index + 1, tabitem);
            //    ThumbDragCompleted(this, null);
            //}

            //if (container.ActualWidth / 2 < -e.HorizontalChange)
            //{
            //    var items = itemscontrol.TryFindParent<DragTabControl>().Items;
            //    var index = items.IndexOf(container.Content);
            //    if (index == 0) return;
            //    var tabitem = container.Content;
            //    items.Remove(tabitem);
            //    items.Insert(index - 1, tabitem);
            //    ThumbDragCompleted(this, null);
            //}
            //}
        }

        private void ThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            EffectWindow.CloseEffect();

            if (args != null && !MousePosInMeasureSize())
            {

                this.RaiseDragCompletedEvent(new DragTabCompletedEventArgs()
                {
                    TabItem = (TabItem)DragChache.DragingSource,
                    IsAtTargetDragTabControl = false,
                    SourceTabControl = this.TryFindParent<DragTabControl>(),
                    MouseLastPoint = Native.GetCursorPos(),
                    StartPointInElement = DragChache.PointInElement
                });
            }
            if (Items.Count == 0)
            {
                Window.GetWindow(this)?.Close();
            }
            if (Items.Count == 1)
            {
                var win = Window.GetWindow(this);
                win.Opacity = 1;
                //win.Visibility = Visibility.Visible;
            }
            DragChache.DragingSource = null;
        }

        private bool PosInMeasureSize(Point pos)
        {
            return pos != null && pos.X > 0 && pos.Y > 0 && pos.X < ActualWidth && pos.Y < ActualHeight;
        }

        private bool MousePosInMeasureSize()
        {
            var pos = Mouse.GetPosition(this).ToWpf();
            //鼠标没有在DragImtesControls范围内
            return PosInMeasureSize(pos);
            //return Mouse.Capture(this);
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DragTabItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var result = ContainerCustomisations != null && ContainerCustomisations.GetContainerForItemOverride != null
                ? ContainerCustomisations.GetContainerForItemOverride()
                : new DragTabItem();

            //result.SizeChanged += ItemSizeChangedEventHandler;
            //return base.GetContainerForItemOverride();
            return result;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (ContainerCustomisations != null && ContainerCustomisations.PrepareContainerForItemOverride != null)
                ContainerCustomisations.PrepareContainerForItemOverride(element, item);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
