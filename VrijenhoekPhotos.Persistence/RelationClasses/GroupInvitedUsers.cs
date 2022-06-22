using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Persistence.RelationClasses
{
    public class GroupInvitedUsers
    {
        public int GroupId { get; set; }
        public GroupDTO Group { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
    }
}
