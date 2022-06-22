using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class AlbumsHandler
    {
        private IAlbumsDAL _dal;
        private ITagsDAL _tagsDal;
        private IPhotosDAL _photosDal;
        private UserInfo _userInfo;

        public AlbumsHandler(IAlbumsDAL dal, ITagsDAL tagsDal, IPhotosDAL photosDal, UserInfo userInfo)
        {
            _dal = dal;
            _tagsDal = tagsDal;
            _photosDal = photosDal;
            _userInfo = userInfo;
        }

        public ResponseResult AllOwned(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllByUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult OwnedCount(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetByUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult AllJoined(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllJoinedByUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult JoinedCount(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetJoinedByUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult All(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllAvailableToUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult Count(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetAvailableToUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult PhotosFromAlbum(int albumId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, albumId))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetPhotosFromAlbum(albumId, page, rpp, nameFilter, sorting));
        }

        public ResponseResult PhotosFromAlbumCount(int albumId, string nameFilter = "")
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, albumId))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetPhotosFromAlbumCount(albumId, nameFilter));
        }

        public ResponseResult ById(int id)
        {
            AlbumDTO album = _dal.GetById(id);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, album.Id))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(album);
        }

        public ResponseResult ByName(string name)
        {
            AlbumDTO album = _dal.GetByName(name, _userInfo.User.Id);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, album.Id))
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(album);
        }

        public ResponseResult ForceAddTag(int albumId, TagDTO tag)
        {
            TagDTO trackedTag = _tagsDal.GetTracked(tag.Id);

            if (trackedTag == null)
            {
                tag.UserId = _userInfo.User.Id;
                tag.CreationDate = DateTime.Now;
                trackedTag = _tagsDal.Create(tag);
            }

            return AddTag(albumId, trackedTag);
        }

        private ResponseResult AddTag(int albumId, TagDTO tag)
        {
            AlbumDTO album = _dal.GetTracked(albumId);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (album.UserId != _userInfo.User.Id && _userInfo.User.Rights == Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to add tags to this album!", ErrorCode.User_Not_Authorized, context: this);
            }

            if (!album.Tags.Any(x => x.Id == tag.Id))
            {
                album.Tags.Add(tag);
            }
            else
            {
                return ResponseHandler.Error("Tag is already present in this Album!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            return ResponseHandler.Success(_dal.Update());
        }

        public ResponseResult RemoveTag(int albumId, int tagId)
        {
            AlbumDTO album = _dal.GetTracked(albumId);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (album.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to remove tags from this album!", ErrorCode.User_Not_Authenticated, context: this);
            }

            TagDTO tag = _tagsDal.GetTracked(tagId);
            album.Tags.Remove(tag);

            return ResponseHandler.Success(_dal.Update());
        }

        public ResponseResult Create(string name)
        {
            if (_userInfo.User.Rights < Rights.CanCreate)
            {
                return ResponseHandler.Error("User is not authorized to create entities.", ErrorCode.User_Not_Authorized, context: this);
            }

            AlbumDTO album = new AlbumDTO()
            {
                CreationDate = DateTime.UtcNow,
                Name = name,
                User = _userInfo.User,
                UserId = _userInfo.User.Id,
            };
            album = _dal.Create(album);
            return ResponseHandler.Success(album);
        }

        public ResponseResult AddPhoto(int albumId, int photoId)
        {
            AlbumDTO album = _dal.GetTracked(albumId);
            PhotoDTO photo = _photosDal.GetTracked(photoId);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (album.UserId != _userInfo.User.Id && photo.UserId != _userInfo.User.Id && _userInfo.User.Rights == Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to add photos to this album!", ErrorCode.User_Not_Authenticated, context: this);
            }

            if (!album.Photos.Any(x => x.Id == photo.Id))
            {
                album.Photos.Add(photo);
            }
            else
            {
                return ResponseHandler.Error("Photo is already present in this Album!", ErrorCode.Request_Data_Incomplete);
            }

            return ResponseHandler.Success(_dal.Update());
        }

        public ResponseResult RemovePhoto(int albumId, int photoId)
        {
            AlbumDTO album = _dal.GetTracked(albumId);
            PhotoDTO photo = _photosDal.GetTracked(photoId);

            if (album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (album.UserId != _userInfo.User.Id && photo.UserId != _userInfo.User.Id && _userInfo.User.Rights == Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to remove photos from this album!", ErrorCode.User_Not_Authenticated, context: this);
            }

            album.Photos.Remove(photo);

            return ResponseHandler.Success(_dal.Update());
        }

        public ResponseResult Update(AlbumDTO album)
        {
            if (album.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to update this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.Update(album));
        }

        public ResponseResult Remove(AlbumDTO album)
        {
            if (album.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to remove this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.Remove(album, _userInfo.User.Id));
        }
    }
}
