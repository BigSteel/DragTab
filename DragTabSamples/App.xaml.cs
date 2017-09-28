using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DragTab.Control.Helper;

namespace DragTabSamples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
        protected override void OnStartup(StartupEventArgs e)
        {
            DragHelper.DragCompletedEvent += DragHelper_DragCompletedEvent;
            MainWindow win = new MainWindow();
            win.Show();
            MainWindow win2 = new MainWindow();
            win2.Show();
        }

        private void DragHelper_DragCompletedEvent(object sender, DragTab.Control.DragTabCompletedEventArgs args)
        {
            MainWindow win = new MainWindow();
            win.WindowStyle = WindowStyle.None;
            win.AllowsTransparency = true;
            win.DragTabControl.Items.Clear();
            win.DragTabControl.Items.Add(args.TabItem);
            POINT point = new POINT();
            GetCursorPos(out point);
            win.Left = point.X;
            win.Top = point.Y;
            win.Show();
        }
    }
}
