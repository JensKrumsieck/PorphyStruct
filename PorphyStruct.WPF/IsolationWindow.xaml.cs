using System.Windows;
using System.Windows.Input;
using ChemSharp.Molecules.HelixToolkit;
using HelixToolkit.Wpf;
using PorphyStruct.ViewModel.Windows;
using ThemeCommons.Controls;
using MacrocycleViewModel = PorphyStruct.ViewModel.Windows.MacrocycleViewModel;

namespace PorphyStruct.WPF;

/// <summary>
/// Interaktionslogik für IsolationWindow.xaml
/// </summary>
public partial class IsolationWindow : DefaultWindow
{
    public MainWindow Context;

    // ReSharper disable once SuggestBaseTypeForParameter
    public IsolationWindow(MainWindow ctx)
    {
        InitializeComponent();
        Context = ctx;
        DataContext = new IsolationViewModel((MacrocycleViewModel)ctx.DataContext);
    }

    private void Viewport3D_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var vm = (IsolationViewModel)DataContext;
        var hits = Viewport3D.Viewport.FindHits(e.GetPosition(Viewport3D));
        foreach (var hit in hits.OrderBy(s => s.Distance))
        {
            if (hit.Visual.GetType() != typeof(Atom3D)) continue;
            var av3d = hit.Visual as Atom3D;
            if (!vm.Isolation.Contains(av3d?.Atom)) vm.Isolation.Add(av3d?.Atom);
        }
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var file = (DataContext as IsolationViewModel)?.Save();
        Context.OpenFile(file);
        Close();
    }
}
