using System.Collections.Generic;
using Orbis.States.State;

namespace Orbis.States
{
    class StateManager
    {
        private Dictionary<State, IState> states;
        private IState activeState;
        private static StateManager stateManager;
        private bool stateChanged;

        public enum State { MENU, GAME }

        private StateManager()
        {
            states = new Dictionary<State, IState>();
            SetDefaultStates();
        }

        /// <summary>
        /// Get an instance of the StateManager
        /// </summary>
        /// <returns></returns>
        public static StateManager GetInstance()
        {
            if (stateManager == null)
            {
                stateManager = new StateManager();
            }
            return stateManager;
        }

        /// <summary>
        /// Add a new state
        /// </summary>
        /// <param name="key">The key to identify the state</param>
        /// <param name="state">The state object</param>
        public void AddState(State key, IState state)
        {
            if (!states.ContainsValue(state) && !states.ContainsKey(key))
            {
                states.Add(key, state);
            }
        }

        /// <summary>
        /// Set an state active
        /// </summary>
        /// <param name="key">The key to identify the state</param>
        public void SetActiveState(State key)
        {
            if (states.ContainsKey(key))
            {
                activeState     = states[key];
                stateChanged    = true;
            }
        }

        /// <summary>
        /// Run the current active state
        /// </summary>
        public void RunState()
        {
            if (activeState != null)
            {
                activeState.Run();
            }
        }

        /// <summary>
        /// Check if a state has changed
        /// </summary>
        /// <returns>Has state changed</returns>
        public bool IsStateChanged()
        {
            // Check if state has changed, and reset if it did
            bool state = stateChanged;
            if (state)
            {
                stateChanged = false;   // Reset stateChanged
            }
            return state;
        }

        /// <summary>
        /// Set all the default game states
        /// </summary>
        private void SetDefaultStates()
        {
            states.Add(State.MENU, new MenuState());
            states.Add(State.GAME, new GameState());

            // Set the default active state
            SetActiveState(State.MENU);
        }
    }
}
