using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Api.Models
{
    public class CreateModel
    {
        public string Result { get; set; }
        public string NewId { get; set; }
        public object CreatedObject { get; set; }
    }
}
