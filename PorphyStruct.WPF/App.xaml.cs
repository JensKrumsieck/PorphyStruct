using System.Windows;
using System.Windows.Threading;

namespace PorphyStruct.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Handles Unhandled Exception
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show("An unexpected exception has occurred. Shutting down the application. \nMessage: " + e.Exception.Message, "Unhandled Exception",
            MessageBoxButton.OK, MessageBoxImage.Error);
        // Prevent default unhandled exception processing
        e.Handled = true;
        Environment.Exit(0);
    }
}
