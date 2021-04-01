using HelixToolkit.Wpf;
using PorphyStruct.ViewModel;
using System;
using System.Windows;

namespace PorphyStruct.WPF
{
    public static class ExportHelper
    {
        public static void ExportViewport(this AnalysisViewModel viewModel, string path)
        {
            //get instance of MainWindow
            var main = (MainWindow)Application.Current.MainWindow;
            var viewPort = main?.Viewport3D;

            //try to render current 3d representation
            if (viewPort == null) throw new InvalidOperationException("Viewport not found - can not render properly");
            viewPort.Viewport.SaveBitmap(path + "_viewport.png", null, 4);
        }
    }
}
