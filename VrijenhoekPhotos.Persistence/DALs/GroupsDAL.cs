using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;
using VrijenhoekPhotos.Persistence.RelationClasses;

namespace VrijenhoekPhotos.Persistence.DALs
{
    public class GroupsDAL : IntIdentifiedBaseDAL<GroupDTO>, IGroupsDAL
    {
        public GroupsDAL(VrijenhoekPhotosDbContext context) : base(context) { }

        public GroupDTO Create(GroupDTO group)
        {
            if (table.Any(x => x.GroupName == group.GroupName))
            {
                return table.AsNoTracking().FirstOrDefault(x => x.GroupName == group.GroupName && x.CreatorId == group.CreatorId);
            }

            group.Creator = null;
            table.Add(group);
            
            return dbContext.SaveChanges() > 0 ?
                table.AsNoTracking().FirstOrDefault(x => x.GroupName == group.GroupName && x.CreatorId == group.CreatorId) : null;
        }

        public IEnumerable<AlbumDTO> GetAlbumsFromGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IEnumerable<AlbumDTO> results = table.AsNoTracking()
                .Include(x => x.Albums)
                    .ThenInclude(x => x.Tags)
                .FirstOrDefault(x => x.Id == groupId)?.Albums;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results.Where(x => x.Name.Contains(nameFilter));
            }

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.Name),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.Name),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetAlbumsFromGroupCount(int groupId, string nameFilter = "")
        {
            IEnumerable<AlbumDTO> results = table
                .Include(x => x.Albums)
                    .ThenInclude(x => x.Tags)
                .FirstOrDefault(x => x.Id == groupId)?.Albums;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.Name.Contains(nameFilter));
            }

            return results?.Count() ?? 0;
        }

        public IEnumerable<UserDTO> GetUsersFromGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IEnumerable<UserDTO> results = table.AsNoTracking()
                .Include(x => x.Users)
                .FirstOrDefault(x => x.Id == groupId)?.Users;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results.Where(x => x.UserName.Contains(nameFilter));
            }

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.UserName),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.UserName),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetUsersFromGroupCount(int groupId, string nameFilter = "")
        {
            IEnumerable<UserDTO> results = table
                .Include(x => x.Users)
                .FirstOrDefault(x => x.Id == groupId)?.Users;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.UserName.Contains(nameFilter));
            }

            return results?.Count() ?? 0;
        }

        public IEnumerable<GroupDTO> SeekAll(string filter, int page = 1, int rpp = 10, Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<GroupDTO> results = table.AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.IconPhoto)
                .Include(x => x.Creator)
                .Where(x => filter == null || x.GroupName.Contains(filter));

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.GroupName),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.GroupName),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetSeekAllCount(string filter)
        {
            return table
                .Count(x => filter == null || x.GroupName.Contains(filter));
        }

        public IEnumerable<GroupDTO> GetAllByUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<GroupDTO> results = table.AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.PendingJoinUsers)
                .Include(x => x.RequestedJoinUsers)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Creator)
                .Where(x => x.CreatorId == userId);

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results.Where(x => x.GroupName.Contains(nameFilter));
            }

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.GroupName),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.GroupName),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetByUserCount(string userId, string nameFilter = "")
        {
            IQueryable<GroupDTO> results = table;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.GroupName.Contains(nameFilter));
            }

            return results?.Count(x => x.CreatorId == userId) ?? 0;
        }

        public IEnumerable<GroupDTO> GetAllJoinedByUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<GroupDTO> results = table.AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Creator)
                .Where(x => x.Users.Any(x => x.Id == userId));

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.GroupName.Contains(nameFilter));
            }

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.GroupName),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.GroupName),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetJoinedByUserCount(string userId, string nameFilter = "")
        {
            IQueryable<GroupDTO> results = table;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.GroupName.Contains(nameFilter));
            }

            return results.Count(x => x.Users.Any(x => x.Id == userId));
        }

        public IEnumerable<GroupDTO> GetAllAvailableToUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<GroupDTO> results = table.AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Creator)
                .Where(x => x.Users.Any(x => x.Id == userId) || x.CreatorId == userId);

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.GroupName.Contains(nameFilter));
            }

            results = sorting switch
            {
                Sorting.Alphabet => results.OrderBy(x => x.GroupName),
                Sorting.Inverted_Alphabet => results.OrderByDescending(x => x.GroupName),
                Sorting.Newest_Oldest => results.OrderByDescending(x => x.CreationDate),
                Sorting.Oldest_Newest => results.OrderBy(x => x.CreationDate),
                _ => results,
            };

            return results
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetAvailableToUserCount(string userId, string nameFilter = "")
        {
            IQueryable<GroupDTO> results = table;

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results?.Where(x => x.GroupName.Contains(nameFilter));
            }

            return results.Count(x => x.Users.Any(x => x.Id == userId) || x.CreatorId == userId);
        }

        public GroupDTO GetTracked(int id)
        {
            return table
                .Include(x => x.Albums)
                .Include(x => x.Creator)
                .Include(x => x.Users)
                .Include(x => x.PendingJoinUsers)
                .Include(x => x.RequestedJoinUsers)
                .Include(x => x.IconPhoto)
                .FirstOrDefault(x => x.Id == id);
        }

        public GroupDTO GetById(int id)
        {
            return table.AsNoTracking()
                .Include(x => x.Creator)
                .Include(x => x.Users)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == id)
                .ExcludeAuthorization();
        }

        public GroupDTO GetOwnedById(int id)
        {
            return table.AsNoTracking()
                .Include(x => x.Creator)
                .Include(x => x.Users)
                .Include(x => x.PendingJoinUsers)
                .Include(x => x.RequestedJoinUsers)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == id)
                .ExcludeAuthorization();
        }

        public bool CheckAccess(string userId, Rights userRights, int id)
        {
            if (userRights == Rights.Admin) return true;

            GroupDTO group = table
                .Include(x => x.Users)
                .FirstOrDefault(x => x.Id == id);

            return userRights >= Rights.CanRead && (
                    group.CreatorId == userId || group.Users.Any(x => x.Id == userId)
                );
        }

        public GroupDTO GetByName(string name)
        {
            return table.AsNoTracking()
                .Include(x => x.Creator)
                .Include(x => x.Users)
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.GroupName == name)
                .ExcludeAuthorization();
        }

        public bool Update(GroupDTO group)
        {
            if (!ValidateOriginality(group.GroupName, group.CreatorId))
            {
                return false;
            }
            group.IconPhoto = null;
            group.Creator = null;

            if (!Exists(group))
            {
                return false;
            }
            if (!EntityIsAttached(group))
            {
                if (KeyIsAttached(group))
                {
                    GroupDTO local = GetAttachedEntityByEntity(group);

                    local.GroupName = string.IsNullOrWhiteSpace(group.GroupName) ? local.GroupName : group.GroupName;
                    local.IconPhotoId = group.IconPhotoId ?? local.IconPhotoId;
                    local.Albums = group.Albums ?? local.Albums;
                    local.Users = group.Users ?? local.Users;
                    local.PendingJoinUsers = group.PendingJoinUsers ?? local.PendingJoinUsers;
                    local.RequestedJoinUsers = group.RequestedJoinUsers ?? local.RequestedJoinUsers;

                    dbContext.Entry(local).State = EntityState.Modified;
                }
                else dbContext.Entry(group).State = EntityState.Modified;
            }

            UpdateAlbums(group);
            UpdateUsers(group);
            UpdatePendingJoinUsers(group);
            UpdateRequestedJoinUsers(group);

            return dbContext.SaveChanges() > 0;
        }

        private void UpdateAlbums(GroupDTO group)
        {
            List<GroupAlbums> currentList = dbContext.GroupAlbums.Where(x => x.GroupId == group.Id).ToList();

            GroupAlbums[] removedArr = new GroupAlbums[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<GroupAlbums> removed = removedArr.ToList();
            foreach (AlbumDTO album in group.Albums)
            {
                if (!currentList.Any(x => x.AlbumId == album.Id))
                {
                    GroupAlbums newRow = new GroupAlbums()
                    {
                        GroupId = group.Id,
                        AlbumId = album.Id,
                    };

                    dbContext.GroupAlbums.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.AlbumId == album.Id);
                }
            }

            foreach (GroupAlbums item in removed)
            {
                dbContext.GroupAlbums.Remove(item);
            }
        }

        private void UpdateUsers(GroupDTO group)
        {
            List<GroupUsers> currentList = dbContext.GroupUsers.Where(x => x.GroupId == group.Id).ToList();

            GroupUsers[] removedArr = new GroupUsers[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<GroupUsers> removed = removedArr.ToList();
            foreach (UserDTO user in group.Users)
            {
                if (!currentList.Any(x => x.UserId == user.Id))
                {
                    GroupUsers newRow = new GroupUsers()
                    {
                        GroupId = group.Id,
                        UserId = user.Id,
                    };

                    dbContext.GroupUsers.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.UserId == user.Id);
                }
            }

            foreach (GroupUsers item in removed)
            {
                dbContext.GroupUsers.Remove(item);
            }
        }

        private void UpdatePendingJoinUsers(GroupDTO group)
        {
            List<GroupInvitedUsers> currentList = dbContext.GroupInvitedUsers.Where(x => x.GroupId == group.Id).ToList();

            GroupInvitedUsers[] removedArr = new GroupInvitedUsers[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<GroupInvitedUsers> removed = removedArr.ToList();
            foreach (UserDTO user in group.PendingJoinUsers)
            {
                if (!currentList.Any(x => x.UserId == user.Id))
                {
                    GroupInvitedUsers newRow = new GroupInvitedUsers()
                    {
                        GroupId = group.Id,
                        UserId = user.Id,
                    };

                    dbContext.GroupInvitedUsers.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.UserId == user.Id);
                }
            }

            foreach (GroupInvitedUsers item in removed)
            {
                dbContext.GroupInvitedUsers.Remove(item);
            }
        }

        private void UpdateRequestedJoinUsers(GroupDTO group)
        {
            List<GroupRequestingUsers> currentList = dbContext.GroupRequestingUsers.Where(x => x.GroupId == group.Id).ToList();

            GroupRequestingUsers[] removedArr = new GroupRequestingUsers[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<GroupRequestingUsers> removed = removedArr.ToList();
            foreach (UserDTO user in group.RequestedJoinUsers)
            {
                if (!currentList.Any(x => x.UserId == user.Id))
                {
                    GroupRequestingUsers newRow = new GroupRequestingUsers()
                    {
                        GroupId = group.Id,
                        UserId = user.Id,
                    };

                    dbContext.GroupRequestingUsers.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.UserId == user.Id);
                }
            }

            foreach (GroupRequestingUsers item in removed)
            {
                dbContext.GroupRequestingUsers.Remove(item);
            }
        }

        public bool Update()
        {
            return dbContext.SaveChanges() > 0;
        }

        public bool Remove(GroupDTO group, string userId)
        {
            GroupDTO removable = table.Include(x => x.Creator).FirstOrDefault(x => x.Id == group.Id && (x.CreatorId == userId || x.Creator.Rights >= Rights.CanRemove));
            table.Remove(removable);
            Logger.Log($"user {userId} removed group with id {removable.Id} owned by user {removable.CreatorId}.");
            return dbContext.SaveChanges() > 0;
        }

        public bool ValidateOriginality(string name, string userId)
        {
            return !String.IsNullOrEmpty(name) &&
                !String.IsNullOrEmpty(userId) &&
                !table.Any(x => x.GroupName == name && x.CreatorId != userId);
        }
    }
}
