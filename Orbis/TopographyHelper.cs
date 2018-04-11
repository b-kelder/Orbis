using Microsoft.Xna.Framework;
using System;

namespace Orbis
{
    class TopographyHelper
    {
        /// <summary>
        /// Returns the distance between two coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Distance(Point a, Point b)
        {
            var cubeA = ConvertAxialToCube(a.ToVector2());
            var cubeB = ConvertAxialToCube(b.ToVector2());
            return Math.Max(Math.Abs(cubeA.X - cubeB.X), Math.Max(Math.Abs(cubeA.Y - cubeB.Y), Math.Abs(cubeA.Z = cubeB.Z)));
        }
        /// <summary>
        /// Converts a hex point to a world point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 HexToWorld(Point point)
        {
            Vector2 qVector = new Vector2((float)Math.Sqrt(3), 0);
            Vector2 pVector = new Vector2((float)Math.Sqrt(3) / 2, 1.5f);
            return qVector * point.X + pVector * point.Y;
        }

        /// <summary>
        /// Converts a world point to a rounded hex point.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static Point RoundWorldToHex(Vector2 world)
        {
            var q = (float)(Math.Sqrt(3) / 3) * world.X - (1.0f / 3) * world.Y;
            var p = (2.0f / 3) * world.Y;
            return RoundHex(new Vector2(q, p));
        }

        private static Point RoundHex(Vector2 hex)
        {
            return ConvertCubeToAxial(RoundCube(ConvertAxialToCube(hex))).ToPoint();
        }

        private static Vector3 RoundCube(Vector3 cube)
        {
            var rx = (float)Math.Round(cube.X);
            var ry = (float)Math.Round(cube.Y);
            var rz = (float)Math.Round(cube.Z);

            var x_diff = Math.Abs(rx - cube.X);
            var y_diff = Math.Abs(ry - cube.Y);
            var z_diff = Math.Abs(rz - cube.Z);

            if(x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if(y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }
            return new Vector3(rx, ry, rz);
        }

        private static Vector2 ConvertCubeToAxial(Vector3 cube)
        {
            return new Vector2(cube.X, cube.Z);
        }

        private static Vector3 ConvertAxialToCube(Vector2 axial)
        {
            return new Vector3(axial.X, -axial.X - axial.Y, axial.Y);
        }
    }
}
