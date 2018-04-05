using Orbis.Engine;
using System;

namespace Orbis.States.State
{
    class GameState : IState
    {
        private Random rand;

        public GameState()
        {
            rand = new Random();
        }

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
                AudioManager.PlaySong("Crossing the Chasm");
            }
            else
            {
                AudioManager.PlaySong("Rocket");
            }
        }
    }
}
