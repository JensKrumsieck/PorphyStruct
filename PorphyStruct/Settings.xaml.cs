using OxyPlot;
using PorphyStruct.Core.OxyPlot.Custom;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using winforms = System.Windows.Forms;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            DataContext = this;

            var availableThemes = from MethodInfo method in typeof(CustomPalettes).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                  select new PaletteInfo(
                                      method.Name,
                                      (OxyPalette)method.Invoke(null, new object[] { 7 }));
            PaletteList = availableThemes;
        }

        public IEnumerable<PaletteInfo> PaletteList { get; set; }

        /// <summary>
        /// Handle Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Core.Properties.Settings.Default.Save();

        /// <summary>
        /// Handle Folder Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string btn = (sender as Button).Tag.ToString();
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Core.Properties.Settings.Default.savePath))
                initialDir = Core.Properties.Settings.Default.savePath;
            using (winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            })
            {
                if (fbd.ShowDialog() == winforms.DialogResult.OK)
                {
                    var textBox = (FindName($"{btn}Path") as TextBox);
                    textBox.Text = fbd.SelectedPath;
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            }
        }

        /// <summary>
        /// Handles text changed in color textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!CheckColor(tb.Text))
                tb.BorderBrush = Brushes.Red;
            else
            {
                tb.BorderBrush = Brushes.Black;
                tb.Foreground = GetColor(tb.Text);
            }
        }

        private bool CheckColor(string hex)
        {
            if (!hex.StartsWith("#")) return false;
            if (Regex.IsMatch(hex, @"^#([0-9a-fA-F]{6})$")) return true;
            return false;
        }

        private Brush GetColor(string hex)
        {
            BrushConverter con = new BrushConverter();
            return (Brush)con.ConvertFromString(hex);
        }
    }
}
