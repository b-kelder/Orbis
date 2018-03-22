using System.Collections.Generic;
using Orbis.States.State;

namespace Orbis.States
{
    class StateManager
    {
        private Dictionary<string, IState> states;
        private IState activeState;
        private static StateManager stateManager;
        private bool stateChanged;

        private StateManager()
        {
            states = new Dictionary<string, IState>();
            activeState = new GameState();
            SetDefaultStates();
        }

        public static StateManager GetInstance()
        {
            if (stateManager == null)
            {
                stateManager = new StateManager();
            }
            return stateManager;
        }

        public void AddState(string key, IState state)
        {
            if (!states.ContainsValue(state) && !states.ContainsKey(key))
            {
                states.Add(key, state);
            }
        }

        public void SetActiveState(string key)
        {
            if (states.ContainsKey(key))
            {
                activeState     = states[key];
                stateChanged    = true;
            }
        }

        public void RunState()
        {
            if (activeState != null)
            {
                activeState.Run();
            }
        }

        public bool IsStateChanged()
        {
            bool state = stateChanged;
            if (state)
            {
                stateChanged = false;
            }
            return state;
        }

        /// <summary>
        /// Set all the default game states
        /// </summary>
        private void SetDefaultStates()
        {
            states.Add("game", new GameState());
            stateChanged = true;
        }
    }
}
