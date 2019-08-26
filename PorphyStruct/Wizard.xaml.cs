using Microsoft.Win32;
using System;
using System.Windows;

namespace PorphyStruct
{
	/// <summary>
	/// Interaktionslogik für Wizard.xaml
	/// </summary>
	public partial class Wizard : Window
    {
        public Wizard()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Handle Cancel Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		/// <summary>
		/// Handle OK Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

		/// <summary>
		/// Handle FileOpen Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
			string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (Properties.Settings.Default.savePath != "")
				initialDir = Properties.Settings.Default.savePath;
			OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "Supported Files (*.cif, *.xyz, *.mol2/*.mol, *.ixyz)|*.cif;*.xyz;*.mol2;*.mol;*.ixyz|Crystallographic Information Files (*.cif)|*.cif|Cartesian Coordinate Files (*.xyz)|*.xyz|Mol2 Files (*.mol2, *.mol)|*.mol2;*.mol|XYZ with Identifier (by PorphyStruct) (*.ixyz)|*.ixyz",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                pathTextBox.Text = ofd.FileName;
            }
        }

		/// <summary>
		/// Returns the filename
		/// </summary>
        public string getFileName
        {
            get { return pathTextBox.Text; }
        }

		/// <summary>
		/// Return Macrocycle Type from ComboBox
		/// </summary>
        public int getType
        {
            //0 = corrole, 1 = porphyrin, 2 = norcorrole, 3 = corrphycene, 4 = porphycene
            get { return TypeListBox.SelectedIndex; }
        }
    }
}
