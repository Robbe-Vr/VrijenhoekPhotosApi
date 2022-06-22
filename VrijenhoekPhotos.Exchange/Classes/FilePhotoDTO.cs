using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class FilePhotoDTO : PhotoDTO
    {
        public FilePhotoDTO() { }
        public FilePhotoDTO(PhotoDTO photo)
        {
            if (photo == null) return;

            base.Id = photo.Id;
            base.Name = photo.Name;
            base.CreationDate = photo.CreationDate;
            base.IsVideo = photo.IsVideo;
            base.Type = photo.Type;
            base.User = photo.User;
            base.UserId = photo.UserId;
            base.Albums = photo.Albums;
        }

        public FileStream Stream { get; set; }
        public string ContentLocation { get; set; }
        public byte[] Content { get; set; }
        public string ThumbnailLocation { get; set; }
        public byte[] Thumbnail { get; set; }
    }
}
