using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IAlbumsDAL
    {
        public IEnumerable<AlbumDTO> GetAllByUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetByUserCount(string id, string nameFilter = "");
        public IEnumerable<AlbumDTO> GetAllJoinedByUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetJoinedByUserCount(string id, string nameFilter = "");
        public IEnumerable<PhotoDTO> GetPhotosFromAlbum(int id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetPhotosFromAlbumCount(int id, string nameFilter = "");
        public IEnumerable<AlbumDTO> GetAllAvailableToUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetAvailableToUserCount(string id, string nameFilter = "");
        public AlbumDTO GetTracked(int id);
        public AlbumDTO GetById(int id);
        public AlbumDTO GetByName(string name, string userId);
        public AlbumDTO Create(AlbumDTO album);
        public bool Update();
        public bool Update(AlbumDTO album);
        public bool Remove(AlbumDTO album, string userId);

        public bool CheckAccess(string userId, Rights userRights, int id);
    }
}
