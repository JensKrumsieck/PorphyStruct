using System.Linq;
using System.Windows.Input;
using HelixToolkit.Wpf;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.Windows;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DefaultWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MacrocycleViewModel();
        }

        /// <summary>
        /// Handles Atom selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewport3D_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hits = Viewport3D.Viewport.FindHits(e.GetPosition(Viewport3D));
            foreach (var hit in hits.OrderBy(s => s.Distance))
            {
                if(hit.Visual.GetType() != typeof(AtomVisual3D)) continue;
                var av3d = hit.Visual as AtomVisual3D;
                ((MacrocycleViewModel) DataContext).SelectedItem = av3d?.Atom;
            }
        }
    }
}
