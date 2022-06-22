using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.FileSystem
{
    public static class FileSystemReader
    {
        private static string picturesFolderPath = Path.Combine(ApplicationEnvironmentData.InDevelopment ? "F:\\VrijenhoekPhotos" : "/app", "Data", "Pictures");

        public static string PicturesBaseFolder { get { return picturesFolderPath; } }

        public static FolderClass ReadPicturesFileSystemFolder(string folder)
        {
            return EnumerateFolderContents(Path.Combine(picturesFolderPath, folder));
        }

        public static FileClass ReadFileFromPicturesFileSystemFolder(string folder, string file)
        {
            string filePath = Path.Combine(picturesFolderPath, folder, file);
            return File.Exists(filePath) ? new FileClass(filePath) : null;
        }

        private static FolderClass EnumerateFolderContents(string folderPath)
        {
            FolderClass folder = new FolderClass()
            {
                Name = Path.GetFileNameWithoutExtension(folderPath),
                Files = new List<FileClass>(),
                SubFolders = new List<FolderClass>(),
            };

            foreach (string entry in Directory.EnumerateFileSystemEntries(folderPath))
            {
                if (File.Exists(entry) && FileTypes.AllowedFileTypes.Any(x => x == Path.GetExtension(entry)))
                {
                    folder.Files.Add(new FileClass(entry));
                }
                else if (Directory.Exists(entry))
                {
                    folder.SubFolders.Add(new FolderClass() { Name = Path.GetFileNameWithoutExtension(entry) });
                }
            }

            return folder;
        }
    }
}
