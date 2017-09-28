using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            var pos = Mouse.GetPosition(args.TabItem);
            win.Show();
        }
    }
}
