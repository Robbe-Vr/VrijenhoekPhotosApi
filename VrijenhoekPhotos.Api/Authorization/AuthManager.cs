using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Authorization
{
    public static class AuthManager
    {
        private static List<SignedInToken> signedInTokens = new List<SignedInToken>();

        public static string SignIn(UserDTO user)
        {
            string accessToken = GenerateAccessToken();
            string refreshToken = GenerateRefreshToken();

            if (!signedInTokens.Any(s => s.UserId == user.Id))
            {
                signedInTokens.Add(new SignedInToken(user, accessToken, refreshToken));
            }
            else
            {
                signedInTokens.First(s => s.UserId == user.Id).Reset(accessToken, refreshToken);
            }

            return refreshToken;
        }

        public static string GetToken(string userId)
        {
            SignedInToken token = signedInTokens.FirstOrDefault(s => s.UserId == userId);

            if (token == null)
            {
                return "Not found.";
            }
            else if (token.Expired)
            {
                return "Expired.";
            }
            else
            {
                return token.AccessToken;
            }
        }

        public static UserDTO GetUser(string accessToken)
        {
            SignedInToken token = signedInTokens.FirstOrDefault(s => s.AccessToken == accessToken);

            return token?.User;
        }

        public static string RefreshToken(string refreshToken)
        {
            if (signedInTokens.Any(s => s.RefreshToken == refreshToken))
            {
                string newAccessToken = GenerateAccessToken();

                signedInTokens.First(s => s.RefreshToken == refreshToken).Refresh(newAccessToken);

                return newAccessToken;
            }
            else return "Not found.";
        }

        public static bool SignOut(string userId)
        {
            int removedCount = signedInTokens.RemoveAll(s => s.UserId == userId || s.Expired);

            return removedCount > -1;
        }

        private static string GenerateAccessToken()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!+";

            Random r = new Random();

            return new string(Enumerable.Repeat(chars, 19).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 9).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 15).Select(c => c[r.Next(c.Length)]).ToArray());
        }

        private static string GenerateRefreshToken()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!+";

            Random r = new Random();

            return new string(Enumerable.Repeat(chars, 19).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 9).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 12).Select(c => c[r.Next(c.Length)]).ToArray()) + "-" +
                   new string(Enumerable.Repeat(chars, 15).Select(c => c[r.Next(c.Length)]).ToArray());
        }
    }

    public class SignedInToken
    {
        public SignedInToken(UserDTO user, string accessToken, string refreshToken)
        {
            User = user;
            UserId = user.Id;

            RefreshToken = refreshToken;
            AccessToken = accessToken;

            ExpiratedDate = DateTime.Now.AddHours(2);
        }

        public void Reset(string newAccessToken, string newRefreshToken)
        {
            RefreshToken = newRefreshToken;
            Refresh(newAccessToken);
        }

        public void Refresh(string newAccessToken)
        {
            AccessToken = newAccessToken;

            ExpiratedDate = DateTime.Now.AddHours(2);
        }

        public string UserId { get; set; }
        public UserDTO User { get; set; }

        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiratedDate { get; private set; }

        public bool Expired { get { return ExpiratedDate < DateTime.Now; } }
        public bool Valid { get { return !String.IsNullOrEmpty(UserId) && !String.IsNullOrEmpty(AccessToken) && !Expired; } }
    }
}
