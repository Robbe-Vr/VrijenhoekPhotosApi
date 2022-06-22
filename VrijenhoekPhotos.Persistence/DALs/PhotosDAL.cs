using Microsoft.EntityFrameworkCore;
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
    public class PhotosDAL : IntIdentifiedBaseDAL<PhotoDTO>, IPhotosDAL
    {
        public PhotosDAL(VrijenhoekPhotosDbContext context) : base(context) { }

        public PhotoDTO Create(PhotoDTO photo)
        {
            if (!ValidateOriginality(photo.Name, photo.Type, photo.UserId))
            {
                return table.AsNoTracking().FirstOrDefault(x => x.Name == photo.Name && x.UserId == photo.UserId);
            }
            photo.User = null;

            table.Add(photo);
            
            return dbContext.SaveChanges() > 0 ?
                table.AsNoTracking().FirstOrDefault(x => x.Name == photo.Name && x.UserId == photo.UserId) : null;
        }

        public IEnumerable<PhotoDTO> GetAllByUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<PhotoDTO> results = table
                .Include(x => x.User)
                .Include(x => x.Albums)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .Where(x => x.UserId == id);

            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                results = results.Where(x => x.Name.Contains(nameFilter) || (x.User != null ? x.User.UserName.Contains(nameFilter) : false));
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

        public int GetByUserCount(string id, string nameFilter = "")
        {
            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                return table.Where(x => x.Name.Contains(nameFilter) || (x.User != null ? x.User.UserName.Contains(nameFilter) : false))
                    .Count(x => x.UserId == id);
            }

            return table.Count(x => x.UserId == id);
        }

        public PhotoDTO GetTracked(int id)
        {
            return table
                .Include(x => x.User)
                .Include(x => x.Albums)
                .FirstOrDefault(x => x.Id == id);
        }

        public PhotoDTO GetById(int id)
        {
            return table
                .Include(x => x.User)
                .Include(x => x.Albums)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id)
                .ExcludeAuthorization();
        }

        public bool CheckAccess(string userId, Rights userRights, int id)
        {
            if (userRights == Rights.Admin) return true;

            PhotoDTO photo = table
                .Include(x => x.Albums)
                    .ThenInclude(x => x.Groups)
                        .ThenInclude(x => x.Users)
                .FirstOrDefault(x => x.Id == id);

            return userRights >= Rights.CanRead && (
                    photo.UserId == userId ||
                    photo.Albums.Any(x => x.UserId == userId ||
                        x.Groups.Any(x => x.CreatorId == userId || x.Users.Any(x => x.Id == userId)))
                );
        }

        public PhotoDTO GetByName(string name)
        {
            return table.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Albums)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Name == name)
                .ExcludeAuthorization();
        }

        public bool Update(PhotoDTO photo)
        {
            if (!ValidateOriginality(photo.Name, photo.Type, photo.UserId))
            {
                return false;
            }
            photo.User = null;

            if (!Exists(photo))
            {
                return false;
            }
            if (!EntityIsAttached(photo))
            {
                if (KeyIsAttached(photo))
                {
                    PhotoDTO local = GetAttachedEntityByEntity(photo);

                    local.Name = string.IsNullOrWhiteSpace(photo.Name) ? local.Name : photo.Name;
                    
                    dbContext.Entry(local).State = EntityState.Modified;
                }
                else dbContext.Entry(photo).State = EntityState.Modified;
            }

            return dbContext.SaveChanges() > 0;
        }

        public bool Update()
        {
            return dbContext.SaveChanges() > 0;
        }

        public bool Remove(PhotoDTO photo, string userId)
        {
            PhotoDTO removable = table.Include(x => x.User).FirstOrDefault(x => x.Id == photo.Id && (x.UserId == userId || x.User.Rights >= Rights.CanRemove));
            if (removable == null) return true;
            table.Remove(removable);
            Logger.Log($"User {userId} removed photo with id {removable.Id} owned by user {photo.UserId}.");
            return dbContext.SaveChanges() > 0;
        }

        public bool ValidateOriginality(string name, string type, string userId)
        {
            return !String.IsNullOrEmpty(name) &&
                !String.IsNullOrEmpty(type) &&
                !String.IsNullOrEmpty(userId) &&
                !table.Any(x => x.Name == name && x.Type == type && x.UserId != userId);
        }
    }
}
