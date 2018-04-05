using Orbis.Engine;

namespace Orbis.States.State
{
    class MenuState : IState
    {
        public void Run()
        {
            AudioManager.PlaySong("Severe Tire Damage", true, 1);
        }
    }
}
