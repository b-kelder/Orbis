using Orbis.Engine;

namespace Orbis.States.State
{
    /// <summary>
    /// Author: AukeM
    /// State when the menu is active
    /// </summary>
    class MenuState : IState
    {
        /// <summary>
        /// Run the state
        /// </summary>
        public void Run()
        {
            AudioManager.PlaySong("Severe Tire Damage", true);
        }
    }
}