using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Core.Handlers
{
    public class TagsHandler
    {
        private ITagsDAL _dal;
        private UserInfo _userInfo;

        public TagsHandler(ITagsDAL dal, UserInfo userInfo)
        {
            _dal = dal;
            _userInfo = userInfo;
        }
    }
}
