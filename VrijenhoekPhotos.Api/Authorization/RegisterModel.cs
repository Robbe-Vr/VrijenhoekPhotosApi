using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Api.Authorization
{
    public class RegisterModel : LoginModel
    {
        public string Email { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
