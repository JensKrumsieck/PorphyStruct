using System;
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
            savePath.Text = PorphyStruct.Core.Settings.Default.savePath;
            importPath.Text = PorphyStruct.Core.Settings.Default.importPath;
        }

        /// <summary>
        /// Handle Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PorphyStruct.Core.Settings.Default.Save();
        }

        /// <summary>
        /// Handle Folder Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string btn = (sender as Button).Tag.ToString();
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(PorphyStruct.Core.Settings.Default.savePath))
                initialDir = PorphyStruct.Core.Settings.Default.savePath;
            using (winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            })
            {
                if (fbd.ShowDialog() == winforms.DialogResult.OK)
                {
                    (FindName($"{btn}Path") as TextBox).Text = fbd.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            PorphyStruct.Core.Settings.Default.savePath = savePath.Text;
            PorphyStruct.Core.Settings.Default.importPath = importPath.Text;
            PorphyStruct.Core.Settings.Default.lineThickness = Convert.ToDouble(lineThickness.Text, System.Globalization.CultureInfo.InvariantCulture);
            PorphyStruct.Core.Settings.Default.minY = Convert.ToDouble(minY.Text, System.Globalization.CultureInfo.InvariantCulture);
            PorphyStruct.Core.Settings.Default.maxY = Convert.ToDouble(maxY.Text, System.Globalization.CultureInfo.InvariantCulture);
            PorphyStruct.Core.Settings.Default.defaultFont = defaultFont.Text;
            PorphyStruct.Core.Settings.Default.xAxis = xAxis.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.zero = zero.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.autoscaleY = autoscaleY.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.autoscaleX = autoscaleX.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.defaultFontSize = Convert.ToInt32(fontSize.Text);
            PorphyStruct.Core.Settings.Default.pngRes = Convert.ToInt32(pngRes.Text);
            PorphyStruct.Core.Settings.Default.pngWidth = Convert.ToInt32(pngWidth.Text);
            PorphyStruct.Core.Settings.Default.pngHeight = Convert.ToInt32(pngHeight.Text);
            PorphyStruct.Core.Settings.Default.markerType = (OxyPlot.MarkerType)markerType.SelectedIndex;
            PorphyStruct.Core.Settings.Default.simMarkerType = (OxyPlot.MarkerType)simMarkerType.SelectedIndex;
            PorphyStruct.Core.Settings.Default.com1MarkerType = (OxyPlot.MarkerType)com1MarkerType.SelectedIndex;
            PorphyStruct.Core.Settings.Default.com2MarkerType = (OxyPlot.MarkerType)com2MarkerType.SelectedIndex;
            PorphyStruct.Core.Settings.Default.rotateTitle = rotateTitle.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.showBox = showBox.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.singleColor = singleColor.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.useImportExportPath = useImportExportPath.IsChecked.Value;
            PorphyStruct.Core.Settings.Default.titleFormat = titleFormat.Text;
            PorphyStruct.Core.Settings.Default.dontMark = dontMark.Text.Replace(" ", "");
            PorphyStruct.Core.Settings.Default.color1 = CheckColor(color1.Text) ? color1.Text : "#000000";
            PorphyStruct.Core.Settings.Default.color2 = CheckColor(color2.Text) ? color2.Text : "#ff0000";
            PorphyStruct.Core.Settings.Default.color3 = CheckColor(color3.Text) ? color3.Text : "#00ff40";
            PorphyStruct.Core.Settings.Default.color4 = CheckColor(color4.Text) ? color4.Text : "#0101DF";
            PorphyStruct.Core.Settings.Default.color5 = CheckColor(color5.Text) ? color5.Text : "#FF8000";
            PorphyStruct.Core.Settings.Default.Save();
            this.Close();
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
