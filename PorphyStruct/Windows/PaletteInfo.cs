using OxyPlot;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace PorphyStruct.Windows
{
    public class PaletteInfo
    {
        public string Title { get; set; }

        public OxyPalette Palette { get; set; }

        public IList<OxyColor> Colors => Palette.Colors;

        public IEnumerable<Brush> Brushes => GetBrushes();

        public IEnumerable<Brush> GetBrushes() => Colors.Select(color => new OxyColorConverter().Convert(color, typeof(Brush), null, null) as Brush ?? System.Windows.Media.Brushes.White);

        public PaletteInfo(string title, OxyPalette palette)
        {
            Title = title;
            Palette = palette;
        }
    }
}
