namespace Orbis.UI
{
    /// <summary>
    ///     Interface for UI elements that can be updated.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public interface IUpdatableElement
    {
        /// <summary>
        ///     Perform this frame's update.
        /// </summary>
        void Update();
    }
}
