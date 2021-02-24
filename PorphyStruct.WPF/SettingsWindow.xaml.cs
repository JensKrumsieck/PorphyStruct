using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using System.Windows;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : DefaultWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void Save_OnClick(object sender, RoutedEventArgs e) => Settings.Instance.Save();
    }
}
