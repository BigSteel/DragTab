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
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            DragHelper.DragCompletedEvent += DragHelper_DragCompletedEvent;
            MainWindow win = new MainWindow();
            win.Show();
            MainWindow win2 = new MainWindow();
            win2.Show();
        }

        private void DragHelper_DragCompletedEvent(object sender, DragTab.Control.DragTabCompletedEventArgs args)
        {
            MainWindow win = new MainWindow();
            win.DragTabControl.Items.Clear();
            win.DragTabControl.Items.Add(args.TabItem);
            var point = args.StartPointInElement;
            win.Left = args.MouseLastPoint.X - point.X;
            win.Top = args.MouseLastPoint.Y - 20 - point.Y;
            win.Show();
        }
    }
}
