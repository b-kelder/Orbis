using Orbis.Engine;

namespace Orbis.States.State
{
    class GameState : IState
    {
        public void Run()
        {
            AudioManager.PlaySong("Crossing the Chasm");
        }
    }
}
