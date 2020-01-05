using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Windows;

namespace PorphyStruct.Files
{

    public class ExportFileType
    {
        public string Title { get; set; }
        public PackIcon Icon { get; set; }
        public PackIcon Secondary { get; set; }
        public string Extension { get; set; }

        public bool IsEnabled
        {
            get
            {
                SaveWindow sw = Application.Current.Windows.OfType<SaveWindow>().First();
                return sw.HasData(Title);
            }
        }

        public override string ToString()
        {
            return Title + "." + Extension;
        }
    }
}
