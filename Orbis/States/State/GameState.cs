using Orbis.Engine;
using System;

namespace Orbis.States.State
{
    /// <summary>
    /// Author: AukeM
    /// State when the actual game is active
    /// </summary>
    class GameState : IState
    {
        private Random rand;

        public GameState()
        {
            rand = new Random();
        }

        /// <summary>
        /// Run the state
        /// </summary>
        public void Run()
        {
            PlayRandomSong();
        }

        /// <summary>
        /// Get a random song
        /// </summary>
        private void PlayRandomSong()
        {
            int songRand = rand.Next(0, 2);
            if (songRand == 0)
            {
                AudioManager.PlaySong("Crossing the Chasm", true);
            }
            else
            {
                AudioManager.PlaySong("Rocket", true);
            }
        }
    }
}
