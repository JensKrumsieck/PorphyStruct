namespace PorphyStruct.ViewModel.IO
{
    public readonly struct ExportFileType
    {
        public string Title { get; }
        public string Icon { get; }
        public string Extension { get; }

        public ExportFileType(string title, string icon, string extension)
        {
            Title = title;
            Icon = icon;
            Extension = extension;
        }
    }
}
