using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class UserInfo
    {
        public UserInfo(UserDTO user)
        {
            User = user;
        }

        public UserDTO User { get; set; }
    }
}
