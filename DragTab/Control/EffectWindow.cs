using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragTab.Control
{
    internal class EffectWindow : Window
    {
        private static EffectWindow instance = new EffectWindow();

        private FrameworkElement targetElement;
        private EffectWindow()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            Topmost = true;
            IsHitTestVisible = false;
        }

        private void SetContent(FrameworkElement content)
        {
            this.Width = content.ActualWidth;
            this.Height = content.ActualHeight;
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
            if (((ContentControl)instance.targetElement).Content is TabItem)
            {
                var pos = instance.targetElement.PointFromScreen(new Point(0, 0));
                instance.Left = -pos.X + horizontalOffset;
                instance.Top = -pos.Y + verticaloff;
            }
        }

        public static void MoveLocation2(double horizontalOffset, double verticaloff)
        {
            instance.DragMove();
        }
    }
}
