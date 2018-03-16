using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Orbis.Events.Writers
{
    abstract class DeviceWriterHelper
    {
        private const string FOLDER_TOKEN = "GeneralFolderToken";

        private StorageFolder storageFolder;
        private StorageFile currentfile;
        private static bool folderPickerActive = false;

        /// <summary>
        /// Create a folder in the DocumentsLibrary
        /// </summary>
        /// <returns>Operation success</returns>
        public async Task<bool> PickFolder()
        {
            // Prevent the folder picker from being called multiple times
            if (folderPickerActive)
            {
                // Keep busy and wait till folder picker becomes available
                while (folderPickerActive)
                {
                    await Task.Delay(1000);
                }
            }
            folderPickerActive = true;

            // Current folder cache. No need to repick folder if in cache
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(FOLDER_TOKEN))
            {
                storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FOLDER_TOKEN);
                return true;
            }

            try
            {
                // Create a Picker
                var folderPicker = new Windows.Storage.Pickers.FolderPicker()
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                };
                folderPicker.FileTypeFilter.Add("*");

                // Pick the folder and make sure one returned
                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(FOLDER_TOKEN, folder);

                    storageFolder       = folder;
                    folderPickerActive  = false;
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Create a file in the picked folder
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="extension">The extension of the file</param>
        /// <param name="option">Collision option</param>
        /// <returns>Operation success</returns>
        public async Task<StorageFile> CreateFile(string name, string extension = "txt", CreationCollisionOption option = CreationCollisionOption.OpenIfExists)
        {
            try
            {
                string fileName = name + "." + extension;
                currentfile = await storageFolder.CreateFileAsync(fileName, option);

                return currentfile;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// Write to file
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="text">The text to write</param>
        /// <returns>Operation success</returns>
        public async Task<bool> WriteToFile(StorageFile file, string text)
        {
            try
            {
                await FileIO.AppendTextAsync(file, text);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Write to current cached file
        /// </summary>
        /// <param name="text">The text to write</param>
        public async void WriteToCurrentFile(string text)
        {
            if (currentfile != null)
            {
                await WriteToFile(currentfile, text);
            }
        }

        /// <summary>
        /// Clear the current folder cash (folder needs to be re-picked)
        /// </summary>
        public void ClearCurrentFolderCache()
        {
            StorageApplicationPermissions.FutureAccessList.Clear();
        }
    }
}
