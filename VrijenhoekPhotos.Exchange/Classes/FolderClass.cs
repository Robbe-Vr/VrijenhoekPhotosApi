using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class FolderClass
    {
        public string Name { get; set; }
        public List<FileClass> Files { get; set; }
        public List<FolderClass> SubFolders { get; set; }
    }
}
