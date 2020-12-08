namespace PorphyStruct.Windows
{
    /// <summary>
    /// Export File Type for SaveWindow
    /// </summary>
    public class ExportFileType
    {
        public ExportFileType(string title, string icon, string[] extension)
        {
            Title = title;
            Icon = icon;
            Extension = extension;
        }

        public string Title { get; set; }
        public string Icon { get; set; }
        public string[] Extension { get; set; }

        public string Extensions => string.Join(", ", Extension);

        public override string ToString() => Title + "." + Extension;
    }
}