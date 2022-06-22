using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IFilePhotosDAL
    {
        public bool TryStorePhoto(ref FilePhotoDTO photo, string userFolder, string name = null, DoubleFileHandling fileHandling = DoubleFileHandling.Error);
        public bool TryGetPhoto(ref FilePhotoDTO photo);
        public bool TryGetPhotoAsStream(ref FilePhotoDTO photo);
        public List<FilePhotoDTO> GetPhotos(UserDTO user);
        public bool TryCreateThumbnail(ref FilePhotoDTO photo, int height = 256, int width = 256);
        public bool TryGetThumbnail(ref FilePhotoDTO filePhoto);
        public bool Update(PhotoDTO photo, string oldName);
        public bool Remove(PhotoDTO photo);
    }
}
