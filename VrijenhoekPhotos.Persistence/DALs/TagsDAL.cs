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
    public class TagsDAL : IntIdentifiedBaseDAL<TagDTO>, ITagsDAL
    {
        public TagsDAL(VrijenhoekPhotosDbContext context) : base(context) { }

        public TagDTO GetTracked(int id)
        {
            return table
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);
        }

        public bool Update()
        {
            return dbContext.SaveChanges() > 0;
        }

        public TagDTO GetById(int id)
        {
            return table.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public TagDTO GetByName(string name)
        {
            return table.AsNoTracking().FirstOrDefault(x => x.Name == name);
        }

        public TagDTO Create(TagDTO tag)
        {
            if (table.Any(x => x.Name == tag.Name))
                return table.FirstOrDefault(x => x.Name == tag.Name);

            tag.User = null;

            table.Add(tag);

            return dbContext.SaveChanges() > 0 ?
                table.FirstOrDefault(x => x.Name == tag.Name) : null;
        }

        public bool Remove(TagDTO tag)
        {
            TagDTO removable = table.Find(tag.Id);
            if (removable != null)
            {
                table.Remove(removable);
                Logger.Log($"Removed tag with id {removable.Id} owned by user {tag.UserId}.");
                return dbContext.SaveChanges() > 0;
            }

            return true;
        }
    }
}
