using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Windows;

namespace PorphyStruct.Files
{
    /// <summary>
    /// Export File Type for SaveWindow
    /// </summary>
    public class ExportFileType
    {
        public string Title { get; set; }
        public PackIcon Icon { get; set; }
        public PackIcon Secondary { get; set; }
        public string Extension { get; set; }

        public bool IsEnabled => Application.Current.Windows.OfType<SaveWindow>().First().HasData(Title); 

        public override string ToString() => Title + "." + Extension;
    }
}
