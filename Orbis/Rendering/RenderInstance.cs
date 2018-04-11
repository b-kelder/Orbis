using Microsoft.Xna.Framework;

namespace Orbis.Rendering
{
    /// <summary>
    /// An instance of a RenderableMesh to render with specific settings.
    /// </summary>
    struct RenderInstance
    {
        /// <summary>
        /// Mesh to render
        /// </summary>
        public RenderableMesh mesh;
        /// <summary>
        /// Transformation (world) matrix to apply
        /// </summary>
        public Matrix matrix;
    }
}
