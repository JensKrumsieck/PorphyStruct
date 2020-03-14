using HelixToolkit.Wpf;
using PorphyStruct.Chemistry;
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
        public string FileName => pathTextBox?.Text;

        /// <summary>
        /// Return Macrocycle Type from ComboBox
        /// </summary>
        public int Type => TypeListBox.SelectedIndex;

        private void pathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Refresh();

        private void TypeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => Refresh();

        /// <summary>
        /// Refresh GUI
        /// </summary>
        public void Refresh()
        {
            if (Type != -1 && !string.IsNullOrEmpty(FileName))
            {
                MolViewer.Children.Clear();
                MolViewer.Items.Add(new DefaultLights());
                Macrocycle.Type type = (Macrocycle.Type)TypeListBox.SelectedIndex;
                var tmp = MacrocycleFactory.Load(pathTextBox.Text, type);
                tmp.Center(s => true);
                foreach (var obj in tmp.Paint3D()) MolViewer.Items.Add(obj);
            }
        }

    }
}
