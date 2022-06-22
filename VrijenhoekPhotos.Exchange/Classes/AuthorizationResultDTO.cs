﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.Classes
{
    public class AuthorizationResultDTO : AuthorizationDTO
    {
        public AuthorizationResultDTO() { }
        public AuthorizationResultDTO(AuthorizationDTO auth)
        {
            base.Id = auth.Id;
            base.UserName = auth.UserName;
            base.Password = auth.Password;
        }

        public Rights Rights { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
