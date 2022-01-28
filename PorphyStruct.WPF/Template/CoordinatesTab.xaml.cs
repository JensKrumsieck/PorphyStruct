using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct.WPF.Template;

public partial class CoordinatesTab : ResourceDictionary
{
    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var grid = (DataGrid)sender;
        if (grid.SelectedItem == null) return;
        grid.ScrollIntoView(grid.SelectedItem);
    }
}
