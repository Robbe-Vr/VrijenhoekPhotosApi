using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Exchange.FactoryInterfaces;
using System.Diagnostics;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Extensions;
using SYWCentralLogging;
using SkiaSharp;
using VrijenhoekPhotos.Exchange;

namespace VrijenhoekPhotos.FileSystem
{
    public class FileSystemPhotoDAL : IFilePhotosDAL
    {
        public FileSystemPhotoDAL()
        {
            if (ApplicationEnvironmentData.InDevelopment)
            {
                FFmpeg.SetExecutablesPath(Path.Combine(ApplicationEnvironmentData.InDevelopment ? Directory.GetCurrentDirectory() : "/app", "ffmpeg", "bin"));
            }
            else
            {
                
            }
        }

        public bool TryGetPhoto(ref FilePhotoDTO photo)
        {
            try
            {
                string name = photo.Name;
                string type = photo.Type;
                FolderClass folder = FileSystemReader.ReadPicturesFileSystemFolder(photo.UserId);

                FileClass file = folder.Files.FirstOrDefault(x => x.Name == name && x.Extension == type);

                if (file == null) return false;

                photo.ContentLocation = file.Location;
            }
            catch (Exception e)
            {
                Logger.Log("Error getting photo! Detail: " + e.Message);
                return false;
            }

            return true;
        }

        public bool TryGetPhotoAsStream(ref FilePhotoDTO photo)
        {
            try
            {
                string name = photo.Name;
                string type = photo.Type;
                FolderClass folder = FileSystemReader.ReadPicturesFileSystemFolder(photo.UserId);

                FileClass file = folder.Files.FirstOrDefault(x => x.Name == name && x.Extension == type);

                if (file == null) return false;

                photo.Stream = File.OpenRead(file.Location);
            }
            catch (Exception e)
            {
                Logger.Log("Error getting photo! Detail: " + e.Message);
                return false;
            }

            return true;
        }

        public List<FilePhotoDTO> GetPhotos(UserDTO user)
        {
            FolderClass folder = FileSystemReader.ReadPicturesFileSystemFolder(user.Id);

            List<FilePhotoDTO> photos = new List<FilePhotoDTO>();
            foreach (FileClass file in folder.Files)
            {
                photos.Add(new FilePhotoDTO()
                {
                    CreationDate = file.CreationDate,
                    Name = file.Name,
                    User = user,
                    UserId = user.Id,
                    Type = file.Extension,
                    ContentLocation = file.Location,
                });
            }
            return photos;
        }

        public bool TryCreateThumbnail(ref FilePhotoDTO photo, int height = 256, int width = 256)
        {
            try
            {
                if (photo.Content?.Length > 0 || TryGetPhoto(ref photo))
                {
                    string name = photo.Name;
                    FileClass file = FileSystemReader.ReadFileFromPicturesFileSystemFolder(photo.UserId, name + photo.Type);
                    string thumbnailPath = Path.Combine(FileSystemReader.PicturesBaseFolder, photo.UserId, "Thumbnails");
                    string thumbnailFileName = photo.Id + "_thumb";
                    const string thumbnailFileType = ".jpg";

                    if (file == null) return false;

                    string fileLocation = file.Location;
                    if (photo.IsVideo)
                    {
                        string thumbnailOutputFilePath = Path.Combine(thumbnailPath, thumbnailFileName + thumbnailFileType);

                        Task task = FFMpegConvert.GetThumbnailFromVideo(file.Location, thumbnailOutputFilePath);
                        task.Wait();

                        if (!File.Exists(thumbnailOutputFilePath)) return false;
                            
                        fileLocation = thumbnailOutputFilePath;
                    }

                    using (SKBitmap bitmap = SKBitmap.Decode(fileLocation))
                    using (SKBitmap thumbnailBitmap = bitmap.Resize(new SKSizeI(width, height), SKFilterQuality.Medium))
                    {
                        if (photo.IsVideo)
                        {
                            using (SKCanvas canvas = new SKCanvas(thumbnailBitmap))
                            {
                                double transparency = 0.3;

                                SKPaint paint = new SKPaint()
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = SKColors.White.WithAlpha((byte)(0xFF * (1 - transparency))),
                                    IsAntialias = true,
                                    StrokeCap = SKStrokeCap.Round,
                                };

                                SKPoint[] points = new SKPoint[]
                                {
                                    new SKPoint((int)Math.Round(width * 0.4), (int)Math.Round(height * 0.4)),
                                    new SKPoint((int)Math.Round(width * 0.6), (int)Math.Round(height * 0.5)),

                                    new SKPoint((int)Math.Round(width * 0.6), (int)Math.Round(height * 0.5)),
                                    new SKPoint((int)Math.Round(width * 0.4), (int)Math.Round(height * 0.6)),

                                    new SKPoint((int)Math.Round(width * 0.4), (int)Math.Round(height * 0.6)),
                                    new SKPoint((int)Math.Round(width * 0.4), (int)Math.Round(height * 0.4)),
                                };

                                SKPath path = new SKPath
                                {

                                };

                                path.MoveTo(points[0]);
                                foreach (SKPoint point in points.Skip(1))
                                    path.LineTo(point);

                                canvas.DrawPath(path, paint);

                                paint.Style = SKPaintStyle.Stroke;
                                paint.Color = SKColors.Black.WithAlpha((byte)(0xFF * (1 - transparency)));
                                paint.StrokeWidth = 5;

                                canvas.DrawPoints(SKPointMode.Lines, points, paint);

                                canvas.Flush();
                            }
                        }

                        using (SKImage img = SKImage.FromBitmap(thumbnailBitmap))
                        using (SKData imgData = img.Encode(SKEncodedImageFormat.Jpeg, 75))
                        {
                            photo.Thumbnail = imgData.ToArray();

                            photo.Content = photo.Thumbnail;
                            string type = photo.Type;
                            photo.Type = thumbnailFileType;
                            TryStorePhoto(ref photo, thumbnailPath, thumbnailFileName, DoubleFileHandling.Overwrite);
                            photo.Type = type;
                        }
                    }
                }
                else throw new ArgumentException("Unable to get content for given photo!");
            }
            catch (Exception e)
            {
                Logger.Log("Error creating thumbnail! Detail: " + e.Message);
                return false;
            }

