using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class PhotosHandler
    {
        private IPhotosDAL _dal;
        private IFilePhotosDAL _filedal;
        private UserInfo _userInfo;

        public PhotosHandler(IPhotosDAL dal, IFilePhotosDAL filedal, UserInfo userInfo)
        {
            _dal = dal;
            _filedal = filedal;
            _userInfo = userInfo;
        }

        public ResponseResult All(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllByUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult Count(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetByUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult ById(int id)
        {
            PhotoDTO photo = _dal.GetById(id);

            if (photo == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, photo.Id))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(photo);
        }

        public ResponseResult ByName(string name)
        {
            PhotoDTO photo = _dal.GetByName(name);

            if (photo == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, photo.Id))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(photo);
        }

        public ResponseResult GetWebViewableFilePhoto(PhotoDTO photo)
        {
            if (photo == null) return ResponseHandler.Error($"Incomplete Photo object sent.", ErrorCode.Request_Data_Incomplete, photo, context: this);

            if (photo.IsVideo && photo.Type != ".mp4")
            {
                photo.Type = ".mp4";
            }

            return ResponseHandler.Success(GetFilePhotoAsStream(photo)?.Result);
        }

        public ResponseResult AddPhoto(FilePhotoDTO photo)
        {
            if (_userInfo.User.Rights < Rights.CanCreate)
            {
                return ResponseHandler.Error("User is not authorized to create entities.", ErrorCode.User_Not_Authorized, context: this);
            }

            if (!ValidateFileType(photo.Type))
            {
                return ResponseHandler.Error($"The filetype '{photo.Type}' is not supported!", ErrorCode.Request_Data_Incomplete, photo, context: this);
            }
            photo.IsVideo = FileTypeIsVideo(photo.Type);

            if (_filedal.TryStorePhoto(ref photo, _userInfo.User.Id, fileHandling: DoubleFileHandling.Increment))
            {
                if (photo.CreationDate == default)
                    photo.CreationDate = DateTime.UtcNow;
                photo.User = _userInfo.User;
                photo.UserId = _userInfo.User.Id;
                photo.Content = null;
                photo.Thumbnail = null;
                photo = new FilePhotoDTO(_dal.Create(photo));

                if (photo.Id > 0)
                {
                    return ResponseHandler.Success(photo);
                }
            }

            return ResponseHandler.Error("Error occured while storing photo!", ErrorCode.Server_Error, objectInAction: photo, context: this);
        }

        private bool ValidateFileType(string type)
        {
            return FileTypes.AllowedFileTypes.Any(fileType => fileType == type);
        }
        private bool FileTypeIsVideo(string type)
        {
            return FileTypes.AllowedPictureFileTypes.Any(fileType => fileType == type) ? false :
                FileTypes.AllowedVideoFileTypes.Any(fileType => fileType == type);
        }

        public ResponseResult GetFilePhoto(PhotoDTO photo)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, photo.Id) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            FilePhotoDTO filePhoto = new FilePhotoDTO(photo);
            if (!_filedal.TryGetPhoto(ref filePhoto))
            {
                filePhoto.ContentLocation = string.Empty;
            }

            return ResponseHandler.Success(filePhoto);
        }

        public ResponseResult GetFilePhotoAsStream(PhotoDTO photo)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, photo.Id) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            FilePhotoDTO filePhoto = new FilePhotoDTO(photo);
            if (!_filedal.TryGetPhotoAsStream(ref filePhoto))
            {
                filePhoto.ContentLocation = string.Empty;
            }

            return ResponseHandler.Success(filePhoto);
        }

        public ResponseResult GetFilePhotoThumbnail(PhotoDTO photo)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, photo.Id) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            FilePhotoDTO filePhoto = new FilePhotoDTO(photo);
            if (!_filedal.TryGetThumbnail(ref filePhoto) && !_filedal.TryCreateThumbnail(ref filePhoto))
            {
                filePhoto.ThumbnailLocation = string.Empty;
            }

            return ResponseHandler.Success(filePhoto);
        }

        public ResponseResult UpdatePhoto(PhotoDTO photo)
        {
            PhotoDTO old = _dal.GetTracked(photo.Id);

            if (old == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (old.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to update this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            bool newName = !string.IsNullOrEmpty(photo.Name) && old.Name != photo.Name;
            string oldName = "";
            if (newName) oldName = old.Name;
            old.Name =  newName ? photo.Name : old.Name;
            old.Albums = photo.Albums ?? old.Albums;
            return ResponseHandler.Success(_dal.Update() && newName ? _filedal.Update(photo, oldName) : true);
        }

        public ResponseResult RemovePhoto(PhotoDTO photo)
        {
            if (photo.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to remove this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.Remove(photo, _userInfo.User.Id) && _filedal.Remove(photo));
        }
    }
}
