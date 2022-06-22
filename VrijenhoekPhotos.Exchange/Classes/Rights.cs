using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public enum Rights
    {
        None = 0,
        CanRead = 5,
        CanCreate = 25,
        CanAdd = 50,
        CanUpdate = 70,
        CanRemove = 80,
        Admin = 100,
    }
}
