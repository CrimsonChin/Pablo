using Pablo.ViewModels;
using System.Collections.Generic;

namespace Pablo.Classes.Interfaces
{
    internal interface IPersistenceService
    {
        void Save(string directory, IEnumerable<ImageViewModel> images);
        IEnumerable<ImageViewModel> Load(string directory);
    }
}