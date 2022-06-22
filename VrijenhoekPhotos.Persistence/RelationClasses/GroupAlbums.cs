using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Persistence.RelationClasses
{
    public class GroupAlbums
    {
        public int GroupId { get; set; }
        public GroupDTO Group { get; set; }
        public int AlbumId { get; set; }
        public AlbumDTO Album { get; set; }
    }
}
