using System.Collections.Generic;

namespace Pablo.Classes.Interfaces
{
    internal interface IFileService
    {
        IEnumerable<string> LoadFiles(string folderPath, HashSet<string> extensions);
    }
}