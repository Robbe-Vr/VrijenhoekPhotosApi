using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Api.Filters;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Controllers
{
    [EnableCors("VrijenhoekPhotosCorsPolicy")]
    [Authorized((int)Rights.CanRead)]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ProcessingController
    {
        private PhotosHandler _photos;
        public PhotosController(PhotosHandler photos, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            _photos = photos;
        }

        [HttpGet]
        [Authorized((int)Rights.CanRead)]
        public IActionResult All([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _photos.All(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Count([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _photos.Count(nameFilter);
            });
        }

        [HttpGet("id/{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetById(int id)
        {
            return Process(() =>
            {
                return _photos.ById(id);
            });
        }

        [HttpGet("webcontent/{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetWebPhotoById(int id)
        {
            try
            {
                if (id < 1) return Problem("Invalid Id.");

                PhotoDTO photo = _photos.ById(id)?.Result as PhotoDTO;
                if (photo == null) return Problem("Access to this photo denied for current user.");

                FilePhotoDTO file = _photos.GetWebViewableFilePhoto(photo)?.Result as FilePhotoDTO;

                FileStreamResult stream = new FileStreamResult(file.Stream, FileTypeToContentType(file.Type));
                stream.EnableRangeProcessing = true;
                return stream;
            }
            catch (Exception e)
            {
                Logger.Log($"Error during content stream!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                return Problem(
                    detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                    title: "Error Occurred!",
                    statusCode: 501
                );
            }
        }

        [HttpGet("content/{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetPhotoById(int id)
        {
            return Process(() =>
            {
                if (id < 1) return Problem("Invalid Id.");

                PhotoDTO photo = _photos.ById(id)?.Result as PhotoDTO;
                if (photo == null) return Problem("Access to this photo denied for current user.");

                FilePhotoDTO file = _photos.GetFilePhoto(photo)?.Result as FilePhotoDTO;

                return File(System.IO.File.OpenRead(file.ContentLocation), FileTypeToContentType(file.Type));
            });
        }

        [HttpGet("thumbnail/{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetThumbnailById(int id)
        {
            return Process(() =>
            {
                if (id < 1) return Problem("Invalid Id.");

                PhotoDTO photo = _photos.ById(id)?.Result as PhotoDTO;
                if (photo == null) return Problem("Access to this photo denied for current user.");

                FilePhotoDTO file = _photos.GetFilePhotoThumbnail(photo)?.Result as FilePhotoDTO;

                if (file.ThumbnailLocation == null) return Problem("Error retrieving thumbnail!");

                return File(System.IO.File.OpenRead(file.ThumbnailLocation), FileTypeToContentType(file.Type));
            });
        }

        private string FileTypeToContentType(string type)
        {
            return type switch
            {
                ".mp4" => "video/mp4",
                ".mpeg" => "video/mpeg",
                ".webm" => "video/webm",
                ".avi" => "video/avi",
                ".mov" => "video/mov",

                ".webp" => "image/webp",
                ".tif" => "image/tiff",
                ".tiff" => "image/tiff",
                ".svg" => "image/svg+xml",
                ".ico" => "image/vnd.microsoft.icon",
                ".gif" => "image/gif",
                ".avif" => "image/avif",
                ".bmp" => "image/bmp",
                ".png" => "image/png",
                ".jpeg" => "image/jpeg",
                ".jpg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }

        [HttpGet("name/{name}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetByName(string name)
        {
            return Process(() =>
            {
                return _photos.ByName(name);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Update(PhotoDTO photo)
        {
            return Process(() =>
            {
                return _photos.UpdatePhoto(photo);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Remove(PhotoDTO photo)
        {
            return Process(() =>
            {
                return _photos.RemovePhoto(photo);
            });
        }
    }
}
