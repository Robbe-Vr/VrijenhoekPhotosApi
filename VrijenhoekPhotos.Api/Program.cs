using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Core.Seeding;
using VrijenhoekPhotos.Exchange;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.FileSystem;
using VrijenhoekPhotos.Persistence;
using VrijenhoekPhotos.Persistence.DALs;

namespace VrijenhoekPhotos.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost build = CreateHostBuilder(args).Build();
            
            try
            {
                Logger.SourceAppName = nameof(VrijenhoekPhotos);

                Task.Run(() =>
                {
                    bool nas_seeding = false;

                    ApplicationEnvironmentData.InDevelopment = ((IWebHostEnvironment)build.Services.GetService(typeof(IWebHostEnvironment))).IsDevelopment();

                    if (ApplicationEnvironmentData.InDevelopment && nas_seeding)
                    {
                        Action<int, int> addPhotoToAlbum = (albumId, photoId) =>
                        {
                            VrijenhoekPhotosDbContext context = new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions);
                            if (context.AlbumPhotos.All(x => x.PhotoId != photoId))
                            {
                                context.AlbumPhotos.Add(new Persistence.RelationClasses.AlbumPhotos()
                                {
                                    AlbumId = albumId,
                                    PhotoId = photoId,
                                });
                                context.SaveChanges();
                            }
                        };

                        UserInfo info = new UserInfo(new UserDTO() { Id = "fe3bf52c-952d-4216-8997-595ea07732c8", UserName = "admin", Email = "robbe.vrijenhoek@kpnmail.nl", Rights = Rights.Admin });
                        NASFolderSeeding.Seed(
                            addPhotoToAlbum,
                            () =>
                            {
                                VrijenhoekPhotosDbContext context = new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions);
                                //context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                                return new Core.Handlers.AlbumsHandler(new AlbumsDAL(context), new TagsDAL(context), new PhotosDAL(context), info);
                            },
                            () =>
                            {
                                VrijenhoekPhotosDbContext context = new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions);
                                //context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                                return new Core.Handlers.PhotosHandler(new PhotosDAL(context), new FileSystemPhotoDAL(), info);
                            },
                            test: false
                        );
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }

            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
