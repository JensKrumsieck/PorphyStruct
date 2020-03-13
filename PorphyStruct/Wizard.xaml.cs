using PorphyStruct.Windows;
using System.IO;
using System.Windows;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für Wizard.xaml
    /// </summary>
    public partial class Wizard : Window
    {
        public Wizard() => InitializeComponent();

        /// <summary>
        /// Handle Cancel Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Handle OK Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //check for file
            if (!string.IsNullOrWhiteSpace(pathTextBox.Text) && File.Exists(pathTextBox.Text) && TypeListBox.SelectedIndex != -1) DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handle FileOpen Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = FileUtil.DefaultOpenFileDialog("Supported Files(*.cif, *.xyz, *.mol2/*.mol, *.ixyz)|*.cif;*.xyz;*.mol2;*.mol;*.ixyz|Crystallographic Information Files (*.cif)|*.cif|Cartesian Coordinate Files (*.xyz)|*.xyz|Mol2 Files (*.mol2, *.mol)|*.mol2;*.mol|XYZ with Identifier (by PorphyStruct) (*.ixyz)|*.ixyz");
            var DialogResult = ofd.ShowDialog();
            if (DialogResult.HasValue && DialogResult.Value) pathTextBox.Text = ofd.FileName;
        }

        /// <summary>
        /// Returns the filename
        /// </summary>
        public string FileName => pathTextBox.Text;

        /// <summary>
        /// Return Macrocycle Type from ComboBox
        /// </summary>
        public int Type => TypeListBox.SelectedIndex;
    }
}
