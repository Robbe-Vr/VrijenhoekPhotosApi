using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class PhotoDTO : IIdentifier<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string Type { get; set; }
        public bool IsVideo { get; set; }
        public ICollection<AlbumDTO> Albums { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
    }
}
