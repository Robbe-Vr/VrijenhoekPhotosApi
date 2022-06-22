using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Api.Filters;
using VrijenhoekPhotos.Api.Models;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Controllers
{
    [EnableCors("VrijenhoekPhotosCorsPolicy")]
    [Authorized((int)Rights.CanCreate)]
    [Route("api/[controller]")]
    [ApiController]
    [RequestFormLimits(ValueLengthLimit = 2_000_000_000, MultipartBodyLengthLimit = 2_000_000_000)]
    public class UploadController : ProcessingController
    {
        private PhotosHandler handler;
        public UploadController(PhotosHandler photosHandler, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            handler = photosHandler;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult Upload([FromForm]FilePhotoModel model)
        {
            return Process(() =>
            {
                CreateModel response = new CreateModel()
                {
                    Result = "Success.",
                    NewId = "",
                };
                try
                {
                    FilePhotoDTO photo = new FilePhotoDTO()
                    {
                        CreationDate = model.CreationDate == default ? DateTime.UtcNow : model.CreationDate,
                        Name = model.Name ?? Path.GetFileNameWithoutExtension(model.ImageFile.FileName),
                        Type = model.Type ?? Path.GetExtension(model.ImageFile.FileName),
                        UserId = model.UserId,
                    };
                    using (MemoryStream stream = new MemoryStream())
                    {
                        model.ImageFile.CopyTo(stream);

                        photo.Content = stream.ToArray();
                    }

                    photo = handler.AddPhoto(photo)?.Result as FilePhotoDTO;
                    response.NewId = photo.Id.ToString();
                    response.CreatedObject = photo;

                    Logger.Log($"New photo with id {photo.Id} uploaded by {photo.UserId}!");

                    return Ok(response);
                }
                catch (Exception e)
                {
                    Logger.Log("Error uploading image! Detail: " + e.Message + "\r\nStackTrace: " + e.StackTrace);
                    return Ok("Error uploading image!");
                }
            });
        }
    }
}
