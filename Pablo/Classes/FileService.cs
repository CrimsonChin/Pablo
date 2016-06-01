using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pablo.Classes.Interfaces;

namespace Pablo.Classes
{
    internal class FileService : IFileService
    {
        IEnumerable<string> IFileService.LoadFiles(string folderPath, HashSet<string> extensions)
        {
            return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Contains(Path.GetExtension(s)));
        }
    }
}
