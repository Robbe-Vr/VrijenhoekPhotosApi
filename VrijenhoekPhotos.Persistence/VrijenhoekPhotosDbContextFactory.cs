using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrijenhoekPhotos.Persistence
{
    public class VrijenhoekPhotosDbContextFactory : IDesignTimeDbContextFactory<VrijenhoekPhotosDbContext>
    {
        public VrijenhoekPhotosDbContext CreateDbContext(string[] args)
        {
            AppConfiguration appConfig = new AppConfiguration();
            var opsBuilder = new DbContextOptionsBuilder<VrijenhoekPhotosDbContext>();
            opsBuilder.UseSqlServer(appConfig.sqlConnectionString);
            return new VrijenhoekPhotosDbContext(opsBuilder.Options);
        }
    }

}
