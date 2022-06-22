using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface ITagsDAL
    {
        public TagDTO GetTracked(int id);
        public bool Update();
        public TagDTO GetById(int id);
        public TagDTO GetByName(string name);
        public TagDTO Create(TagDTO tag);
        public bool Remove(TagDTO tag);
    }
}
