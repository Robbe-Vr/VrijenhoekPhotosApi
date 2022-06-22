using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;

namespace VrijenhoekPhotos.Persistence.DALs
{
    public abstract class BaseDAL<TDTO> where TDTO : class
    {
        public VrijenhoekPhotosDbContext dbContext;
        public DbSet<TDTO> table;
        public BaseDAL(VrijenhoekPhotosDbContext context)
        {
            dbContext = context;
            table = dbContext.GetTable<TDTO>(typeof(TDTO).Name.Replace("DTO", "s").Replace("Auth", string.Empty));

            dbContext.SaveChangesFailed += (sender, e) =>
            {
                Logger.Log("EF Failed to save changes to db!\nError: " + e.Exception.Message + "\n" + e.Exception.StackTrace);
            };
        }

        public bool EntityIsAttached(TDTO entity)
        {
            return dbContext.Set<TDTO>().Local.Any(i => i == entity);
        }
    }

    public abstract class StringIdentifiedBaseDAL<TDTO> : BaseDAL<TDTO> where TDTO : class, IIdentifier<string>
    {
        public StringIdentifiedBaseDAL(VrijenhoekPhotosDbContext context) : base(context)
        {
        }

        public void DetachLocal(TDTO obj)
        {
            dbContext.DetachLocal(obj, obj.Id);
        }

        public bool KeyIsAttached(TDTO entity)
        {
            return dbContext.Set<TDTO>().Local.Any(i => i.Id == entity.Id);
        }

        public TDTO GetAttachedEntityByEntity(TDTO entity)
        {
            return dbContext.Set<TDTO>().Local.FirstOrDefault(i => i.Id == entity.Id);
        }

        public bool Exists(TDTO entity)
        {
            return dbContext.Set<TDTO>().Any(i => i.Id == entity.Id);
        }
    }

    public abstract class IntIdentifiedBaseDAL<TDTO> : BaseDAL<TDTO> where TDTO : class, IIdentifier<int>
    {
        public IntIdentifiedBaseDAL(VrijenhoekPhotosDbContext context) : base(context)
        {
        }

        public void DetachLocal(TDTO obj)
        {
            dbContext.DetachLocal(obj, obj.Id);
        }
        public bool KeyIsAttached(TDTO entity)
        {
            return dbContext.Set<TDTO>().Local.Any(i => i.Id == entity.Id);
        }

        public TDTO GetAttachedEntityByEntity(TDTO entity)
        {
            return dbContext.Set<TDTO>().Local.FirstOrDefault(i => i.Id == entity.Id);
        }

        public bool Exists(TDTO entity)
        {
            return dbContext.Set<TDTO>().Any(i => i.Id == entity.Id);
        }
    }

    public static class ContextExtensions
    {
        public static void DetachLocal<TDTO>(this VrijenhoekPhotosDbContext dbContext, TDTO obj, int id) where TDTO : class, IIdentifier<int>
        {
            TDTO local = dbContext.Set<TDTO>().Local.FirstOrDefault(x => x.Id == id);
            if (local != null)
            {
                dbContext.Entry(obj).State = EntityState.Detached;
            }
        }

        public static void DetachLocal<TDTO>(this VrijenhoekPhotosDbContext dbContext, TDTO obj, string id) where TDTO : class, IIdentifier<string>
        {
            TDTO local = dbContext.Set<TDTO>().Local.FirstOrDefault(x => x.Id == id);
            if (local != null)
            {
                dbContext.Entry(obj).State = EntityState.Detached;
            }
        }
    }
}
