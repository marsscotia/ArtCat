using ArtCat.Models;
using MarcelloDB;
using MarcelloDB.Collections;
using MarcelloDB.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ArtCat.Services
{

    /// <summary>
    ///
    /// Class uses MarcelloDB to persist user data.
    ///
    /// MarcelloDB is embedded noSQL document db for UWP.
    /// 
    /// </summary>
    public static class DataService
    {
        //Represents the db session.  
        private static Session _session;
        //Represents the location where the data is stored
        private static readonly StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        //The name of the main collection file
        private static readonly string collectionFileName = "art.data";
        //The name of the journal file
        private static readonly string journalFileName = "journal";
        //The name of the main collection
        private static readonly string collectionName = "pieces";
        //The collection for the art pieces
        private static Collection<Piece, Guid> _piecesCollection;
        //The base path for image files
        public static readonly string BaseImagePath = "ms-appdata:///local/";
        //The extensions which define image file types
        private static readonly string[] imageExtensions = {".jpg", ".jpeg", ".bmp", ".png", ".tiff"};
        //A flag for destroying the database on initialisation: set to true to wipe the existing database on initialisation
        private static readonly bool _destroyDB = false;

        /// <summary>
        /// Connects to the database
        /// </summary>
        public static async void ConnectToDataSource()
        {
            Debug.WriteLine("DataService.cs: Attempting to connect to database.");
            if (_destroyDB)
            {
                Debug.WriteLine("DataService.cs: Destroy Database flag set; destroying existing database.");
                await _destroyDatabase();
            }
            IPlatform platform = new MarcelloDB.uwp.Platform();
            _session = new Session(platform, localFolder.Path);
            var file = _session[collectionFileName];
            try
            {
                _piecesCollection = file.Collection<Piece, Guid>(collectionName, piece => piece.Id);
                Debug.WriteLine("DataService.cs: Connected to database.");
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine("DataService.cs: " + e.Message);

            }
            
        }

        /// <summary>
        /// Disconnects from the database.  Most useful
        /// when app is suspending or being terminated.
        /// </summary>
        public static void DisconnectFromDataSource()
        {
            if (_session != null)
            {
                _session.Dispose();
            }
        }

        /// <summary>
        /// Fetches all the pieces in the database.
        /// </summary>
        /// <returns>A list of all of the pieces in the database, or an empty list if there are none.</returns>
        public static List<Piece> GetPieces()
        {
            List<Piece> results = new List<Piece>();
            if (_piecesCollection != null)
            {
                results.AddRange(_piecesCollection.All);
            }
            return results;
        }

        /// <summary>
        /// Finds pieces in the database whose names contain the search term
        /// passed as a parameter. Ignores case when searching.
        /// </summary>
        /// <param name="aSearchTerm">A search term to find pieces</param>
        /// <returns>A list of pieces whose names contain the search term, or an empty list if there is none</returns>
        public static List<Piece> FindPieces(string aSearchTerm)
        {
            List<Piece> results = new List<Piece>();
            if (_piecesCollection != null)
            {
                foreach (var piece in _piecesCollection.All)
                {
                    if (piece.Name.Contains(aSearchTerm, StringComparison.InvariantCultureIgnoreCase))
                    {
                        results.Add(piece);
                    }
                } 
            }
            return results;
        }

        public static List<Piece> FindSoldPieces(DateTimeOffset? aStartDate, DateTimeOffset? anEndDate)
        {
            List<Piece> results = new List<Piece>();
            if (_piecesCollection != null)
            {
                foreach (var piece in _piecesCollection.All)
                {
                    if (piece.DateSold != null)
                    {
                        if (piece.DateSold >= aStartDate && piece.DateSold <= anEndDate)
                        {
                            results.Add(piece);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Retrieves a set of unique names of pieces.
        /// </summary>
        /// <returns>A set of the names of the pieces, or an empty set if there is none.</returns>
        public static HashSet<string> GetNames()
        {
            HashSet<string> results = new HashSet<string>();
            if (_piecesCollection != null)
            {
                foreach (var piece in _piecesCollection.All)
                {
                    if (piece.Name != null)
                    {
                        results.Add(piece.Name); 
                    }
                } 
            }
            return results;
        }

        public static HashSet<string> GetMedia()
        {
            HashSet<string> results = new HashSet<string>();
            if (_piecesCollection != null)
            {
                foreach (var piece in _piecesCollection.All)
                {
                    if (piece.Medium != null)
                    {
                        results.Add(piece.Medium); 
                    }
                }
            }
            return results;
        }

        public static void PersistPiece(Piece aPiece)
        {
            if (_piecesCollection != null)
            {
                _piecesCollection.Persist(aPiece);
            }
        }

        public static void DestroyPiece(Piece aPiece)
        {
            if (_piecesCollection != null)
            {
                _piecesCollection.Destroy(aPiece.Id);
            }
        }

        public static async Task SaveImageFile(IStorageItem file, string name)
        {
            Windows.Storage.StorageFolder storage = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile storageFile = file as StorageFile;
            await storageFile.CopyAsync(storage, name, NameCollisionOption.ReplaceExisting);
            
        }

        public static async Task DeleteImageFile(string aPath)
        {
            string fileName = GetFileNameFromPath(aPath);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    StorageFolder storage = ApplicationData.Current.LocalFolder;
                    StorageFile file = await storage.GetFileAsync(fileName);
                    await file.DeleteAsync();
                    Debug.WriteLine("DataService.cs: Deleted image file");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("DataService.cs: Error while deleting file: " + e.Message);
                }
            }

        }

        /// <summary>
        /// Method determines whether a given file name represents
        /// an image file, according to the file extension.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>True if the file is a file with an image extension, false otherwise</returns>
        public static bool IsImageFile(string name)
        {
            bool result = false;
            int dotAt = 0;
            if (!string.IsNullOrWhiteSpace(name))
            {
                dotAt = name.LastIndexOf(".");
                if (dotAt > 0)
                {
                    string extension = name.Substring(dotAt);
                    result = imageExtensions.Contains(extension, StringComparer.CurrentCultureIgnoreCase);
                }
            }
            return result;
        }

        private static async Task _destroyDatabase()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile dbFile = await storageFolder.GetFileAsync(collectionFileName);
                await dbFile.DeleteAsync();
                Debug.WriteLine("DataService.cs: Database file destroyed.");
            }
            catch (FileNotFoundException)
            {
                
            }

            try
            {
                StorageFile journalFile = await storageFolder.GetFileAsync(journalFileName);
                await journalFile.DeleteAsync();
                Debug.WriteLine("DataService.cs: Database jounral file destroyed.");
            }
            catch (FileNotFoundException)
            {
                
            }
            
        }

        /// <summary>
        /// Method returns the file name from an msappx:// URI
        /// </summary>
        /// <param name="aPath"></param>
        /// <returns>the file name, or an empty string if the parameter is not in the expected format</returns>
        public static string GetFileNameFromPath(string aPath)
        {
            string result = "";
            Uri uri = new Uri(aPath);
            string[] segments = uri.Segments;
            string fileName = segments[segments.Length - 1];
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                result = fileName;
            }
            return result;
        }

        /// <summary>
        /// Removes images from the app folder which are no longer
        /// being used to support thumbnails for pieces.
        ///
        /// This behaviour is encapsulated into this method as when images are removed in the UI,
        /// there is no way to tell exactly when the file resource is released.  
        /// </summary>
        /// <returns>a Task object when the method has completed</returns>
        public static async Task ClearUnusedImages()
        {
            //we find all of the image files in the local folder
            StorageFolder storage = ApplicationData.Current.LocalFolder;
            var files = await storage.GetFilesAsync();
            var imageFiles = from file in files
                             where IsValidImageFileName(file.Name)
                             select file;
            List<StorageFile> keepFiles = new List<StorageFile>();

            //we loop through each piece
            if (_piecesCollection != null)
            {
                foreach (var piece in _piecesCollection.All)
                {
                    //we find out which files are still being used
                    if (piece.Images != null)
                    {
                        foreach (var number in piece.Images)
                        {
                            StorageFile matchingFile = files.FirstOrDefault(file => file.Name.Equals(piece.Id + "_" + number));
                            if (matchingFile != null)
                            {
                                keepFiles.Add(matchingFile);
                            }
                        } 
                    }
                }

                //then we determine whether there are any files we don't want to keep
                var removalFiles = imageFiles.Except(keepFiles);

                //if there are any files to be removed, we remove them
                if (removalFiles.Count() > 0)
                {
                    Debug.WriteLine(string.Format("DataService.cs: Removing {0} unnecessary files", removalFiles.Count()));
                    foreach (var file in removalFiles)
                    {
                        try
                        {
                            await file.DeleteAsync();
                        }
                        catch (Exception)
                        {
                            //TODO: Add files which threw exception to list to deal with another time 
                        }
                    }
                }
            }

            
        }

        private static bool IsValidImageFileName(string filename)
        {
            bool result = false;

            string[] components = filename.Split("_");
            if (components.Length == 2)
            {
                bool containsGuid = Guid.TryParse(components[0], out Guid g);
                bool containsInt = int.TryParse(components[1], out int i);
                result = containsGuid && containsInt;
            }
                    
            return result;
        }

    }
}
