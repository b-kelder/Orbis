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
    /// Class based on: https://github.com/CartBlanche/MonoGame-Samples/blob/master/NetRumble/AudioManager.cs
    /// </summary>
    class AudioManager
    {
        // Directory config
        private const string FILE_EXTENSION     = "*.xnb";
        private const string FILE_DIR_ROOT      = "Audio";
        private const string FILE_DIR_SONGS     = FILE_DIR_ROOT + "/Songs";
        private const string FILE_DIR_EFFECT    = FILE_DIR_ROOT + "/Effects";

        // Audio config
        private const float DEFAULT_VOLUME      = 1;
        private const bool DEFAULT_REPEATING    = false;

        // Libs for songs/effects
        private static Dictionary<string, Song> songLib;
        private static Dictionary<string, SoundEffectInstance> effectLib;

        private static AudioManager audioManager;


        private AudioManager()
        {
            songLib     = new Dictionary<string, Song>();
            effectLib   = new Dictionary<string, SoundEffectInstance>();
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
            LoadEffects(content);
        }

        /// <summary>
        /// Load all songs
        /// </summary>
        /// <param name="content"></param>
        private static void LoadSongs(ContentManager content)
        {
            try
            {
                FileInfo[] files = new DirectoryInfo(content.RootDirectory + "/" + FILE_DIR_SONGS).GetFiles(FILE_EXTENSION);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName     = Path.GetFileNameWithoutExtension(files[i].Name);
                    songLib[fileName]   = content.Load<Song>(FILE_DIR_SONGS + "/" + fileName);
                }
            }
            catch(Exception ex)
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
                FileInfo[] files = new DirectoryInfo(content.RootDirectory + "/" + FILE_DIR_EFFECT).GetFiles(FILE_EXTENSION);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName         = Path.GetFileNameWithoutExtension(files[i].Name);
                    SoundEffect soundEffect = content.Load<SoundEffect>(FILE_DIR_EFFECT + "/" + fileName);

                    soundEffect.Name        = fileName;
                    effectLib[fileName]     = soundEffect.CreateInstance();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        /// <summary>
        /// Play a song
        /// </summary>
        /// <param name="name">The name of the song</param>
        /// <param name="repeating">Should it repeat</param>
        /// <param name="volume">The volume</param>
        public static void PlaySong(string name, bool repeating = DEFAULT_REPEATING, float volume = DEFAULT_VOLUME)
        {
            if (audioManager == null || effectLib == null)
            {
                Debug.WriteLine("No AudioManager or no songs present.");
                return;
            }

            // Make sure the song exists
            if (songLib.ContainsKey(name))
            {
                MediaPlayer.IsRepeating = repeating;
                MediaPlayer.Volume = volume;
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
    }
}
