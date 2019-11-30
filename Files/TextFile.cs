using System;

namespace PorphyStruct.Files
{
    abstract class TextFile
    {
        /// <summary>
        /// the files path
        /// </summary>
        public string Path = "";
        /// <summary>
        /// the file's content as string
        /// </summary>
        public string Content = "";

        /// <summary>
        /// the file's content as array of lines
        /// </summary>
        public string[] Lines;

        /// <summary>
        /// Constructor creates needed variables
        /// </summary>
        /// <param name="path"></param>
        public TextFile(string path)
        {
            this.Path = path;
            // read cif - File and get parameters & coordinates
            Content = System.IO.File.ReadAllText(this.Path);
            Lines = Content.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None);
        }
    }
}
