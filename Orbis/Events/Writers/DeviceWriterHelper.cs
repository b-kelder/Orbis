using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace Orbis.Events.Writers
{
    abstract class DeviceWriterHelper
    {
        private const string ORBIS_FOLDER = "Orbis Simulation Logs";

        private StorageFolder storageFolder;
        private StorageFile currentfile;

        /// <summary>
        /// Create a folder in the DocumentsLibrary
        /// </summary>
        /// <param name="folder">The name of the folder</param>
        public async Task<bool> PickFolder()
        {
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
                    Windows.Storage.AccessCache.StorageApplicationPermissions.
                    FutureAccessList.AddOrReplace("PickedFolderToken", folder);

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
        /// Create a file in the DocumentsLibrary
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="extension">The extension of the file</param>
        public async Task<bool> CreateFile(string name, string extension = "txt")
        {
            try
            {
                string fileName = name + "." + extension;
                currentfile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Write to a file
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="text">The text to write</param>
        public async void WriteToFile(StorageFile file, string text)
        {
            try
            {
                await FileIO.WriteTextAsync(file, text);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Write to the current known file
        /// </summary>
        /// <param name="text"></param>
        public void WriteToCurrentFile(string text)
        {
            WriteToFile(currentfile, text);
        }
    }
}
