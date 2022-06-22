using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IGroupsDAL
    {
        public IEnumerable<GroupDTO> GetAllByUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetByUserCount(string id, string nameFilter = "");
        public IEnumerable<GroupDTO> SeekAll(string filter, int page = 1, int rpp = 10, Sorting sorting = Sorting.Newest_Oldest);
        public int GetSeekAllCount(string filter);
        public IEnumerable<GroupDTO> GetAllAvailableToUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetAvailableToUserCount(string id, string nameFilter = "");
        public IEnumerable<GroupDTO> GetAllJoinedByUser(string userId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetJoinedByUserCount(string id, string nameFilter = "");
        public IEnumerable<AlbumDTO> GetAlbumsFromGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetAlbumsFromGroupCount(int groupId, string nameFilter = "");
        public IEnumerable<UserDTO> GetUsersFromGroup(int groupId, int page = 1, int rpp = 10, string nameFilter = "", Sorting sorting = Sorting.Newest_Oldest);
        public int GetUsersFromGroupCount(int groupId, string nameFilter = "");
        public GroupDTO GetOwnedById(int id);
        public GroupDTO GetTracked(int id);
        public GroupDTO GetById(int id);
        public GroupDTO GetByName(string name);
        public GroupDTO Create(GroupDTO group);
        public bool Remove(GroupDTO group, string userId);
        public bool Update();
        public bool Update(GroupDTO group);

        public bool CheckAccess(string userId, Rights userRights, int id);
    }
}
