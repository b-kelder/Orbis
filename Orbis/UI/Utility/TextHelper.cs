using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orbis.UI.Utility
{
    /// <summary>
    ///     Contains helper functions for text handling.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public static class TextHelper
    {
        /// <summary>
        ///     Wrap a text string to fit within the given width.
        /// </summary>
        /// 
        /// <param name="font">
        ///     The font intended for drawing the text.
        /// </param>
        /// <param name="text">
        ///     The text to wrap.
        /// </param>
        /// <param name="maxWidth">
        ///     The maximum line width for the text.
        /// </param>
        /// 
        /// <returns>
        ///     A wrapped piece of text.
        /// </returns>
        public static string WrapText(SpriteFont font, string text, float maxWidth)
        {
            List<string> wrappedLines = new List<string>();
            StringBuilder builder = new StringBuilder();

            // Gets the lines in the string.
            string[] delimiters = new string[2] { Environment.NewLine, "\n" };
            string[] lines = text.Split(delimiters, StringSplitOptions.None);

            foreach (string line in lines)
            {
                // The line is wrapped to fit within the text box.
                string wrappedLine = WrapLine(font, line, maxWidth);
                wrappedLines.Add(wrappedLine);
            }

            foreach (string wrappedLine in wrappedLines)
            {
                builder.AppendLine(wrappedLine);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Clips text to fit within the given width.
        /// </summary>
        /// 
        /// <param name="font">
        ///     The font intended for drawing the text.
        /// </param>
        /// <param name="text">
        ///     The text to wrap.
        /// </param>
        /// <param name="maxWidth">
        ///     The maximum line width for the text.
        /// </param>
        /// 
        /// <returns>
        ///     A clipped string.
        /// </returns>
        public static string ClipText(SpriteFont font, string text, float maxWidth)
        {
            // The text is wrapped to fit in the button.
            float textWidth;
            var wrapped = text;
            bool fits;

            // First measurement needs to be done regardless.
            do
            {
                textWidth = font.MeasureString(wrapped).X;
                if (textWidth > maxWidth)
                {
                    wrapped = wrapped.Substring(0, wrapped.Length - 1);
                    fits = false;
                }
                else
                {
                    fits = true;
                }
            } while (!fits);

            return wrapped.TrimEnd(' ');
        }

        /// <summary>
        ///     Wrap the given line to fit within the given width.
        /// </summary>
        /// 
        /// <param name="line">
        ///     The line to wrap.
        /// </param>
        /// <param name="maxWidth">
        ///     The maximum width of the text in the line.
        /// </param>
        /// 
        /// <returns>
        ///     A wrapped line.
        /// </returns>
        private static string WrapLine(SpriteFont font, string line, float maxWidth)
        {
            StringBuilder lineBuilder = new StringBuilder();
            float spaceWidth = font.MeasureString(" ").X;
            float lineWidth = 0F;

            string[] words = line.Split(' ');

            foreach (string word in words)
            {
                float wordWidth = font.MeasureString(word).X;

                // Round upwards to make sure rounding errors don't cause text to go beyond the max width.
                if (Math.Ceiling(lineWidth + wordWidth) < maxWidth)
                {
                    lineBuilder.Append(word + " ");
                    lineWidth += wordWidth + spaceWidth;
                }
                else
                {
                    // Round upwards to make sure rounding errors don't cause text to go beyond the max width.
                    if (Math.Ceiling(wordWidth) > maxWidth)
                    {
                        // Wrap words that don't fit in their entirety.
                        string remainder = "";
                        float remainderWidth;
                        do
                        {
                            float partWidth;
                            string part = word;
                            do
                            {
                                remainder = part.Substring(part.Length - 1) + remainder;
                                part = part.Substring(0, part.Length - 1);
                                partWidth = font.MeasureString(part).X;
                            } while (Math.Ceiling(partWidth) > maxWidth);

                            lineBuilder.Append(part + "\n");
                            remainderWidth = font.MeasureString(remainder).X;

                        } while (Math.Ceiling(remainderWidth) > maxWidth);
                        lineBuilder.Append(remainder + " ");
                    }
                    else
                    {
                        // Add the word on a new line.
                        lineBuilder.Append("\n" + word + " ");
                        lineWidth = spaceWidth;
                    }
                }
            }

            return lineBuilder.ToString();
        }
    }
}
