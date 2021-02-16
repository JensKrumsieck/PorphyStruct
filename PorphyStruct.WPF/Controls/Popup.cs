using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct.WPF.Controls
{
    public class Popup : ContentControl
    {
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Popup));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        static Popup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Popup), 
                new FrameworkPropertyMetadata(typeof(Popup)));
        }
    }
}