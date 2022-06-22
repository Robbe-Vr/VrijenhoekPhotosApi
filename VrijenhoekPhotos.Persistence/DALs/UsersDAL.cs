using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Persistence.DALs
{
    public class UsersDAL : StringIdentifiedBaseDAL<UserDTO>, IUsersDAL
    {
        public UsersDAL(VrijenhoekPhotosDbContext context) : base(context) { }

        public UserDTO Create(UserDTO user)
        {
            if (table.Any(x => x.UserName == user.UserName))
            {
                return null;
            }

            if (String.IsNullOrEmpty(user.Id))
                user.Id = Guid.NewGuid().ToString();

            table.Add(user);
            
            return dbContext.SaveChanges() > 0 ?
                table.AsNoTracking().FirstOrDefault(x => x.UserName == user.UserName).ExcludeAuthorization() : null;
        }

        public UserDTO GetTracked(string id)
        {
            return table
                .Include(x => x.Groups)
                .Include(x => x.OwnedGroups)
                .Include(x => x.PendingJoinGroups)
                .Include(x => x.RequestedJoinGroups)
                .Include(x => x.Albums)
                .FirstOrDefault(x => x.Id == id);
        }

        public UserDTO GetById(string id)
        {
            return table.AsNoTracking()
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.OwnedGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.PendingJoinGroups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.PendingJoinGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.RequestedJoinGroups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.RequestedJoinGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.Albums)
                .FirstOrDefault(x => x.Id == id)
                .ExcludeAuthorization();
        }

        public UserDTO GetAuthById(string id)
        {
            return table.AsNoTracking()
                .Include(x => x.Albums)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.OwnedGroups)
                .FirstOrDefault(x => x.Id == id);
        }


        public UserDTO GetByName(string name)
        {
            return table.AsNoTracking()
                .FirstOrDefault(x => x.UserName == name || x.Email == name)
                .ExcludeAuthorization();
        }

        public UserDTO GetAuthByName(string name)
        {
            return table.AsNoTracking()
                .FirstOrDefault(x => x.UserName == name || x.Email == name);
        }

        public void IncrementFailedLogin(UserDTO user)
        {
            if (user.LockoutEnabled) { return; }

            user.AccessFailedCount++;

            if (user.AccessFailedCount >= 3)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddMinutes(5);
            }

            table.Update(user);
        }

        public bool IsLockedOut(UserDTO user)
        {
            if (user.LockoutEnabled)
            {
                if (user.LockoutEnd < DateTime.Now)
                {
                    user.LockoutEnd = null;
                    user.LockoutEnabled = false;

                    table.Update(user);

                    return false;
                }

                return true;
            }

            return false;
        }

        public bool ValidateOriginality(string name)
        {
            return !table.Any(x => x.UserName == name);
        }

        public IEnumerable<UserDTO> GetAll(int page = 1, int rpp = 10)
        {
            return table
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.OwnedGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.PendingJoinGroups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.PendingJoinGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.RequestedJoinGroups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.RequestedJoinGroups)
                    .ThenInclude(x => x.IconPhoto)

                .Include(x => x.Albums)
                .AsNoTracking()
                .Skip(rpp * (page - 1))
                .Take(rpp)
                .ExcludeAuthorization();
        }

        public int GetCount()
        {
            return table.Count();
        }

        public bool Update(UserDTO user)
        {
            if (!ValidateOriginality(user.UserName))
            {
                return false;
            }

            if (!Exists(user))
            {
                return false;
            }
            if (!EntityIsAttached(user))
            {
                if (KeyIsAttached(user))
                {
                    UserDTO local = GetAttachedEntityByEntity(user);

                    local.UserName = string.IsNullOrWhiteSpace(user.UserName) ? local.UserName : user.UserName;
                    local.Email = string.IsNullOrWhiteSpace(user.Email) ? local.Email : user.Email;
                    local.PasswordHash = user.PasswordHash ?? local.PasswordHash;
                    local.Rights = user.Rights != local.Rights ? user.Rights : local.Rights;
                    local.AccessFailedCount = user.AccessFailedCount != local.AccessFailedCount ? user.AccessFailedCount : local.AccessFailedCount;
                    local.LockoutEnabled = user.LockoutEnabled != local.LockoutEnabled ? user.LockoutEnabled : local.LockoutEnabled;
                    local.LockoutEnd = user.LockoutEnd ?? local.LockoutEnd;

                    dbContext.Entry(local).State = EntityState.Modified;
                }
                else dbContext.Entry(user).State = EntityState.Modified;
            }

            return dbContext.SaveChanges() > 0;
        }

        public bool Update()
        {
            return dbContext.SaveChanges() > 0;
        }

        public bool Remove(UserDTO user, string userId)
        {
            UserDTO removable = table.FirstOrDefault(x => x.Id == user.Id && (x.Id == userId || table.Any(x => x.Id == userId) ? table.FirstOrDefault(x => x.Id == userId).Rights >= Rights.Admin : false));
            if (removable == null) return true;
            table.Remove(removable);
            Logger.Log($"User {userId} removed user {removable.Id}.");
            return dbContext.SaveChanges() > 0;
        }
    }

    public static class DALExtensions
    {
        private const int maxDepth = 5;

        public static IEnumerable<UserDTO> ExcludeAuthorization(this IEnumerable<UserDTO> list, int depth = 0)
        {
            depth++;
            return list.Select(x => {
                x.PasswordHash = null;
                x.Salt = null;
                x.LockoutEnabled = false;
                x.LockoutEnd = null;
                x.AccessFailedCount = 0;
                return x;
            });
        }

        public static UserDTO ExcludeAuthorization(this UserDTO x, int depth = 0)
        {
            depth++;
            if (x == null) return x;

            x.PasswordHash = null;
            x.Salt = null;
            x.LockoutEnabled = false;
            x.LockoutEnd = null;
            x.AccessFailedCount = 0;
            return x;
        }

        public static IEnumerable<PhotoDTO> ExcludeAuthorization(this IEnumerable<PhotoDTO> list, int depth = 0)
        {
            depth++;
            return list.Select(x => {
                if (x.User != null && depth < maxDepth)
                    x.User = x.User.ExcludeAuthorization(depth);
                if (x.Albums != null && depth < maxDepth)
                    x.Albums = x.Albums.ExcludeAuthorization(depth).ToList();
                return x;
            });
        }
        public static PhotoDTO ExcludeAuthorization(this PhotoDTO x, int depth = 0)
        {
            depth++;
            if (x == null) return x;

            if (x.User != null && depth < maxDepth)
                x.User = x.User.ExcludeAuthorization(depth);
            if (x.Albums != null && depth < maxDepth)
                x.Albums = x.Albums.ExcludeAuthorization(depth).ToList();
            return x;
        }

        public static IEnumerable<AlbumDTO> ExcludeAuthorization(this IEnumerable<AlbumDTO> list, int depth = 0)
        {
            depth++;
            return list.Select(x => {
                if (x.User != null && depth < maxDepth)
                    x.User = x.User.ExcludeAuthorization(depth);
                if(x.IconPhoto != null && x.IconPhoto.User != null && depth < maxDepth)
                    x.IconPhoto.User = x.IconPhoto.User.ExcludeAuthorization(depth);
                if (x.Photos != null && depth < maxDepth)
                    x.Photos = x.Photos.ExcludeAuthorization(depth).ToList();
                if (x.Groups != null && depth < maxDepth)
                    x.Groups = x.Groups.ExcludeAuthorization(depth).ToList();
                return x;
            });
        }

        public static AlbumDTO ExcludeAuthorization(this AlbumDTO x, int depth = 0)
        {
            depth++;
            if (x == null) return x;

            if (x.User != null && depth < maxDepth)
                x.User = x.User.ExcludeAuthorization(depth);
            if (x.IconPhoto != null && x.IconPhoto.User != null && depth < maxDepth)
                x.IconPhoto.User = x.IconPhoto.User.ExcludeAuthorization(depth);
            if (x.Photos != null && depth < maxDepth)
                x.Photos = x.Photos.ExcludeAuthorization(depth).ToList();
            if (x.Groups != null && depth < maxDepth)
                x.Groups = x.Groups.ExcludeAuthorization(depth).ToList();
            return x;
        }

        public static IEnumerable<GroupDTO> ExcludeAuthorization(this IEnumerable<GroupDTO> list, int depth = 0)
        {
            depth++;
            return list.Select(x => {
                if (x.Creator != null && depth < maxDepth)
                    x.Creator = x.Creator.ExcludeAuthorization();
                if (x.Users != null && depth < maxDepth)
                    x.Users = x.Users.ExcludeAuthorization(depth).ToList();
                if (x.PendingJoinUsers != null && depth < maxDepth)
                    x.PendingJoinUsers = x.PendingJoinUsers.ExcludeAuthorization(depth).ToList();
                if (x.RequestedJoinUsers != null && depth < maxDepth)
                    x.RequestedJoinUsers = x.RequestedJoinUsers.ExcludeAuthorization(depth).ToList();
                if (x.IconPhoto != null && x.IconPhoto.User != null && depth < maxDepth)
                    x.IconPhoto.User = x.IconPhoto.User.ExcludeAuthorization(depth);
                return x;
            });
        }

        public static GroupDTO ExcludeAuthorization(this GroupDTO x, int depth = 0)
        {
            depth++;
            if (x == null) return x;

            if (x.Users != null && depth < maxDepth)
                x.Users = x.Users.ExcludeAuthorization(depth).ToList();
            if (x.PendingJoinUsers != null && depth < maxDepth)
                x.PendingJoinUsers = x.PendingJoinUsers.ExcludeAuthorization(depth).ToList();
            if (x.RequestedJoinUsers != null && depth < maxDepth)
                x.RequestedJoinUsers = x.RequestedJoinUsers.ExcludeAuthorization(depth).ToList();
            if (x.IconPhoto != null && x.IconPhoto.User != null && depth < maxDepth)
                x.IconPhoto.User = x.IconPhoto.User.ExcludeAuthorization(depth);
            return x;
        }
    }
}
