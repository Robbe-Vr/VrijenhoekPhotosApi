using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Authorization
{
    internal static class MailSender
    {
        private class NotificationServerRequestContent
        {
            public string Subject { get; set; }
            public string Content { get; set; }
            public string Sender { get; set; }
            public string Reciever { get; set; }
        }

        private const string parameterSeparator = "|:=:|";
        private const string keyValueSeparator = "::";

        internal static bool SendResetEmail(UserDTO user, string token)
        {
            NotificationServerRequestContent content = new NotificationServerRequestContent()
            {
                Subject = "Reset Password for " + user.UserName,
                Content =
                    "parameter_separation" + parameterSeparator +
                    "applicationName" + keyValueSeparator + "VrijenhoekPhotos" + parameterSeparator +
                    "applicationUrlName" + keyValueSeparator + "photos" + parameterSeparator +
                    "userName" + keyValueSeparator + user.UserName + parameterSeparator +
                    "resetPasswordUrl" + keyValueSeparator + $"https://photos.sywapps.com/account/resetpassword?reset_token={token}"
                    ,
                Sender = "info@sywapps.com",
                Reciever = user.Email,
            };

            return SendEmail(content, new Uri("http://localhost:12420/api/notify/resetpassword"));
        }

        internal static bool SendWelcomeEmail(UserDTO user)
        {
            NotificationServerRequestContent content = new NotificationServerRequestContent()
            {
                Subject = "Welcome to VrijenhoekPhotos!",
                Content =
                    "parameter_separation" + parameterSeparator +
                    "applicationName" + keyValueSeparator + "VrijenhoekPhotos" + parameterSeparator +
                    "applicationUrlName" + keyValueSeparator + "photos" + parameterSeparator +
                    "userName" + keyValueSeparator + user.UserName
                    ,
                Sender = "info@sywapps.com",
                Reciever = user.Email,
            };

            return SendEmail(content, new Uri("http://localhost:12420/api/notify/welcome"));
        }

        private static bool SendEmail(NotificationServerRequestContent content, Uri uri)
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage res = client.Send(new HttpRequestMessage()
            {
                RequestUri = uri,
                Content = JsonContent.Create(content),
                Method = HttpMethod.Post,
            });

            return res.IsSuccessStatusCode;
        }
    }
}
