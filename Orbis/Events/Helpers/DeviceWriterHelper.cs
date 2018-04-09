using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;

namespace Orbis.Events.Helpers
{
    /// <summary>
    /// Author: AukeM
    /// Helper class with functionality to write to user system
    /// </summary>
    abstract class DeviceWriterHelper
    {
        private const string FOLDER_TOKEN = "GeneralFolderToken";           // Token which identifies the folder location (used for cache)

        private StorageFolder storageFolder;                                // The folder to store data in (the picked folder)
        private StorageFile currentfile;                                    // The current file loaded in system and to write to
        private static SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);   // Write lock
        private static SemaphoreSlim pickerLock = new SemaphoreSlim(1, 1);  // Folderpicker lock

        /// <summary>
        /// Set the folder to the package local folder
        /// </summary>
        public void SetLocalFolder()
        {
            storageFolder = ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// Open explorer window at current specified storageFolder
        /// </summary>
        public async void OpenStorageFolder()
        {
            // If no storage folderi s specified, do not continue
            if (!IsStorageFolderConfiguered())
            {
                return;
            }
            await Launcher.LaunchFolderAsync(storageFolder);
        }

        /// <summary>
        /// Open explorer window at current specified storageFolder or fallback at default folder
        /// </summary>
        public async void OpenStorageFolderOrDefault()
        {
            // If no storage folder is configuered, set default and continue
            if (!IsStorageFolderConfiguered())
            {
                SetLocalFolder();
            }
            await Launcher.LaunchFolderAsync(storageFolder);
        }

        /// <summary>
        /// Check if a storage folder has been configuered
        /// </summary>
        /// <returns>True on configuered, false on not configuered</returns>
        public bool IsStorageFolderConfiguered()
        {
            return storageFolder != null;
        }

        /// <summary>
        /// Create a picker with suggested location in documents folder
        /// </summary>
        /// <returns>Operation success</returns>
        public async Task<bool> PickFolder()
        {
            // Wait wile lock is active
            await pickerLock.WaitAsync();

            // Make sure the lock is always released
            try
            {
                // Current folder cache. No need to repick folder if in cache
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(FOLDER_TOKEN))
                {
                    storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FOLDER_TOKEN);
                    return true;    // No need to pick the folder, as it is in cache
                }

                try
                {
                    // Create a Picker with specified config
                    FolderPicker folderPicker = new FolderPicker()
                    {
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                        ViewMode = PickerViewMode.Thumbnail,
                        FileTypeFilter = { ".txt", ".xml", ".json" },
                    };
                
                    // Pick the folder and make sure one returned
                    StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        // Store picked folder in cache so it does not need to be picked again.
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace(FOLDER_TOKEN, folder);

                        storageFolder = folder;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Folder picking error: " + ex);
                }
                return false;
            }
            finally
            {
                // Make sure lock is released
                pickerLock.Release();
            }
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
            // If no folder is selected, set local folder
            if (!IsStorageFolderConfiguered())
            {
                SetLocalFolder();
            }

            // Try to create a file
            try
            {
                string fileName = name + "." + extension;
                currentfile = await storageFolder.CreateFileAsync(fileName, option);

                return currentfile;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Create File error: " + ex);
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
            // Wait wile lock is active
            await writeLock.WaitAsync();

            // If no folder is selected, set local folder
            if (!IsStorageFolderConfiguered())
            {
                SetLocalFolder();
            }

            // Try to write to file
            try
            {
                await FileIO.AppendTextAsync(file, text);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Write to file error: " + ex);
            }
            finally
            {
                // Make sure lock is released
                writeLock.Release();
            }
            return false;
        }

        /// <summary>
        /// Write to current cached file
        /// </summary>
        /// <param name="text">The text to write</param>
        public async void WriteToCurrentFile(string text)
        {
            // Only write to current file if it exists
            if (currentfile != null)
            {
                await WriteToFile(currentfile, text);
            }
        }

        /// <summary>
        /// Clear the current folder cache (folder needs to be re-picked)
        /// </summary>
        public void ClearCurrentFolderCache()
        {
            StorageApplicationPermissions.FutureAccessList.Clear();
        }
    }
}
