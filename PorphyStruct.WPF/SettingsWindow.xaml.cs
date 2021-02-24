using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
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
