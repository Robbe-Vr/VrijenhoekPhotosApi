using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Models
{
    public class FilePhotoModel : PhotoDTO
    {
        public IFormFile ImageFile { get; set; }
    }
}
