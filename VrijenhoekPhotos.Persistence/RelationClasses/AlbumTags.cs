using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Persistence.RelationClasses
{
    public class AlbumTags
    {
        public int AlbumId { get; set; }
        public AlbumDTO Album { get; set; }
        public int TagId { get; set; }
        public TagDTO Tag { get; set; }
    }
}
