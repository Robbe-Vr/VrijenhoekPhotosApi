using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class UsersHandler
    {
        private IUsersDAL _dal;
        private UserInfo _info;

        public UsersHandler(IUsersDAL dal, UserInfo info)
        {
            _dal = dal;
            _info = info;
        }

        public ResponseResult All(int page = 1, int rpp = 10)
        {
            return ResponseHandler.Success(_dal.GetAll(page, rpp));
        }

        public ResponseResult Count()
        {
            return ResponseHandler.Success(_dal.GetCount());
        }

        public ResponseResult GetGroupInvites()
        {
            return ResponseHandler.Success(_dal.GetById(_info.User.Id)?.PendingJoinGroups);
        }

        public ResponseResult GetJoinRequests()
        {
            return ResponseHandler.Success(_dal.GetById(_info.User.Id)?.RequestedJoinGroups);
        }

        public ResponseResult UpdateName(string newName, string userId = null)
        {
            if (userId != _info.User.Id && _info.User.Rights < Rights.Admin)
            {
                ResponseHandler.Error("Current user is not authorized to perform this action.", ErrorCode.User_Not_Authorized, context: this);
            }

            UserDTO user = _dal.GetTracked(userId ?? _info.User.Id);

            user.UserName = newName;

            return ResponseHandler.Success(_dal.Update());
        }
        
        public ResponseResult UpdatePassword(string newPassword, string userId = null)
        {
            if (userId != _info.User.Id && _info.User.Rights < Rights.Admin)
            {
                ResponseHandler.Error("Current user is not authorized to perform this action.", ErrorCode.User_Not_Authorized, context: this);
            }

            UserDTO user = _dal.GetTracked(userId ?? _info.User.Id);

            user.PasswordHash = HashPassword(newPassword, user.Salt);

            return ResponseHandler.Success(_dal.Update());
        }

        public UserDTO GetByNameOrEmail(string nameOrEmail)
        {
            return _dal.GetByName(nameOrEmail);
        }

        public ResponseResult Remove(UserDTO user = null)
        {
            if (user?.Id != _info.User.Id && _info.User.Rights < Rights.Admin)
            {
                ResponseHandler.Error("Current user is not authorized to perform this action.", ErrorCode.User_Not_Authorized, user, context: this);
            }

            return ResponseHandler.Success(_dal.Remove(user ?? _info.User, _info.User.Id));
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
