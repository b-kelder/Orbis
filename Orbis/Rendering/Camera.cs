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
        public Vector3 Position { get; set; }
        public Vector3 LookTarget { get; set; }
        public float ClipNear { get; set; }
        public float ClipFar { get; set; }
        public float FoV { get; set; }
        public float OrthographicScale { get; set; }
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

        public Matrix CreateViewMatrix()
        {
            return Matrix.CreateLookAt(Position, LookTarget, Vector3.UnitZ);
        }

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
