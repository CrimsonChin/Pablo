using System.IO;

namespace Pablo.ViewModels
{
    internal class ImageViewModel
    {
        public ImageViewModel(string filePath)
        {
            FilePath = filePath;
        }

        public string FileName => Path.GetFileName(FilePath);

        public string FilePath { get; }

        public bool IsFavourite { get; set; }
    }
}