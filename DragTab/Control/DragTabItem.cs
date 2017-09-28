using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DragTab.Control
{
    [TemplatePart(Name = ThumbPartName, Type = typeof(Thumb))]
    public class DragTabItem : ContentControl
    {
        public const string ThumbPartName = "PART_Thumb";

        private Thumb _partThumb;
        static DragTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragTabItem), new FrameworkPropertyMetadata(typeof(DragTabItem)));
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(DragTabItem), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsDragingProperty = DependencyProperty.Register(
            "IsDraging", typeof(bool), typeof(DragTabItem), new PropertyMetadata(default(bool)));

        public bool IsDraging
        {
            get { return (bool)GetValue(IsDragingProperty); }
            set { SetValue(IsDragingProperty, value); }
        }

        /// <summary>
        /// 在ItemsControl中顺序
        /// </summary>
        public int Index
        {
            get { return MeasureIndex(); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _partThumb = GetTemplateChild(ThumbPartName) as Thumb;
            if (_partThumb != null)
            {
                _partThumb.DragStarted += _partThumb_DragStarted;
                _partThumb.DragDelta += _partThumb_DragDelta;
                _partThumb.DragCompleted += _partThumb_DragCompleted;
            }
        }

        private void _partThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            IsDraging = false;
        }

        private void _partThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            IsDraging = true;
        }

        private void _partThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            IsSelected = true;
            if (Content is TabItem)
            {
                ((TabItem)Content).IsSelected = true;
            }
        }

        private int MeasureIndex()
        {
            return ItemsControl.GetItemsOwner(this)?.Items.IndexOf(this) ?? -1;
        }
    }
}
