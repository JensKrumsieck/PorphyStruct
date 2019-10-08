using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct
{
    public static class MarginSetter
    {
        public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);
        public static void SetMargin(DependencyObject obj, Thickness value) => obj.SetValue(MarginProperty, value);

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetter), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));
        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;
            panel.Loaded += new RoutedEventHandler(panel_Loaded);
        }
        static void panel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as Panel;
            // Go over the children and set margin for them:

            foreach (var child in panel.Children)
            {

                var fe = child as FrameworkElement;
                if (fe == null) continue;
                fe.Margin = MarginSetter.GetMargin(panel);
            }
        }
    }
}