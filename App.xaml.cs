/* 2021/4/16 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TerminalMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            Debug.WriteLine($"Unhandled exception: {e.Exception.Message}");
            Debug.WriteLine(e.Exception.StackTrace);
            MessageBox.Show("A unhandled exception just occurred.", "Unhandled Exception",
                MessageBoxButton.OK, MessageBoxImage.Warning);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
