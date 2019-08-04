using System.Windows;

namespace PorphyStruct
{
	/// <summary>
	/// Interaktionslogik für Info.xaml
	/// </summary>
	public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
			DataContext = this;
        }

		/// <summary>
		/// gets current software version
		/// </summary>
		public string AssemblyVersion
		{
			get
			{
				return System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ResourceAssembly.Location).ProductVersion;
			}
		}

		/// <summary>
		/// navigates browser to URI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Uri.ToString());
		}
	}
}
