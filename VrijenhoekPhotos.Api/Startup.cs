using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Api.Authorization;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Persistence.DALs;
using VrijenhoekPhotos.Persistence;
using VrijenhoekPhotos.Exchange;
using VrijenhoekPhotos.FileSystem;
using Microsoft.AspNetCore.Http.Features;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace VrijenhoekPhotos.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string VrijenhoekPhotosApiCorsPolicy = "VrijenhoekPhotosCorsPolicy";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: VrijenhoekPhotosApiCorsPolicy, builder =>
                {
                    builder.WithOrigins("http://localhost:2813", "https://localhost:2813",
                                        "http://192.168.2.101:2813", "https://192.168.2.101:2813",
                                        "http://photos.sywapps.com", "https://photos.sywapps.com")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.MaxDepth = 6;
                });

            services.AddHttpContextAccessor();

            Func<HttpContext, UserDTO> getUserByToken = new Func<HttpContext, UserDTO>((HttpContext context) =>
            {
                StringValues accessToken = context.Request.Headers["VrijenhoekPhotos_AccessToken"];

                if (String.IsNullOrEmpty(accessToken) && (Regex.IsMatch(context.Request.Path.Value, "/api/photos/webcontent/[0-9]+") && context.Request.Query.TryGetValue("VrijenhoekPhotos_AccessToken", out accessToken)))
                {
                    accessToken = accessToken.ToString().Replace(' ', '+');
                }

                return AuthManager.GetUser(accessToken);
            });

            //services.AddScoped(x => new AuthorizationHandler(new UsersDAL(new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions))));
            //services.AddScoped(x => new PhotosHandler(new PhotosDAL(new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions)), new FileSystemPhotoDAL(), new UserInfo(getUserByToken(x.GetService<IHttpContextAccessor>().HttpContext))));
            //services.AddScoped(x => new AlbumsHandler(new AlbumsDAL(new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions), ), new UserInfo(getUserByToken(x.GetService<IHttpContextAccessor>().HttpContext))));
            //services.AddScoped(x => new GroupsHandler(new GroupsDAL(new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions)), new UserInfo(getUserByToken(x.GetService<IHttpContextAccessor>().HttpContext))));

            services.AddScoped(x => new VrijenhoekPhotosDbContext(VrijenhoekPhotosDbContext.ops.dbOptions));

            services.AddScoped(x => new UserInfo(getUserByToken(x.GetService<IHttpContextAccessor>().HttpContext)));

            services.AddScoped<PhotosHandler>();
            services.AddScoped<IPhotosDAL, PhotosDAL>();
            services.AddScoped<IFilePhotosDAL, FileSystemPhotoDAL>();

            services.AddScoped<AlbumsHandler>();
            services.AddScoped<IAlbumsDAL, AlbumsDAL>();

            services.AddScoped<TagsHandler>();
            services.AddScoped<ITagsDAL, TagsDAL>();

            services.AddScoped<GroupsHandler>();
            services.AddScoped<IGroupsDAL, GroupsDAL>();

            services.AddScoped<AuthorizationHandler>();
            services.AddScoped<UsersHandler>();
            services.AddScoped<IUsersDAL, UsersDAL>();

            services.AddSingleton(x =>
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

                jsonSettings.Formatting = Formatting.Indented;
                jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsonSettings.MaxDepth = 6;

                return jsonSettings;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                ApplicationEnvironmentData.InDevelopment = true;
            }
            else
            {
                ApplicationEnvironmentData.InDevelopment = false;
            }

            app.UseRouting();

            app.UseCors(VrijenhoekPhotosApiCorsPolicy);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
