using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Authorization
{
    public static class ResetManager
    {
        private static List<ResetToken> resetTokens = new List<ResetToken>();

        public static bool ResetPassword(UserDTO user)
        {
            ResetToken token;
            if ((token = resetTokens.FirstOrDefault(x => x.UserId == user?.Id)) != null)
            {
                token.Reset(GenerateResetToken());
            }
            else
            {
                token = new ResetToken(user, GenerateResetToken());
                resetTokens.Add(token);
            }

            if (MailSender.SendResetEmail(user, token.Token))
            {
                return true;
            }
            else
            {
                resetTokens.RemoveAll(x => x.UserId == user.Id);
                return false;
            }
        }

        public static UserDTO ValidateReset(string token)
        {
            UserDTO user = resetTokens.FirstOrDefault(x => !x.Expired && x.Token == token)?.User;
            resetTokens.RemoveAll(x => x.Expired);
            return user;
        }

        private static string GenerateResetToken()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!+";

            Random r = new();

            return new string(Enumerable.Repeat(chars, 19).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 9).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 12).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 17).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 14).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 6).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 15).Select(c => c[r.Next(c.Length)]).ToArray());
        }
    }

    public class ResetToken
    {
        public ResetToken(UserDTO user, string resetToken)
        {
            User = user;
            UserId = user.Id;

            Token = resetToken;

            ExpiratedDate = DateTime.Now.AddMinutes(5);
        }

        public void Reset(string newResetToken)
        {
            Token = newResetToken;

            ExpiratedDate = DateTime.Now.AddMinutes(5);
        }

        public string UserId { get; set; }
        public UserDTO User { get; set; }

        public string Token { get; set; }
        public DateTimeOffset ExpiratedDate { get; private set; }

        public bool Expired { get { return ExpiratedDate < DateTime.Now; } }
        public bool Valid { get { return !String.IsNullOrEmpty(UserId) && !String.IsNullOrEmpty(Token) && !Expired; } }
    }
}
