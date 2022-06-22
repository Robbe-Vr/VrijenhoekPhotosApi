using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class AuthorizationHandler
    {
        private IUsersDAL _dal;

        public AuthorizationHandler(IUsersDAL dal)
        {
            _dal = dal;
        }

        public AuthorizationResultDTO Login(AuthorizationDTO auth)
        {
            UserDTO user = _dal.GetAuthByName(auth.UserName);

            if (user == null || HashPassword(auth.Password, user.Salt) != user.PasswordHash)
            {
                if (user != null)
                {
                    _dal.IncrementFailedLogin(user);
                }

                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "Either the username or password is incorrect!",
                };
            }

            if (_dal.IsLockedOut(user))
            {
                int lockoutDuration = (int)(user.LockoutEnd - DateTime.Now).GetValueOrDefault().TotalSeconds;

                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = $"Account is locked out! Try again in {lockoutDuration} seconds.",
                };
            }

            return new AuthorizationResultDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.PasswordHash,
                Success = true,
            };
        }

        private bool ValidateEmail(string email)
        {
            return System.Net.Mail.MailAddress.TryCreate(email, out System.Net.Mail.MailAddress m);
        }
        
        private bool ValidateUserName(string name)
        {
            return new Regex(@"[a-zA-Z0-9 .,_+\-=]{2,64}").IsMatch(name);
        }

        public AuthorizationResultDTO Register(AuthorizationDTO auth, string confirmPw, DateTime creationDate)
        {
            if (auth.Password != confirmPw)
            {
                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "Passwords do not match!",
                };
            }

            if (!ValidateEmail(auth.Email))
            {
                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "Invalid email address!",
                };
            }

            if (!ValidateUserName(auth.UserName))
            {
                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "Invalid username! A username must be between 2 and 64 characters and cannot contain any symbols besides the following '.,_+-='.",
                };
            }

            if (!_dal.ValidateOriginality(auth.UserName))
            {
                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "An user with this username already exists!",
                };
            }
            if (!_dal.ValidateOriginality(auth.Email))
            {
                return new AuthorizationResultDTO(auth)
                {
                    Success = false,
                    Error = "An user with this email address already exists!",
                };
            }

            string salt = NewSalt(8);
            UserDTO user = _dal.Create(new UserDTO()
            {
                Email = auth.Email,
                UserName = auth.UserName,
                PasswordHash = HashPassword(auth.Password, salt),
                Salt = salt,
                CreationDate = creationDate,
                Rights = Rights.CanCreate,
            });

            return new AuthorizationResultDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.PasswordHash,
                Success = true,
            };
        }

        public UserDTO GetNewUser(string userName)
        {
            return _dal.GetByName(userName);
        }

        private string NewSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[size];

            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        private string SaltOnString(string str, string saltString)
        {
            byte[] saltedBytes = Encoding.UTF8.GetBytes(str);

            byte[] salt = Encoding.UTF8.GetBytes(saltString);

            for (int i = 0; i < salt.Length; i++)
            {
                saltedBytes.Append(salt[i]);
            }

            string saltedString = "";
            foreach (byte b in saltedBytes)
            {
                saltedString += b.ToString("X2");
            }

            return saltedString;
        }

        private string HashPassword(string plainPw, string salt)
        {
            HashAlgorithm algorithm = SHA256.Create();

            string saltedString = SaltOnString(plainPw, salt);

            byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltedString));

            string hash = "";
            foreach (byte b in hashBytes)
            {
                hash += b.ToString("X2");
            }

            return hash;
        }
    }
}
