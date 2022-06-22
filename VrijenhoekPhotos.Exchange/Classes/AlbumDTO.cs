using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class AlbumDTO : IIdentifier<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public int? IconPhotoId { get; set; }
        public PhotoDTO IconPhoto { get; set; }

        public ICollection<TagDTO> Tags { get; set; }
        public ICollection<PhotoDTO> Photos { get; set; }
        public ICollection<GroupDTO> Groups { get; set; }

        public string UserId { get; set; }
        public UserDTO User { get; set; }
    }
}
