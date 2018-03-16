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

        /// <summary>
        /// Create a folder in the DocumentsLibrary
        /// </summary>
        /// <returns>Operation success</returns>
        public async Task<bool> PickFolder()
        {
            StorageApplicationPermissions.FutureAccessList.Clear();
            // No need to repick file if file is already picked
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
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop,
                };
                folderPicker.FileTypeFilter.Add("*");

                // Pick the folder
                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(FOLDER_TOKEN, folder);

                    storageFolder = folder;
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
        /// <param name="extension">Extension of the file</param>
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
        /// <param name="file">File to write to</param>
        /// <param name="text">The text to write</param>
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
        /// Write to the current known file
        /// </summary>
        /// <param name="text">The text to write</param>
        public async void WriteToCurrentFile(string text)
        {
            if (currentfile != null)
            {
                await WriteToFile(currentfile, text);
            }
        }
    }
}
