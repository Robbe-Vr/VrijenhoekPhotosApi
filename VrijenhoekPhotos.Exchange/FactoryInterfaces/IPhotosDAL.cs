using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IPhotosDAL
    {
        public IEnumerable<PhotoDTO> GetAllByUser(string id, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetByUserCount(string id, string nameFilter = "");
        public PhotoDTO GetTracked(int id);
        public PhotoDTO GetById(int id);
        public PhotoDTO GetByName(string name);
        public PhotoDTO Create(PhotoDTO photo);
        public bool Update();
        public bool Update(PhotoDTO photo);
        public bool Remove(PhotoDTO photo, string userId);

        public bool CheckAccess(string userId, Rights userRights, int id);
    }
}
