using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;
using ThemeCommons.Controls;
using Path = System.IO.Path;

namespace PorphyStruct.WPF;

public partial class BatchWindow : DefaultWindow
{
    public BatchWindow()
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

    private void TypeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (BatchViewModel)DataContext;
        vm.Type = (MacrocycleType)TypeList.SelectedIndex;
    }

    private async void Process_OnClick(object sender, RoutedEventArgs e)
    {
        MainGrid.IsEnabled = false;
        Results.Visibility = Visibility.Visible;
        var vm = (BatchViewModel)DataContext;
        await vm.Process();
        MainGrid.IsEnabled = true;
        var stats = new StatisticsViewModel()
        {
            WorkingDir = vm.WorkingDir,
            IsRecursive = vm.IsRecursive
        };
        new StatisticsWindow()
        {
            DataContext = stats
        }.Show();
    }
}
