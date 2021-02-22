using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct.WPF.Template
{
    public partial class CoordinatesTab : ResourceDictionary
    {
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.ScrollIntoView(grid.SelectedItem);
        }
    }
}
