using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Orbis.Engine
{
    /// <summary>
    /// Author: AukeM
    /// Class based on: https://github.com/CartBlanche/MonoGame-Samples/blob/master/NetRumble/AudioManager.cs
    /// Manage audio: songs/soundeffects
    /// </summary>
    class AudioManager
    {
        // Directory config
        private const string FILE_EXTENSION     = "*.xnb";
        private const string FILE_DIR_ROOT      = "Audio";
        private const string FILE_DIR_SONGS     = FILE_DIR_ROOT + "/Songs";
        private const string FILE_DIR_EFFECT    = FILE_DIR_ROOT + "/Effects";

        // Main Audio config
        private const float DEFAULT_VOLUME      = 0.5F;
        private const bool DEFAULT_REPEATING    = false;

        // Variable Audio config
        private static bool audioEnabled;

        // Libs for songs/effects
        private static Dictionary<string, Song> songLib;
        private static Dictionary<string, SoundEffectInstance> effectLib;

        private static AudioManager audioManager;

        private AudioManager()
        {
            songLib         = new Dictionary<string, Song>();
            effectLib       = new Dictionary<string, SoundEffectInstance>();
            audioEnabled    = true;
        }

        /// <summary>
        /// Init new AudioManager
        /// </summary>
        public static void Initialize()
        {
            if (audioManager == null)
            {
                audioManager = new AudioManager();
            }
        }

        /// <summary>
        /// Load all content
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            LoadSongs(content);
            //LoadEffects(content); // Disabled as we do not have effects.
        }

        /// <summary>
        /// Enable or disable audio
        /// </summary>
        /// <param name="enabled"></param>
        public static void EnableAudio(bool enabled)
        {
            audioEnabled = enabled;
        }

        /// <summary>
        /// Play a song
        /// </summary>
        /// <param name="name">The name of the song</param>
        /// <param name="repeating">Should it repeat</param>
        /// <param name="volume">The volume</param>
        public static void PlaySong(string name, bool repeating = DEFAULT_REPEATING, float volume = DEFAULT_VOLUME)
        {
            // Only play if audio is enabled
            if (!audioEnabled)
            {
                Debug.WriteLine("AUDIO MANAGER: audio not playing as it has been disabled in config.");
                return;
            }

            if (audioManager == null || effectLib == null)
            {
                Debug.WriteLine("No AudioManager or no songs present.");
                return;
            }

            // Make sure the song exists
            if (songLib.ContainsKey(name))
            {
                MediaPlayer.IsRepeating = repeating;
                MediaPlayer.Volume      = volume;
                MediaPlayer.Play(songLib[name]);
            }
            else
            {
                Debug.WriteLine("Song: " + name + " does not exist.");
            }
        }

        /// <summary>
        /// Play an effect
        /// </summary>
        /// <param name="name">The name of the effect</param>
        public static void PlayEffect(string name)
        {
            // Only play if audio is enabled
            if (!audioEnabled)
            {
                Debug.WriteLine("AUDIO MANAGER: audio not playing as it has been disabled in config.");
                return;
            }

            if (audioManager == null || effectLib == null)
            {
                Debug.WriteLine("No AudioManager or no effects present.");
                return;
            }

            // Make sure the effect exists
            if (effectLib.ContainsKey(name))
            {
                effectLib[name].Play();
            }
            else
            {
                Debug.WriteLine("Effect: " + name + " does not exist.");
            }
        }

        /// <summary>
        /// Stop a song
        /// </summary>
        public static void StopSong()
        {
            MediaPlayer.Stop();
        }


        /// <summary>
        /// Pause a song
        /// </summary>
        public static void PauseSong()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resume a song that has been paused
        /// </summary>
        public static void ResumeSong()
        {
            if(MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }
    
        /// <summary>
        /// Load all songs
        /// </summary>
        /// <param name="content"></param>
        private static void LoadSongs(ContentManager content)
        {
            try
            {
                // Fetch files from directory
                FileInfo[] files = new DirectoryInfo(content.RootDirectory + "/" + FILE_DIR_SONGS).GetFiles(FILE_EXTENSION);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName     = Path.GetFileNameWithoutExtension(files[i].Name);      // Fetch name without extension
                    songLib[fileName]   = content.Load<Song>(FILE_DIR_SONGS + "/" + fileName);  // Load from pipeline and push to lib
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Load all effects
        /// </summary>
        /// <param name="content"></param>
        private static void LoadEffects(ContentManager content)
        {
            try
            {
                // Fetch files from directory
                FileInfo[] files = new DirectoryInfo(content.RootDirectory + "/" + FILE_DIR_EFFECT).GetFiles(FILE_EXTENSION);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName         = Path.GetFileNameWithoutExtension(files[i].Name);              // Fetch file without extension
                    SoundEffect soundEffect = content.Load<SoundEffect>(FILE_DIR_EFFECT + "/" + fileName);  // Load from pipeline

                    // Push to lib so it can be used
                    soundEffect.Name = fileName;
                    effectLib[fileName] = soundEffect.CreateInstance();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Gets the enabled state
        /// </summary>
        /// <returns>Returns the enabled state</returns>
        public static bool IsEnabled()
        {
            return audioEnabled;
        }
    }
}
