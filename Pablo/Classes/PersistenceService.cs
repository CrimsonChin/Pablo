using Pablo.Classes.Interfaces;
using Pablo.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Pablo.Classes
{
    internal class PersistenceService : IPersistenceService
    {
        private const string PersistenceFile = "Pablo.xml";

        public void Save(string directory, IEnumerable<ImageViewModel> images)
        {
            var favs = new XElement("Images");
            foreach (var image in images)
            {
                favs.Add(image.ToXml());
            }

            var path = Path.Combine(directory, PersistenceFile);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            favs.Save(path);
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
        }

        public IEnumerable<ImageViewModel> Load(string directory)
        {
            var images = new List<ImageViewModel>();

            var path = Path.Combine(directory, PersistenceFile);
            if (File.Exists(path) == false)
            {
                return images;
            }

            var persistenceDoc = XElement.Load(path);
            foreach (var item in persistenceDoc.Elements())
            {
                var imageViewModel = new ImageViewModel(item);
                if (File.Exists(imageViewModel.FilePath))
                {
                    images.Add(imageViewModel);
                }
            }

            return images;
        }
    }
}
