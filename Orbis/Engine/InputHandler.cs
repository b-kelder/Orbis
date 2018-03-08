using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Orbis.Engine
{
    /// <summary>
    /// Stores all currently pressed keys. Suppors keypresses.
    /// </summary>
    /// <Author>Jannick Zeegers</Author>
    public class InputHandler
    {
        /// <summary>
        /// Currently pressed keys.
        /// </summary>
        public List<Keys> pressedKeys;

        /// <summary>
        /// Keys marked as already triggered.
        /// </summary>
        public List<Keys> triggerOnce;

        /// <summary>
        /// Constructor for Input Handler.
        /// </summary>
        public InputHandler()
        {
            pressedKeys = new List<Keys>();
            triggerOnce = new List<Keys>();
        }

        public bool IsKeyDown(Keys key)
        {
            if (pressedKeys.Contains(key)) return true;
            return false;
        }

        public bool IsKeyDown(Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (!pressedKeys.Contains(key)) return false;
            }
            return true;
        }

        public bool IsKeyUp(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            return state.IsKeyUp(key);
        }

        public bool IsKeyUp(Keys[] keys)
        {
            KeyboardState state = Keyboard.GetState();
            foreach (Keys key in keys)
            {
                if (!state.IsKeyUp(key)) return false;
            }
            return true;
        }

        public bool IsKeyPressed(Keys key)
        {
            if (pressedKeys.Contains(key) && !triggerOnce.Contains(key)) return true;
            return false;
        }

        public bool IsKeyPressed(Keys[] keys)
        {
            foreach(Keys key in keys)
            {
                if (!pressedKeys.Contains(key) || triggerOnce.Contains(key)) return false;
            }
            return true;
        }

        public bool IsKeyReleased(Keys key)
        {
            if (!pressedKeys.Contains(key) && triggerOnce.Contains(key)) return true;
            return false;
        }

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

            foreach(Keys key in removeListP)
            {
                if (pressedKeys.Contains(key)) pressedKeys.Remove(key);
            }

            foreach(Keys key in removeListT)
            {
                if (triggerOnce.Contains(key)) triggerOnce.Remove(key);
            }

            foreach(Keys key in state.GetPressedKeys())
            {
                if(pressedKeys.Contains(key) && !triggerOnce.Contains(key))
                {
                    triggerOnce.Add(key);
                }
                else
                {
                    pressedKeys.Add(key);
                }
            }    
        }
    }
}
