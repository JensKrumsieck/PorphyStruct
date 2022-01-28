using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OxyPlot;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF;

public partial class SettingsWindow : DefaultWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        Settings.Save();
        Close();
    }
    private void Search_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (sender as Button)?.Tag.ToString()!;
        var textBox = (FindName($"{btn}Path") as TextBox);
        var ofd = new OpenFileDialog()
        {
            FileName = "Select Folder.",
            InitialDirectory = textBox?.Text ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            ValidateNames = false,
            CheckFileExists = false,
            CheckPathExists = false
        };
        if (ofd.ShowDialog(this) != true) return;
        textBox!.Text = Path.GetDirectoryName(ofd.FileName) ?? "";
        textBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }

    private void ColorAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var color = AddColor.Text;
        var actualColor = OxyColor.Parse(color);
        var vm = (SettingsViewModel)DataContext;
        vm.ComparisonColors.Add(actualColor);
    }
}
