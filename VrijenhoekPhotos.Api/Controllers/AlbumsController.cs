using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrijenhoekPhotos.Api.Filters;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Controllers
{
    [EnableCors("VrijenhoekPhotosCorsPolicy")]
    [Authorized((int)Rights.CanRead)]
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ProcessingController
    {
        private AlbumsHandler _albums;
        public AlbumsController(AlbumsHandler albums, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            _albums = albums;
        }

        [HttpGet]
        [Authorized((int)Rights.CanRead)]
        public IActionResult All([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _albums.All(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Count([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _albums.Count(nameFilter);
            });
        }

        [HttpGet("joined")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AllJoined([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _albums.AllJoined(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("joined/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult JoinedCount([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _albums.JoinedCount(nameFilter);
            });
        }

        [HttpGet("owned")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AllOwned([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _albums.AllOwned(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("owned/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult OwnedCount([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _albums.OwnedCount(nameFilter);
            });
        }

        [HttpGet("{albumId}/photos")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Photos(int albumId, [FromQuery] int page, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _albums.PhotosFromAlbum(albumId, page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("{albumId}/photos/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AlbumsCount(int albumId, [FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _albums.PhotosFromAlbumCount(albumId, nameFilter);
            });
        }

        [HttpGet("{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetById(int id)
        {
            return Process(() =>
            {
                return _albums.ById(id);
            });
        }

        [HttpPost("{albumId}/addtag")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult AddTagToAlbum(int albumId, TagDTO tag)
        {
            return Process(() =>
            {
                return _albums.ForceAddTag(albumId, tag);
            });
        }

        [HttpPost("{albumId}/removetag/{tagId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult RemoveTagFromAlbum(int albumId, int tagId)
        {
            return Process(() =>
            {
                return _albums.RemoveTag(albumId, tagId);
            });
        }

        [HttpPost("{albumId}/add/{photoId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult AddPhotoToAlbum(int albumId, int photoId)
        {
            return Process(() =>
            {
                return _albums.AddPhoto(albumId, photoId);
            });
        }

        [HttpPost("{albumId}/remove/{photoId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult RemovePhotoFromAlbum(int albumId, int photoId)
        {
            return Process(() =>
            {
                return _albums.RemovePhoto(albumId, photoId);
            });
        }

        [HttpPost("{action}/{name}")]
        [Authorized((int)Rights.CanUpdate)]
        public IActionResult Create(string name)
        {
            return Process(() =>
            {
                return _albums.Create(name);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanUpdate)]
        public IActionResult Update(AlbumDTO album)
        {
            return Process(() =>
            {
                return _albums.Update(album);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Remove(AlbumDTO album)
        {
            return Process(() =>
            {
                return _albums.Remove(album);
            });
        }
    }
}
