namespace Orbis.UI
{
    /// <summary>
    ///     Interface for UI elements that can be updated.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public interface IUpdateableElement
    {
        /// <summary>
        ///     Perform this frame's update.
        /// </summary>
        void Update();

        /// <summary>
        ///     Is the element in focus?
        /// </summary>
        bool Focused { get; set; }
    }
}
