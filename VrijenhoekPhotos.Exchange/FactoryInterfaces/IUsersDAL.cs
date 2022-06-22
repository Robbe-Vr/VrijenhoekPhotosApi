using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IUsersDAL
    {
        public IEnumerable<UserDTO> GetAll(int page = 1, int rpp = 10);
        public int GetCount();
        public UserDTO Create(UserDTO user);
        public UserDTO GetTracked(string id);
        public UserDTO GetById(string id);
        public UserDTO GetAuthById(string id);
        public UserDTO GetByName(string name);
        public UserDTO GetAuthByName(string name);
        bool IsLockedOut(UserDTO user);
        void IncrementFailedLogin(UserDTO user);
        public bool Update();
        public bool Update(UserDTO user);
        public bool Remove(UserDTO user, string userId);
        public bool ValidateOriginality(string name);
    }
}
