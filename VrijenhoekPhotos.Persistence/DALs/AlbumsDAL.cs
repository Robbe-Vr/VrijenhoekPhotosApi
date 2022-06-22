using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class AlbumsDAL : IntIdentifiedBaseDAL<AlbumDTO>, IAlbumsDAL
    {
        public AlbumsDAL(VrijenhoekPhotosDbContext context) : base(context) { }

        public AlbumDTO Create(AlbumDTO album)
        {
            if (table.Any(x => x.Name == album.Name && x.UserId == album.UserId))
            {
                return table.AsNoTracking().FirstOrDefault(x => x.Name == album.Name && x.UserId == album.UserId);
            }

            album.User = null;
            table.Add(album);

            return dbContext.SaveChanges() > 0 ?
                table.AsNoTracking().FirstOrDefault(x => x.Name == album.Name && x.UserId == album.UserId) : null;
        }

        public IEnumerable<AlbumDTO> GetAllByUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<AlbumDTO> results = table
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Photos)
                    .ThenInclude(x => x.User)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.User)
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

            return table
                .Count(x => x.UserId == id);
        }

        public IEnumerable<AlbumDTO> GetAllJoinedByUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<AlbumDTO> results = table
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Photos)
                    .ThenInclude(x => x.User)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.UserId != userId && x.Groups.Any(x => x.Users.Any(x => x.Id == userId) || x.CreatorId == userId));

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

        public int GetJoinedByUserCount(string userId, string nameFilter = "")
        {
            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                return table
                    .Include(x => x.Groups)
                        .ThenInclude(x => x.Users)
                    .Include(x => x.Groups)
                        .ThenInclude(x => x.Creator)
                    .Where(x => x.Name.Contains(nameFilter) || (x.User != null ? x.User.UserName.Contains(nameFilter) : false))
                    .Count(x => x.UserId != userId && x.Groups.Any(x => x.Users.Any(x => x.Id == userId) || x.CreatorId == userId));
            }

            return table
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Count(x => x.UserId != userId && x.Groups.Any(x => x.Users.Any(x => x.Id == userId) || x.CreatorId == userId));
        }

        public IEnumerable<AlbumDTO> GetAllAvailableToUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IQueryable<AlbumDTO> results = table
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Photos)
                    .ThenInclude(x => x.User)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.Groups.Any(x => x.Users.Any(x => x.Id == id) || x.CreatorId == id) || x.UserId == id);

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

        public int GetAvailableToUserCount(string id, string nameFilter = "")
        {
            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                return table
                    .Include(x => x.Groups)
                        .ThenInclude(x => x.Users)
                    .Include(x => x.Groups)
                        .ThenInclude(x => x.Creator)
                    .Count(x => x.Groups.Any(x => x.Users.Any(x => x.Id == id) || x.CreatorId == id) || x.UserId == id);
            }

            return table
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Count(x => x.Groups.Any(x => x.Users.Any(x => x.Id == id) || x.CreatorId == id) || x.UserId == id);
        }

        public IEnumerable<PhotoDTO> GetPhotosFromAlbum(int id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest)
        {
            IEnumerable<PhotoDTO> results = table
                .Include(x => x.Photos)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id)?.Photos;

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

        public int GetPhotosFromAlbumCount(int id, string nameFilter = "")
        {
            if (!String.IsNullOrWhiteSpace(nameFilter) && nameFilter.Length > 1)
            {
                return table
                    .Include(x => x.Photos)
                        .ThenInclude(x => x.User)
                    .FirstOrDefault(x => x.Id == id)?.Photos
                    .Count(x => x.Name.Contains(nameFilter) || (x.User != null ? x.User.UserName.Contains(nameFilter) : false)) ?? 0;
            }

            return table
                .Include(x => x.Photos)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id)?.Photos
                .Count() ?? 0;
        }


        public AlbumDTO GetTracked(int id)
        {
            return table
                .Include(x => x.IconPhoto)
                .Include(x => x.Tags)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.User)
                .Include(x => x.Photos)
                .FirstOrDefault(x => x.Id == id);
        }

        public AlbumDTO GetById(int id)
        {
            return table
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id)
                .ExcludeAuthorization();
        }

        public bool CheckAccess(string userId, Rights userRights, int id)
        {
            if (userRights == Rights.Admin) return true;

            AlbumDTO album = table
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .FirstOrDefault(x => x.Id == id);

            return userRights >= Rights.CanRead && (
                    album.UserId == userId ||
                    album.Groups.Any(x => x.CreatorId == userId || x.Users.Any(x => x.Id == userId))
                );
        }

        public AlbumDTO GetByName(string name, string userId)
        {
            return table
                .Include(x => x.IconPhoto)
                    .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Creator)
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => x.UserId == userId && x.Name == name)
                .ExcludeAuthorization();
        }

        public bool Update(AlbumDTO album)
        {
            album.User = null;
            album.IconPhoto = null;
            album.Photos = album.Photos.Select(x => { x.User = null; x.Albums = null; return x; }).ToList();
            album.Tags = album.Tags.Select(x => { x.User = null; x.Albums = null; return x; }).ToList();

            if (!Exists(album))
            {
                return false;
            }
            if (!EntityIsAttached(album))
            {
                if (KeyIsAttached(album))
                {
                    AlbumDTO local = GetAttachedEntityByEntity(album);

                    local.Name = string.IsNullOrWhiteSpace(album.Name) ? local.Name : album.Name;
                    local.IconPhotoId = album.IconPhotoId ?? local.IconPhotoId;
                    local.Photos = album.Photos ?? local.Photos;
                    local.Tags = album.Tags ?? local.Tags;

                    dbContext.Entry(local).State = EntityState.Modified;
                }
                else dbContext.Entry(album).State = EntityState.Modified;
            }

            UpdatePhotos(album);
            UpdateTags(album);
            
            return dbContext.SaveChanges() > 0;
        }

        public bool Update()
        {
            return dbContext.SaveChanges() > 0;
        }

        private void UpdatePhotos(AlbumDTO album)
        {
            List<AlbumPhotos> currentList = dbContext.AlbumPhotos.Where(x => x.AlbumId == album.Id).ToList();

            AlbumPhotos[] removedArr = new AlbumPhotos[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<AlbumPhotos> removed = removedArr.ToList();
            foreach (PhotoDTO photo in album.Photos)
            {
                if (!currentList.Any(x => x.PhotoId == photo.Id))
                {
                    AlbumPhotos newRow = new AlbumPhotos()
                    {
                        AlbumId = album.Id,
                        PhotoId = photo.Id,
                    };

                    dbContext.AlbumPhotos.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.PhotoId == photo.Id);
                }
            }

            foreach (AlbumPhotos item in removed)
            {
                dbContext.AlbumPhotos.Remove(item);
            }
        }

        private void UpdateTags(AlbumDTO album)
        {
            List<AlbumTags> currentList = dbContext.AlbumTags.Where(x => x.AlbumId == album.Id).ToList();

            AlbumTags[] removedArr = new AlbumTags[currentList.Count];
            currentList.CopyTo(removedArr, 0);
            List<AlbumTags> removed = removedArr.ToList();
            foreach (TagDTO tag in album.Tags)
            {
                if (!currentList.Any(x => x.TagId == tag.Id))
                {
                    AlbumTags newRow = new AlbumTags()
                    {
                        AlbumId = album.Id,
                        TagId = tag.Id,
                    };

                    dbContext.AlbumTags.Add(newRow);
                }
                else
                {
                    removed.RemoveAll(x => x.TagId == tag.Id);
                }
            }

            foreach (AlbumTags item in removed)
            {
                dbContext.AlbumTags.Remove(item);
            }
        }

        public bool Remove(AlbumDTO album, string userId)
        {
            AlbumDTO removable = table.Find(album.Id);
            if (removable.UserId == userId)
            {
                table.Remove(removable);
                Logger.Log($"User {userId} removed photo with id {removable.Id} owned by user {album.UserId}.");
                return dbContext.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
