using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using ThemeCommons.Controls;
using Path = System.IO.Path;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaktionslogik für StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : DefaultWindow
    {
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                FileName = "Select Folder.",
                InitialDirectory = PathTextBox.Text,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = false
            };
            if (ofd.ShowDialog(this) != true) return;
            PathTextBox!.Text = Path.GetDirectoryName(ofd.FileName) ?? "";
            PathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }

        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType != typeof(double)) return;
            if (e.Column is DataGridTextColumn column) column.Binding.StringFormat = "{0:N3}";
        }
    }
}
