using System;
using System.IO;
using System.Xml.Linq;

namespace Pablo.ViewModels
{
    internal class ImageViewModel
    {
        public ImageViewModel(string filePath, bool isFavourite = false)
        {
            FilePath = filePath;
            IsFavourite = isFavourite;
        }

        public ImageViewModel(XElement xml)
        {
            var attribute = xml.Attribute(nameof(FilePath));
            if (attribute != null)
            {
                FilePath = attribute.Value;
            }

            attribute = xml.Attribute(nameof(IsFavourite));
            if (attribute != null)
            {
                IsFavourite = Convert.ToBoolean(attribute.Value);
            }
        }

        public string FileName => Path.GetFileName(FilePath);

        public string FilePath { get; }

        public bool IsFavourite { get; set; }

        public XElement ToXml()
        {
            return new XElement(nameof(ImageViewModel),
                new XAttribute(nameof(FilePath), FilePath),
                new XAttribute(nameof(IsFavourite), IsFavourite));
        }
    }
}