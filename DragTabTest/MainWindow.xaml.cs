using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DragTabTest.Control.Helper;
using DragTabTest.Core;
using DragTabTest.Util;

namespace DragTabTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Myitem_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb tb = sender as Thumb;
            EffectWindow.ShowEffect(tb);
        }

        private void Myitem_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb tb = sender as Thumb;


            var itemscontrol = tb.GetParentObject() as ItemsControl;

            RoutedEventArgs arg = new RoutedEventArgs(DragHelper.DragDeltaEvent, tb);
            tb.RaiseEvent(arg);
            if (itemscontrol == null)
            {
                EffectWindow.MoveLocation2(e.HorizontalChange, e.VerticalChange);
                //移出itemscontrol范围
                if (DragChache.DragingSource != tb)
                    DragChache.DragingSource = tb;
                return;
            }
            else
            {
                EffectWindow.MoveLocation(e.HorizontalChange, e.VerticalChange);
            }
            if (itemscontrol.Items.Contains(tb))
            {
                if (tb.Height <= e.VerticalChange || -e.VerticalChange >= tb.Height)
                {
                    DragChache.DragingSource = tb;
                    return;
                }
                if (tb.Width / 2 < e.HorizontalChange)
                {
                    var items = itemscontrol.Items;
                    var len = items.Count;
                    var index = items.IndexOf(tb);
                    if (len == index + 1)
                    {
                        return;
                    }
                    items.Remove(tb);
                    items.Insert(index + 1, tb);
                }

                if (tb.Width / 2 < -e.HorizontalChange)
                {
                    var items = itemscontrol.Items;
                    var index = items.IndexOf(tb);
                    if (index == 0) return;
                    items.Remove(tb);
                    items.Insert(index - 1, tb);
                }
            }
        }

        private void Myitem_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            EffectWindow.CloseEffect();

            DragChache.DragingSource = null;
        }

        private void ItemsControl_OnPreviewMouseMove(object sender, RoutedEventArgs e)
        {
            ItemsControl ItemsCon = sender as ItemsControl;

            //
            Console.WriteLine(Mouse.GetPosition(ItemsCon));
            if (DragChache.DragingSource != null)
            {
                if (ItemsCon.Items.Contains(DragChache.DragingSource)) return;
                int insertIndex = -1;
                foreach (var item in ItemsCon.Items)
                {
                    var deobject = item as FrameworkElement;
                    var point = Mouse.GetPosition(deobject);
                    if (point.X < deobject.Width && point.X > 0)
                    {
                        int index = ItemsCon.Items.IndexOf(item);
                        if (point.X <= deobject.Width / 2)
                        {
                            insertIndex = index;
                        }
                        else
                        {
                            insertIndex = index + 1;
                        }
                    }
                }
                if (insertIndex >= 0)
                {
                    var oldItemsControl = ((DependencyObject)DragChache.DragingSource).GetParentObject() as ItemsControl;
                    oldItemsControl.Items.Remove(DragChache.DragingSource);
                    ItemsCon.Items.Insert(insertIndex, DragChache.DragingSource);
                }
                return;
            }
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = sender as ItemsControl;
            if (itemsControl == null) return;
            itemsControl.AddHandler(DragHelper.DragDeltaEvent, new RoutedEventHandler(ItemsControl_OnPreviewMouseMove), true);
        }

    }



    public class EffectWindow : Window
    {
        private static EffectWindow instance = new EffectWindow();

        private FrameworkElement targetElement;
        private EffectWindow()
        {
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.PreviewMouseLeftButtonDown += EffectWindow_PreviewMouseLeftButtonDown;
            this.MouseMove += EffectWindow_MouseMove;
            //this.MouseMove;
            AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler((ss, ee) =>
            {
                //if (this.Visibility != Visibility.Visible) return;
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }

            }), true);
        }

        private void EffectWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void EffectWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SetContent(FrameworkElement content)
        {
            this.Width = content.Width;
            this.Height = content.Height;
            targetElement = content;
            this.Background = new VisualBrush(content);
        }

        public new void Show()
        {
            if (IsLoaded)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                base.Show();
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

            }
        }

        public new void Close()
        {
            this.Hide();
        }

        public static void ShowEffect(FrameworkElement content)
        {
            instance.SetContent(content);
            var pos = content.PointFromScreen(new Point(0, 0));
            instance.Left = -pos.X;
            instance.Top = -pos.Y;
            instance.Show();
        }

        public static void CloseEffect()
        {
            instance.Close();
        }

        public static void MoveLocation(double horizontalOffset, double verticaloff)
        {
            var pos = instance.targetElement.PointFromScreen(new Point(0, 0));
            instance.Left = -pos.X + horizontalOffset;
            instance.Top = -pos.Y + verticaloff;
        }

        public static void MoveLocation2(double horizontalOffset, double verticaloff)
        {
            instance.DragMove();
        }
    }
}
