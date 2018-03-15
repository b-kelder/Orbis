using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbis.UI
{
    /// <summary>
    ///     A textbox in the Orbis UI that the user can enter text into.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class InputTextBox : TextBox
    {
        ///// <summary>
        /////     Enables or disables multiline editing.
        ///// </summary>
        //public bool MultiLine { get; set; }

        ///// <summary>
        /////     Is the input box in focus?
        ///// </summary>
        //private bool _focus;

        ///// <summary>
        /////     The current line that the caret is positioned on.
        ///// </summary>
        //private int _currentLine;

        ///// <summary>
        /////     The current position of the caret within the line.
        ///// </summary>
        //private int _caretPos;

        ///// <summary>
        /////     Create a new <see cref="InputTextBox"/>.
        ///// </summary>
        //public InputTextBox() : base()
        //{
        //    MultiLine = true;
        //    _currentLine = 0;
        //    _caretPos = 0;
        //    _focus = true;
        //}

        ///// <summary>
        /////     Update the input text box.
        ///// </summary>
        ///// 
        ///// <param name="gameTime">
        /////     The game loop's current game time.
        ///// </param>
        //public override void Update(GameTime gameTime)
        //{
        //    base.Update(gameTime);
        //}

        ///// <summary>
        /////     Draw the input text box.
        ///// </summary>
        ///// 
        ///// <param name="spriteBatch">
        /////     The spriteBatch to use for drawing.
        ///// </param>
        ///// <param name="gameTime">
        /////     The game loop's current game time.
        ///// </param>
        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    base.Draw(spriteBatch, gameTime);
        //}

        ///// <summary>
        /////     Update the input text box layout.
        ///// </summary>
        //public override void UpdateLayout()
        //{
        //    base.UpdateLayout();
        //}

        ///// <summary>
        /////     The input handler for text input.
        ///// </summary>
        ///// 
        ///// <param name="sender">
        /////     The window that fired the event.
        ///// </param>
        ///// <param name="e">
        /////     The event args for the event.
        ///// </param>
        //public void Window_TextInput(object sender, TextInputEventArgs e)
        //{
        //    if (_focus)
        //    {
        //        string[] delimiters = new string[2] { Environment.NewLine, "\n" };
        //        string[] lines = _text.ToString().Split(delimiters, StringSplitOptions.None);
        //        string line = lines[_currentLine];
        //        if (e.Key == Keys.Back)
        //        {
        //            lines[_currentLine] = line.Remove(_caretPos - 1, 1);
        //        }
        //        else if (e.Key == Keys.Enter)
        //        {
        //            if (MultiLine)
        //            {
        //                line = line.Insert(line.Length, "\n");
        //            }
        //        }
        //        else
        //        {
        //            char character = e.Character;
        //            if (TextFont.Characters.Contains(character))
        //            {
        //                lines[_currentLine] = line.Insert(_caretPos, "" + character);
        //                _caretPos++;
        //            }
        //        }
        //        UpdateLayout();
        //    }
        //}
    }
}
