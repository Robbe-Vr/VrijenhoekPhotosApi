using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Api.Authorization
{
    public class TokenModel
    {
        public string VrijenhoekPhotos_RefreshToken { get; set; }
        public string VrijenhoekPhotos_AccessToken { get; set; }
        public string Result { get; set; }
    }
}
