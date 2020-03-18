namespace PorphyStruct.Windows
{
    /// <summary>
    /// Export File Type for SaveWindow
    /// </summary>
    public class ExportFileType
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string[] Extension { get; set; }

        public string Extensions => string.Join(", ", Extension);

        public override string ToString() => Title + "." + Extension;
    }
}