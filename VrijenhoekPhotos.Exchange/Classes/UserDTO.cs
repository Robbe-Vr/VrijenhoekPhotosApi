using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class UserDTO : IIdentifier<string>
    {
        public UserDTO() { }
        public UserDTO(UserDTO user)
        {
            Id = user.Id;
            Email = user.Email;
            UserName = user.UserName;
            Rights = user.Rights;
            CreationDate = user.CreationDate;

            Groups = user.Groups;
            OwnedGroups = user.OwnedGroups;
            PendingJoinGroups = user.PendingJoinGroups;
            RequestedJoinGroups = user.RequestedJoinGroups;
            Albums = user.Albums;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public Rights Rights { get; set; }
        public DateTime CreationDate { get; set; }

        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }

        public ICollection<GroupDTO> Groups { get; set; }
        public ICollection<GroupDTO> OwnedGroups { get; set; }
        public ICollection<GroupDTO> PendingJoinGroups { get; set; }
        public ICollection<GroupDTO> RequestedJoinGroups { get; set; }
        public ICollection<AlbumDTO> Albums { get; set; }
    }
}
