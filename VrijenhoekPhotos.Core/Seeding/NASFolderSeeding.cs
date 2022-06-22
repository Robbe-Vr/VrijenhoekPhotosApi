using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Core.Seeding
{
    public static  class NASFolderSeeding
    {
        private static readonly string nasFolder = Path.Combine("F:", "NAS", "Users", "DSPhoto", "Photo");

        private static Func<AlbumsHandler> _albumsHandler;
        private static Func<PhotosHandler> _photosHandler;
        private static Action<int, int> _add;

        private static readonly string _over2GBFiles = Path.Combine("F:", "VrijenhoekPhotos", "Over2GBFiles.txt");

        public static void Seed(Action<int, int> addToAlbum, Func<AlbumsHandler> createAlbumsHandler, Func<PhotosHandler> createPhotosHandler, bool test = false)
        {
            _add = addToAlbum;
            _albumsHandler = createAlbumsHandler;
            _photosHandler = createPhotosHandler;

            EnumerateFolderContent(test ? 
                Path.Combine(nasFolder, "Vakanties", "2004 Duitsland (Sauerland)")
                : nasFolder
            );

            Trace.WriteLine("\n----------\nDone seeding from NAS folder!\n----------\n");
            Console.WriteLine("\n----------\nDone seeding from NAS folder!\n----------\n");
        }

        private static void EnumerateFolderContent(string path, string parentFolderNameCluster = null)
        {
            int? albumId = 0;

            string dirName = Path.GetFileNameWithoutExtension(path);
            string folderNameCluster = parentFolderNameCluster;
            if (folderNameCluster != null) folderNameCluster += (parentFolderNameCluster == string.Empty ? "" : @"\\") + dirName;

            try
            {
                foreach (string filePath in Directory.EnumerateFiles(path))
                {
                    string type = Path.GetExtension(filePath).ToLower();
                    if (FileTypes.AllowedFileTypes.Any(x => x == type))
                    {
                        if (new FileInfo(filePath).Length > 2_000_000_000)
                        {
                            using StreamWriter stream = File.AppendText(_over2GBFiles);
                            stream.WriteLine(filePath);
                            continue;
                        }
                        if (albumId < 1)
                        {
                            AlbumsHandler albumsHandler = _albumsHandler();
                            ResponseResult result = albumsHandler.Create(folderNameCluster ?? "root");
                            albumId = (result.Result as AlbumDTO)?.Id;
                            Trace.WriteLine("New Album created with name: " + folderNameCluster + " and Id: " + albumId);
                            Console.WriteLine("New Album created with name: " + folderNameCluster + " and Id: " + albumId);
                            albumsHandler = null;
                        }

                        TrySeedFile(filePath, type, albumId);
                    }
                }

                foreach (string dirPath in Directory.EnumerateDirectories(path))
                {
                    string name = Path.GetFileNameWithoutExtension(dirPath);
                    if (name == "@eaDir" || name == "#recycle") continue;

                    EnumerateFolderContent(dirPath, folderNameCluster ?? "");
                }
            }
            finally
            {
                albumId = null;
                dirName = null;
                folderNameCluster = null;

                GC.Collect();
            }
        }

        private static void TrySeedFile(string path, string type, int? albumId)
        {
            string photoName = Path.GetFileNameWithoutExtension(path);

            PhotosHandler photosHandler = _photosHandler();
            AlbumsHandler albumsHandler = _albumsHandler();

            byte[] content = File.ReadAllBytes(path);

            try
            {
                ResponseResult result = photosHandler.AddPhoto(new FilePhotoDTO()
                {
                    Content = content,
                    CreationDate = File.GetCreationTimeUtc(path),
                    Name = Path.GetFileNameWithoutExtension(path),
                    IsVideo = FileTypes.AllowedVideoFileTypes.Any(x => x == type),
                    Type = type,
                });

                FilePhotoDTO photo = result.Result as FilePhotoDTO;
                int? photoId = photo.Id;
                Trace.WriteLine($"Photo created and stored with Id: {photoId}!");
                Console.WriteLine($"Photo created and stored with Id: {photoId}!");
                photo = null;

                if (photoId.HasValue && photoId > 0 && albumId.HasValue && albumId > 0)
                {
                    _add(albumId.Value, photoId.Value);
                    //albumsHandler.AddPhoto(albumId.Value, photoId.Value);
                    Trace.WriteLine($"photo with Id: {photoId} added to album with Id: {albumId}!");
                    Console.WriteLine($"photo with Id: {photoId} added to album with Id: {albumId}!");
                }
                else
                {
                    Trace.WriteLine("Id values missing! Not adding photo to album!");
                    Console.WriteLine("Id values missing! Not adding photo to album!");
                }
            }
            finally
            {
                albumsHandler = null;
                photosHandler = null;
                content = null;

                GC.Collect();
            }
        }
    }
}
