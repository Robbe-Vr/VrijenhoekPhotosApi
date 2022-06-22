using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public static class FileTypes
    {
        public static IEnumerable<string> AllowedFileTypes { get { return AllowedPictureFileTypes.Concat(AllowedVideoFileTypes); } }
        public static IEnumerable<string> AllowedPictureFileTypes { get; } = new List<string>()
                                                                            {
                                                                                ".webp",
                                                                                ".tif",
                                                                                ".tiff",
                                                                                ".svg",
                                                                                ".ico",
                                                                                ".gif",
                                                                                ".avif",
                                                                                ".bmp",
                                                                                ".jpg",
                                                                                ".jpeg",
                                                                                ".png",
                                                                            };
        public static IEnumerable<string> AllowedVideoFileTypes { get; } = new List<string>()
                                                                            {
                                                                                ".ogv",
                                                                                ".avi",
                                                                                ".vob",
                                                                                ".mov",
                                                                                ".mkv",
                                                                                ".mpeg",
                                                                                ".webm",
                                                                                ".mp4",
                                                                            };
    }
}
