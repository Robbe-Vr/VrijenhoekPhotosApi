using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class GroupsHandler
    {
        private IGroupsDAL _dal;
        private IUsersDAL _usersdal;
        private IAlbumsDAL _albumsdal;
        private UserInfo _userInfo;

        public GroupsHandler(IGroupsDAL dal, IUsersDAL usersdal, IAlbumsDAL albumsdal, UserInfo userInfo)
        {
            _dal = dal;
            _usersdal = usersdal;
            _albumsdal = albumsdal;
            _userInfo = userInfo;
        }

        public ResponseResult Create(string name)
        {
            if (_userInfo.User.Rights < Rights.CanCreate)
            {
                return ResponseHandler.Error("User is not authorized to create entities.", ErrorCode.User_Not_Authorized, context: this);
            }

            GroupDTO group = new GroupDTO()
            {
                CreationDate = DateTime.UtcNow,
                GroupName = name,
                Creator = _userInfo.User,
                CreatorId = _userInfo.User.Id,
            };
            group = _dal.Create(group);

            return ResponseHandler.Success(group);
        }

        public ResponseResult All(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllAvailableToUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult Count(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetAvailableToUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult Seek(string filter, int page = 1, int rpp = 10, Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.SeekAll(filter, page, rpp, sorting));
        }

        public ResponseResult GetSeekCount(string filter)
        {
            return ResponseHandler.Success(_dal.GetSeekAllCount(filter));
        }

        public ResponseResult AllJoined(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllJoinedByUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult JoinedCount(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetJoinedByUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult AllOwned(int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            return ResponseHandler.Success(_dal.GetAllByUser(_userInfo.User.Id, page, rpp, nameFilter, sorting));
        }

        public ResponseResult OwnedCount(string nameFilter = "")
        {
            return ResponseHandler.Success(_dal.GetByUserCount(_userInfo.User.Id, nameFilter));
        }

        public ResponseResult AlbumsInGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, groupId) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetAlbumsFromGroup(groupId, page, rpp, nameFilter, sorting));
        }

        public ResponseResult AlbumsInGroupCount(int groupId, string nameFilter = "")
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, groupId) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetAlbumsFromGroupCount(groupId, nameFilter));
        }

        public ResponseResult UsersInGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, groupId) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetUsersFromGroup(groupId, page, rpp, nameFilter, sorting));
        }

        public ResponseResult UsersInGroupCount(int groupId, string nameFilter = "")
        {
            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, groupId) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.GetUsersFromGroupCount(groupId, nameFilter));
        }

        public ResponseResult ById(int id)
        {
            GroupDTO group = _dal.GetById(id);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, group.Id) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(group);
        }

        public ResponseResult ByName(string name)
        {
            GroupDTO group = _dal.GetByName(name);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (!_dal.CheckAccess(_userInfo.User.Id, _userInfo.User.Rights, group.Id) && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(group);
        }

        public ResponseResult RequestJoin(int groupId)
        {
            GroupDTO group = _dal.GetTracked(groupId);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.Users.Any(x => x.Id == _userInfo.User.Id) || group.CreatorId == _userInfo.User.Id)
            {
                return ResponseHandler.Success(new { Result = false, Message = "User is already member of this group!" });
            }
            else if (group.Users.Any(x => x.Id == _userInfo.User.Id))
            {
                return ResponseHandler.Success(new { Result = false, Message = "User has already send a join request for this group!" });
            }
            else if (group.PendingJoinUsers.Any(x => x.Id == _userInfo.User.Id))
            {
                AddUser(group, _userInfo.User.Id);
                return ResponseHandler.Success(new { Result = true, Special = "User has an open join request for this group which we accepted instead!" });
            }

            UserDTO user = _usersdal.GetTracked(_userInfo.User.Id);
            group.RequestedJoinUsers.Add(user);

            return _dal.Update() ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to request for join!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult AcceptRequest(int groupId, string userId)
        {
            GroupDTO group = _dal.GetTracked(groupId);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id)
            {
                return ResponseHandler.Error("User is not authorized to manage this group!", ErrorCode.User_Not_Authorized, context: this);
            }
            if (group.RequestedJoinUsers.All(x => x.Id != userId))
            {
                return ResponseHandler.Error("User is not authorized to manage this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            return AddUser(group, userId);
        }

        public ResponseResult DeclineRequest(int groupId, string userId)
        {
            GroupDTO group = _dal.GetTracked(groupId);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id)
            {
                return ResponseHandler.Error("the given user has not send a request to join this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            int originalCount = group.RequestedJoinUsers.Count;

            UserDTO user = _usersdal.GetTracked(userId);
            group.Users.Remove(user);

            if (originalCount != group.RequestedJoinUsers.Count)
            {
                _dal.Update(group);

                return ResponseHandler.Success(group);
            }
            else
            {
                return ResponseHandler.Success(new { Result = false, Message = "The given user does not have an open request to decline!" });
            }
        }

        public ResponseResult Invite(int groupId, string userId)
        {
            UserDTO user = _usersdal.GetTracked(userId);
            GroupDTO group = _dal.GetTracked(groupId);

            if (group == null && user == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id)
            {
                return ResponseHandler.Error("User is not authorized to manage this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            if (group.Users.Any(x => x.Id == user.Id) || group.CreatorId == userId)
            {
                return ResponseHandler.Success(new { Result = false, Message = "User is already member of this group!" });
            }
            else if (group.PendingJoinUsers.Any(x => x.Id == _userInfo.User.Id))
            {
                return ResponseHandler.Success(new { Result = false, Message = "User has already been invited to join this group!" });
            }
            else if (group.RequestedJoinUsers.Any(x => x.Id == userId))
            {
                AddUser(group, userId);
                return ResponseHandler.Success(new { Result = true, Special = "User has an open join request for this group which we accepted instead!" });
            }

            group.Users.Add(user);

            return _dal.Update() ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to invite user!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult AcceptInvite(int groupId)
        {
            GroupDTO group = _dal.GetTracked(groupId);

            if (group == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.PendingJoinUsers.All(x => x.Id != _userInfo.User.Id))
            {
                return ResponseHandler.Error("User has not received an invite to join this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            return AddUser(group, _userInfo.User.Id);
        }

        public ResponseResult DeclineInvite(int groupId)
        {
            GroupDTO group = _dal.GetTracked(groupId);

            int originalCount = group.PendingJoinUsers.Count;

            UserDTO user = _usersdal.GetTracked(_userInfo.User.Id);
            group.Users.Remove(user);

            if (originalCount != group.PendingJoinUsers.Count)
            {
                _dal.Update();

                return ResponseHandler.Success(group);
            }
            else
            {
                return ResponseHandler.Success(new { Result = false, Message = "User does not have an open invite to decline!" });
            }
        }

        public ResponseResult AddAlbum(int groupId, int albumId)
        {
            GroupDTO group = _dal.GetTracked(groupId);
            AlbumDTO album = _albumsdal.GetTracked(albumId);

            if (group == null && album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id && album.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to manage this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            group.Albums.Add(album);

            return _dal.Update() ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to add album to group!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult RemoveAlbum(int groupId, int albumId)
        {
            GroupDTO group = _dal.GetTracked(groupId);
            AlbumDTO album = _albumsdal.GetTracked(albumId);

            if (group == null && album == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id && album.UserId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to manage this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            group.Albums.Remove(album);

            return _dal.Update() ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to invite user!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult Update(GroupDTO group)
        {
            if (group.CreatorId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to read this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.Update(group));
        }

        private ResponseResult AddUser(GroupDTO group, string userId)
        {
            UserDTO user = _usersdal.GetTracked(userId);

            if (user == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id && userId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to update this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            group.Users.Add(user);

            group.Users.Remove(user);
            group.Users.Remove(user);

            return _dal.Update(group) ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to add user to group!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult RemoveUser(int groupId, string userId)
        {
            GroupDTO group = _dal.GetTracked(groupId);
            UserDTO user = _usersdal.GetTracked(userId);

            if (group == null && user == null)
            {
                return ResponseHandler.Error("Invalid data supplied!", ErrorCode.Request_Data_Incomplete, context: this);
            }

            if (group.CreatorId != _userInfo.User.Id && userId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to update this group!", ErrorCode.User_Not_Authorized, context: this);
            }

            group.Users.Remove(user);

            return _dal.Update() ? ResponseHandler.Success(group) : ResponseHandler.Error("Failed to remvoe user from group!", ErrorCode.Server_Error, new { group, _userInfo.User }, this);
        }

        public ResponseResult Remove(GroupDTO group)
        {
            if (group.CreatorId != _userInfo.User.Id && _userInfo.User.Rights != Rights.Admin)
            {
                return ResponseHandler.Error("User is not authorized to remove this entity.", ErrorCode.User_Not_Authorized, context: this);
            }

            return ResponseHandler.Success(_dal.Remove(group, _userInfo.User.Id));
        }
    }
}
