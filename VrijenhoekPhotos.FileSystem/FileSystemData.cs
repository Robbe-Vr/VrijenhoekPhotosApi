using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange;

namespace VrijenhoekPhotos.FileSystem
{
    internal static class FileSystemData
    {
        internal static string picturesFolderPath
        {
            get
            {
                return Path.Combine(ApplicationEnvironmentData.InDevelopment ? "F:\\VrijenhoekPhotos" : "/app", "Data", "Pictures");
            }
        }
    }
}
