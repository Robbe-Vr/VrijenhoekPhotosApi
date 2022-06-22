using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class GroupDTO : IIdentifier<int>
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreationDate { get; set; }
        public int? IconPhotoId { get; set; }
        public PhotoDTO IconPhoto { get; set; }

        public ICollection<AlbumDTO> Albums { get; set; }
        public ICollection<UserDTO> Users { get; set; }
        public ICollection<UserDTO> PendingJoinUsers { get; set; }
        public ICollection<UserDTO> RequestedJoinUsers { get; set; }

        public string CreatorId { get; set; }
        public UserDTO Creator { get; set; }
    }
}
