using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Orbis.Engine
{
    /// <summary>
    /// Stores all currently pressed keys and enables more checks than the default keyboard state model.
    /// </summary>
    /// <Author>Jannick Zeegers</Author>
    public class InputHandler
    {
        /// <summary>
        /// Currently pressed keys.
        /// </summary>
        private List<Keys> pressedKeys;

        /// <summary>
        /// Keys marked as already triggered.
        /// </summary>
        private List<Keys> triggerOnce;

        /// <summary>
        /// Constructor for Input Handler.
        /// </summary>
        public InputHandler()
        {
            pressedKeys = new List<Keys>();
            triggerOnce = new List<Keys>();
        }

        /// <summary>
        /// Checks if a given key is currently held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if key is held down.</returns>
        public bool IsKeyHeld(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        /// <summary>
        /// Checks if multiple given keys are currently held down.
        /// </summary>
        /// <param name="keys">An array of keys to check.</param>
        /// <returns>Returns true if all keys are held down.</returns>
        public bool IsKeyHeld(Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (!pressedKeys.Contains(key)) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given key is currently not held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if key is not held down.</returns>
        public bool IsKeyUp(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            return state.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if multiple given keys are currently not held down.
        /// </summary>
        /// <param name="keys">An array of keys to check.</param>
        /// <returns>Returns true if all keys are not held down.</returns>
        public bool IsKeyUp(Keys[] keys)
        {
            KeyboardState state = Keyboard.GetState();
            foreach (Keys key in keys)
            {
                if (!state.IsKeyUp(key)) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given key is down once.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is down once.</returns>
        public bool IsKeyDown(Keys key)
        {
            if (pressedKeys.Contains(key) && !triggerOnce.Contains(key)) return true;
            return false;
        }

        /// <summary>
        /// Checks if a given key is down once while a modifier is being held down simultaneously.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="modifier">The modifier key to check.</param>
        /// <returns>Returns true if the modifier is held down and the key is down once.</returns>
        public bool IsKeyDown(Keys key, Keys modifier)
        {
            if (pressedKeys.Contains(modifier) && (pressedKeys.Contains(key) && !triggerOnce.Contains(key))) return true;
            return false;
        }

        /// <summary>
        /// Checks if a given key is down once while multiple modifiers are being held down simultaneously.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="modifiers">An array of modifier keys to check.</param>
        /// <returns>Returns true if the modiefier keys are being held down and the key is down once.</returns>
        public bool IsKeyDown(Keys key, Keys[] modifiers)
        {
            foreach (Keys modifier in modifiers) 
            {
                if (!pressedKeys.Contains(modifier)) return false;
            }
            if (pressedKeys.Contains(key) && !triggerOnce.Contains(key)) return true;
            return false;
        }

        /// <summary>
        /// Checks if a given key is released from being held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is released from being held down.</returns>
        public bool IsKeyReleased(Keys key)
        {
            if (!pressedKeys.Contains(key) && triggerOnce.Contains(key)) return true;
            return false;
        }

        /// <summary>
        /// Updates the current keyboard keys states.
        /// </summary>
        public void UpdateInput()
        {
            KeyboardState state = Keyboard.GetState();
            List<Keys> removeListP = new List<Keys>();
            foreach (Keys key in pressedKeys)
            {
                if (state.IsKeyUp(key))
                {
                    removeListP.Add(key);
                }
            }

            List<Keys> removeListT = new List<Keys>();
            foreach (Keys key in triggerOnce)
            {
                if (state.IsKeyUp(key) && !pressedKeys.Contains(key))
                {
                    removeListT.Add(key);
                }
            }

            foreach (Keys key in removeListP)
            {
                if (pressedKeys.Contains(key)) pressedKeys.Remove(key);
            }

            foreach (Keys key in removeListT)
            {
                if (triggerOnce.Contains(key)) triggerOnce.Remove(key);
            }

            foreach (Keys key in state.GetPressedKeys())
            {
                if (pressedKeys.Contains(key) && !triggerOnce.Contains(key))
                {
                    triggerOnce.Add(key);
                }
                else
                {
                    pressedKeys.Add(key);
                }
            }    
        }

        /// <summary>
        /// Prints a line when a key is pressed or released.
        /// </summary>
        public void PrintKeyLists()
        {
            foreach (Keys key in pressedKeys)
            {
                if (!triggerOnce.Contains(key)) System.Diagnostics.Debug.WriteLine(key + " down");
            }
            foreach (Keys key in triggerOnce)
            {
                if (!pressedKeys.Contains(key)) System.Diagnostics.Debug.WriteLine(key + " up");
            }
        }
    }
}
