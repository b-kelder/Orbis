using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Orbis.Events.Writers
{
    abstract class DeviceWriterHelper
    {
        private StorageFolder storageFolder;
        private StorageFile currentfile;

        /// <summary>
        /// Create a folder in the DocumentsLibrary
        /// </summary>
        /// <returns>Operation success</returns>
        public async Task<bool> PickFolder()
        {
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFolderToken"))
            {
                storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
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
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

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
        public async Task<StorageFile> CreateFile(string name, string extension = "txt")
        {
            try
            {
                string fileName = name + "." + extension;
                currentfile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

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
        public async void WriteToFile(StorageFile file, string text)
        {
            try
            {
                await FileIO.AppendTextAsync(file, text);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Write to the current known file
        /// </summary>
        /// <param name="text">The text to write</param>
        public void WriteToCurrentFile(string text)
        {
            if (currentfile != null)
            {
                WriteToFile(currentfile, text);
            }
        }
    }
}
