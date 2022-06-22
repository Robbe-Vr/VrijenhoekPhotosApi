using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class FileClass
    {
        public FileClass() { }
        public FileClass(string location)
        {
            Location = location;
        }

        public DateTime CreationDate { get { return File.GetCreationTimeUtc(Location); } }
        public string Location { get; set; }
        public string Name { get { return Path.GetFileNameWithoutExtension(Location); } }
        public string Extension { get { return Path.GetExtension(Location); } }
        public bool IsImage { get
            {
                return (new string[]
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
                    })
                    .Any(x => x == Extension.ToLower());
            }
        }
    }
}
