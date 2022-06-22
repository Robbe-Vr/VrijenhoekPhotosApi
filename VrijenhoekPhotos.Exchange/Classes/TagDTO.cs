using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class TagDTO : IIdentifier<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<AlbumDTO> Albums { get; set; }
    }
}
