using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    enum CameraMode
    {
        Perspective,
        Orthographic
    }

    /// <summary>
    /// Represents a camera in a 3d world.
    /// </summary>
    class Camera
    {
        /// <summary>
        /// Position of camera in the world.
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Location that the camera should look at.
        /// </summary>
        public Vector3 LookTarget { get; set; }
        /// <summary>
        /// Near clip distance.
        /// </summary>
        public float ClipNear { get; set; }
        /// <summary>
        /// Far clip distance.
        /// </summary>
        public float ClipFar { get; set; }
        /// <summary>
        /// Field of View in degrees.
        /// </summary>
        public float FoV { get; set; }
        /// <summary>
        /// Scale used for Orthographic mode.
        /// </summary>
        public float OrthographicScale { get; set; }
        /// <summary>
        /// Current mode for the camera.
        /// </summary>
        public CameraMode Mode { get; set; }

        public Camera()
        {
            Position = Vector3.Zero;
            LookTarget = Vector3.Zero;
            ClipNear = 0.1f;
            ClipFar = 500;
            FoV = 65;
            OrthographicScale = 10;
            Mode = CameraMode.Perspective;
        }

        /// <summary>
        /// Creates this camera's view matrix.
        /// </summary>
        /// <returns>View matrix</returns>
        public Matrix CreateViewMatrix()
        {
            return Matrix.CreateLookAt(Position, LookTarget, Vector3.UnitZ);
        }

        /// <summary>
        /// Creates this camera's projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio to use.</param>
        /// <returns>Projection matrix</returns>
        public Matrix CreateProjectionMatrix(float aspectRatio)
        {
            if(Mode == CameraMode.Perspective)
            {
                return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FoV), aspectRatio, ClipNear, ClipFar);
            }
            else
            {
                return Matrix.CreateOrthographic(OrthographicScale * aspectRatio, OrthographicScale, ClipNear, ClipFar);
            }            
        }
    }
}
