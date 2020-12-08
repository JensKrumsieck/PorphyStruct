using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PorphyStruct.Styles
{
    internal static class LocalExtensions
    {
        public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
        {
            if (((FrameworkElement)templateFrameworkElement).TemplatedParent is Window window) action(window);
        }


        public static IntPtr GetWindowHandle(this Window window)
        {
            var helper = new WindowInteropHelper(window);
            return helper.Handle;
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive",
            typeof(bool),
            typeof(LocalExtensions),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static void SetIsActive(DependencyObject element, bool value) => element.SetValue(LocalExtensions.IsActiveProperty, value);
        public static bool GetIsActive(DependencyObject element) => (bool)element.GetValue(LocalExtensions.IsActiveProperty);

    }

    public partial class DefaultWindow
    {
        /// <summary>
        /// Register DependencyObject FileName
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFileName(DependencyObject obj) => (string)obj.GetValue(FileNameProperty);
        public static void SetFileName(DependencyObject obj, string val) => obj.SetValue(FileNameProperty, val);
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.RegisterAttached("FileName",
                typeof(string),
                typeof(DefaultWindow),
                new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Register Dependency Object TitleBarContents
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetTitleBarContents(DependencyObject obj) => obj.GetValue(TitleBarContentsProperty);
        public static void SetTitleBarContents(DependencyObject obj, object val) => obj.SetValue(TitleBarContentsProperty, val);
        public static readonly DependencyProperty TitleBarContentsProperty =
            DependencyProperty.RegisterAttached("TitleBarContents",
                typeof(object),
                typeof(DefaultWindow), null);

        /// <summary>
        /// State Changed on Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Window_Loaded(object sender, RoutedEventArgs e) => ((Window)sender).StateChanged += WindowStateChanged;

        private static void WindowStateChanged(object? sender, EventArgs e)
        {
            var w = ((Window)sender!);
            var handle = w.GetWindowHandle();
            var containerBorder = (Border)w.Template.FindName("Container", w);

            if (w.WindowState == WindowState.Maximized)
            {
                // Make sure window doesn't overlap with the taskbar.
                var screen = System.Windows.Forms.Screen.FromHandle(handle);
                if (screen.Primary)
                {
                    containerBorder.Padding = new Thickness(
                        SystemParameters.WorkArea.Left + 5,
                        SystemParameters.WorkArea.Top + 5,
                        (SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right) + 5,
                        (SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom) + 5);
                }
            }
            else
            {
                containerBorder.Padding = new Thickness(5, 5, 5, 5);
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => sender.ForWindowFromTemplate(SystemCommands.CloseWindow);

        private void MinimizeButtonClick(object sender, RoutedEventArgs e) => sender.ForWindowFromTemplate(SystemCommands.MinimizeWindow);

        private void MaximizeButtonClick(object sender, RoutedEventArgs e) => sender.ForWindowFromTemplate(w =>
                                                                            {
                                                                                if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
                                                                                else SystemCommands.MaximizeWindow(w);
                                                                            });
    }
}