            return true;
        }

        public bool TryGetThumbnail(ref FilePhotoDTO photo)
        {
            try
            {
                string name = photo.Id.ToString();
                FileClass file = FileSystemReader.ReadFileFromPicturesFileSystemFolder(Path.Combine(photo.UserId, "Thumbnails"), name + "_thumb.jpg");

                if (file == null) return false;

                photo.ThumbnailLocation = file.Location;
            }
            catch (Exception e)
            {
                Logger.Log("Error getting thumbnail! Detail: " + e.Message);
                return false;
            }

            return true;
        }

        private string IncrementFileName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            string dirPath = Path.GetDirectoryName(path);

            string incrementedFileNameTemplate = fileName + " ({0})";

            int incrementIndex = 1;
            string incrementedFileName = fileName;
            
            while (File.Exists(Path.Combine(dirPath, incrementedFileName + ext)))
            {
                incrementIndex++;
                incrementedFileName = String.Format(incrementedFileNameTemplate, incrementIndex);
            }

            return incrementedFileName;
        }

        public bool TryStorePhoto(ref FilePhotoDTO photo, string userFolder, string name = null, DoubleFileHandling fileHandling = DoubleFileHandling.Error)
        {
            try
            {
                string folder = Path.Combine(FileSystemData.picturesFolderPath, userFolder);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, $"{name ?? photo.Name}{photo.Type}");

                if (File.Exists(path))
                {
                    switch (fileHandling)
                    {
                        case DoubleFileHandling.Error:
                            throw new ArgumentException("File name already exists in this folder!");

                        case DoubleFileHandling.Overwrite:
                            break;

                        case DoubleFileHandling.Increment:
                            photo.Name = IncrementFileName(path);
                            path = Path.Combine(folder, $"{name ?? photo.Name}{photo.Type}");
                            break;
                    }
                }
                
                File.WriteAllBytes(path, photo.Content);

                if (!userFolder.EndsWith("Thumbnails") && photo.IsVideo && photo.Type != ".mp4")
                {
                    string fileName = name ?? photo.Name;
                    string type = photo.Type;
                    Task.Run(() =>
                        FFMpegConvert.ConvertToMp4(path, Path.Combine(folder, $"{fileName}.mp4"), type)
                    );
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error saving photo! Detail: " + e.Message);
                return false;
            }

            return true;
        }

        public bool Update(PhotoDTO photo, string oldName)
        {
            if (string.IsNullOrEmpty(photo.Name)) return false;

            FileClass file = FileSystemReader.ReadFileFromPicturesFileSystemFolder(photo.UserId, oldName + photo.Type);
            if (file != null)
            {
                File.Move(file.Location, Path.Combine(FileSystemReader.PicturesBaseFolder, photo.UserId, photo.Name + photo.Type));
            }
            else return false;

            return true;
        }

        public bool Remove(PhotoDTO photo)
        {
            FileClass file = FileSystemReader.ReadFileFromPicturesFileSystemFolder(photo.UserId, photo.Name + photo.Type);
            if (file != null)
            {
                File.Delete(file.Location);
            }
            else return false;

            if (photo.IsVideo && photo.Type != ".mp4")
            {
                FileClass convertedFile = FileSystemReader.ReadFileFromPicturesFileSystemFolder(photo.UserId, photo.Name + ".mp4");
                if (convertedFile != null)
                {
                    File.Delete(convertedFile.Location);
                }
                else return false;
            }

            FileClass thumbnailFile = FileSystemReader.ReadFileFromPicturesFileSystemFolder(Path.Combine(photo.UserId, "Thumbnails"), photo.Id + "_thumb.jpg");
            if (thumbnailFile != null)
            {
                bool deleted = false;
                int attempts = 0;
                while (!deleted || attempts > 3)
                {
                    try
                    {
                        File.Delete(thumbnailFile.Location);

                        deleted = true;
                    }
                    catch (IOException e)
                    {
                        attempts++;
                        Logger.Log("Failed to delete file: " + e.Message);
                    }
                }
            }

            return true;
        }
    }
}
