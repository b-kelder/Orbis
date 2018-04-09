using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Orbis.Engine
{
    /// <summary>
    /// An enumerator to mirror mousebuttons.
    /// </summary>
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    };

    /// <summary>
    /// Adds functionality to the default Keyboard, Mouse and GamePad state models respectively.
    /// </summary>
    /// <Author>Jannick Zeegers</Author>
    public class InputHandler
    {
        /// <summary>
        /// The static variable holding the once instance of the singleton InputHandler.
        /// </summary>
        private static InputHandler uniqueInstance;

        /// <summary>
        /// The state of the keyboard.
        /// </summary>
        private KeyboardState kState;

        /// <summary>
        /// The previous state of the keyboard.
        /// </summary>
        private KeyboardState kStatePrevious;

        /// <summary>
        /// The state of the mouse.
        /// </summary>
        private MouseState mState;

        /// <summary>
        /// The previous state of the mouse.
        /// </summary>
        private MouseState mStatePrevious;

        /// <summary>
        /// The state of the gamepad.
        /// </summary>
        private GamePadState gState;

        /// <summary>
        /// The previous state of the gamepad.
        /// </summary>
        private GamePadState gStatePrevious;

        /// <summary>
        /// The private constructor for InputHandler.
        /// </summary>
        private InputHandler() { }

        /// <summary>
        /// Used to get the single instance of the InputHandler.
        /// </summary>
        /// <returns>Returns the singleton InputHandler.</returns>
        public static InputHandler GetInstance()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new InputHandler();
            }
            return uniqueInstance;
        }

        #region KeyboardInput
        /// <summary>
        /// Checks if a given key is currently held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is held down.</returns>
        public bool IsKeyHeld(Keys key)
        {
            return kState.IsKeyDown(key);
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
                if (kState.IsKeyUp(key)) return false;
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
            return kState.IsKeyDown(key) && kStatePrevious.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a given key is down once while a modifier is being held down simultaneously.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="modifier">The modifier key to check.</param>
        /// <returns>Returns true if the modifier is held down and the key is down once.</returns>
        public bool IsKeyDown(Keys key, Keys modifier)
        {
            if(kState.IsKeyDown(modifier))
            {
                return kState.IsKeyDown(key) && kStatePrevious.IsKeyUp(key);
            }
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
                if (kState.IsKeyUp(modifier)) return false;
            }
            return kState.IsKeyDown(key) && kStatePrevious.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a given key is currently not held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is not held down.</returns>
        public bool IsKeyUp(Keys key)
        {
            return kState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if multiple given keys are currently not held down.
        /// </summary>
        /// <param name="keys">An array of keys to check.</param>
        /// <returns>Returns true if all keys are not held down.</returns>
        public bool IsKeyUp(Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (kState.IsKeyDown(key)) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given key is released from being held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is released from being held down.</returns>
        public bool IsKeyReleased(Keys key)
        {
            return kState.IsKeyUp(key) && kStatePrevious.IsKeyDown(key);
        }

        /// <summary>
        /// Updates the current keyboard keys states.
        /// </summary>
        private void UpdateKeyboardInput()
        {
            KeyboardState newKState = Keyboard.GetState();
            kStatePrevious = kState;
            kState = newKState;
        }
        #endregion

        #region MouseInput
        /// <summary>
        /// Checks if a given mousebutton is currently held down.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <returns>Returns true if the mousebutton is held down.</returns>
        public bool IsMouseHold(MouseButton mButton)
        {
            return GetButtonState(mButton) == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if multiple given mousebuttons are currently held down.
        /// </summary>
        /// <param name="mButtons">An array of mousebuttons to check.</param>
        /// <returns>Returns true if all mousebuttons are held down.</returns>
        public bool IsMouseHold(MouseButton[] mButtons)
        {
            foreach(MouseButton mButton in mButtons)
            {
                if (GetButtonState(mButton) == ButtonState.Released) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given mousebutton is down once.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <returns>Returns true if the mousebutton is down once.</returns>
        public bool IsMouseDown(MouseButton mButton)
        {
            return GetButtonStatePrevious(mButton) == ButtonState.Released && 
                   GetButtonState(mButton)         == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if a given mousebutton is down once while a modifier is being held down simultaneously.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <param name="modifier">The modifier mousebutton to check.</param>
        /// <returns>Returns true if the modifier is held down and the mousebutton is down once.</returns>
        public bool IsMouseDown(MouseButton mButton, MouseButton modifier)
        {
            if (GetButtonState(modifier) == ButtonState.Pressed)
            {
                return GetButtonStatePrevious(mButton) == ButtonState.Released &&
                       GetButtonState(mButton)         == ButtonState.Pressed;
            }
            return false;
        }

        /// <summary>
        /// Checks if a given mousebutton is down once while multiple modifiers are being held down simultaneously.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <param name="modifiers">An array of modifier mousebuttons to check.</param>
        /// <returns>Returns true if the modiefier mousebuttons are being held down and the mousebutton is down once.</returns>
        public bool IsMouseDown(MouseButton mButton, MouseButton[] modifiers)
        {
            foreach (MouseButton modifier in modifiers)
            {
                if (GetButtonState(modifier) == ButtonState.Released) return false;
            }
            return GetButtonStatePrevious(mButton) == ButtonState.Released &&
                   GetButtonState(mButton)         == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if a given mousebutton is currently not held down.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <returns>Returns true if the mousebutton is not held down.</returns>
        public bool IsMouseUp(MouseButton mButton)
        {
            return GetButtonState(mButton) == ButtonState.Released;
        }

        /// <summary>
        /// Checks if multiple given mousebuttons are currently not held down.
        /// </summary>
        /// <param name="mButtons">An array of mousebuttons to check.</param>
        /// <returns>Returns true if all mousebuttons are not held down.</returns>
        public bool IsMouseUp(MouseButton[] mButtons)
        {
            foreach(MouseButton mButton in mButtons)
            {
                if (GetButtonState(mButton) == ButtonState.Pressed) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given mousebutton is released from being held down.
        /// </summary>
        /// <param name="mButton">The mousebutton to check.</param>
        /// <returns>Returns true if the mousebutton is released from being held down.</returns>
        public bool IsMouseReleased(MouseButton mButton)
        {
            return GetButtonStatePrevious(mButton) == ButtonState.Pressed &&
                   GetButtonState(mButton)         == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the mousewheel has been used.
        /// </summary>
        /// <returns>Returns an integer with the mousewheel position.</returns>
        public int MouseScroll()
        {
            return mState.ScrollWheelValue - mStatePrevious.ScrollWheelValue;
        }

        /// <summary>
        /// Returns the difference in position of the mouse cursor since last check.
        /// </summary>
        /// <returns>Returns Point of difference.</returns>
        public Point MouseMove()
        {
            return mState.Position - mStatePrevious.Position;
        }

        /// <summary>
        /// Gets the current mousecursor position.
        /// </summary>
        /// <returns>Returns Point of current possition.</returns>
        public Point GetMousePosition()
        {
            return mState.Position;
        }

        /// <summary>
        /// Returns the state of the given MouseButton.
        /// </summary>
        /// <param name="button">The mousebutton to check.</param>
        /// <returns>Returns the ButtonState of the given Mousebutton</returns>
        private ButtonState GetButtonState(MouseButton button)
        {
            ButtonState bState = mState.LeftButton;
            if (button == MouseButton.Middle)
            {
                bState = mState.MiddleButton;
            }
            else if (button == MouseButton.Right)
            {
                bState = mState.RightButton;
            }
            return bState;
        }

        /// <summary>
        /// Returns the previous state of the given MouseButton.
        /// </summary>
        /// <param name="button">The mousebutton to check.</param>
        /// <returns>Returns the ButtonState of the given Mousebutton</returns>
        private ButtonState GetButtonStatePrevious(MouseButton button)
        {
            ButtonState bState = mStatePrevious.LeftButton;
            if (button == MouseButton.Middle)
            {
                bState = mStatePrevious.MiddleButton;
            }
            else if (button == MouseButton.Right)
            {
                bState = mStatePrevious.RightButton;
            }
            return bState;
        }

        /// <summary>
        /// Updates the current mousebutton states.
        /// </summary>
        private void UpdateMouseInput()
        {
            MouseState newMState = Mouse.GetState();
            mStatePrevious = mState;
            mState = newMState;
        }
        #endregion

        #region GamePadInput
        /// <summary>
        /// Checks if a given button is currently held down.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns true if the button is held down.</returns>
        public bool IsGamePadHold(Buttons button)
        {
            return gState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if multiple given buttons are currently held down.
        /// </summary>
        /// <param name="buttons">An array of buttons to check.</param>
        /// <returns>Returns true if all buttons are held down.</returns>
        public bool IsGamePadHold(Buttons[] buttons)
        {
            foreach(Buttons button in buttons)
            {
                if (gState.IsButtonUp(button)) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given button is down once.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns true if the button is down once.</returns>
        public bool IsGamePadDown(Buttons button)
        {
            return gState.IsButtonDown(button) && gStatePrevious.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if a given button is down once while a modifier is being held down simultaneously.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="modifier">The modifier button to check.</param>
        /// <returns>Returns true if the modifier is held down and the button is down once.</returns>
        public bool IsGamePadDown(Buttons button, Buttons modifier)
        {
            if (gState.IsButtonDown(modifier))
            {
                return gState.IsButtonDown(button) && gStatePrevious.IsButtonUp(button);
            }
            return false;
        }

        /// <summary>
        /// Checks if a given button is down once while multiple modifiers are being held down simultaneously.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="modifiers">An array of modifier buttons to check.</param>
        /// <returns>Returns true if the modiefier buttons are being held down and the button is down once.</returns>
        public bool IsGamePadDown(Buttons button, Buttons[] modifiers)
        {
            foreach (Buttons modifier in modifiers)
            {
                if (gState.IsButtonUp(modifier)) return false;
            }
            return gState.IsButtonDown(button) && gStatePrevious.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if a given button is currently not held down.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns true if the button is not held down.</returns>
        public bool IsGamePadUp(Buttons button)
        {
            return gState.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if multiple given buttons are currently not held down.
        /// </summary>
        /// <param name="buttons">An array of buttons to check.</param>
        /// <returns>Returns true if all buttons are not held down.</returns>
        public bool IsGamePadUp(Buttons[] buttons)
        {
            foreach (Buttons button in buttons)
            {
                if (gState.IsButtonDown(button)) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a given button is released from being held down.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>Returns true if the button is released from being held down.</returns>
        public bool IsGamePadReleased(Buttons button)
        {
            return gState.IsButtonUp(button) && gStatePrevious.IsButtonDown(button);
        }

        /// <summary>
        /// Updates the current gamepad button states.
        /// </summary>
        private void UpdateGamePadInput()
        {
            GamePadState newGState = GamePad.GetState(0);
            gStatePrevious = gState;
            gState = newGState;
        }
        #endregion

        /// <summary>
        /// Updates the states of all the input devices.
        /// </summary>
        public void UpdateInput()
        {
            UpdateKeyboardInput();
            UpdateMouseInput();
            UpdateGamePadInput();
        }
    }
}
