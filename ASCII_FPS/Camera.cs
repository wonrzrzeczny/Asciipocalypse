using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS
{
    public class Camera
    {
        public Vector3 CameraPos { get; set; }
        public float Rotation { get; set; }

        public float Near { get; private set; }
        public float Far { get; private set; }
        public float Fov { get; private set; }
        public float AspectRatio { get; private set; }
        public float FocalLength { get; private set; }

        private readonly float nearScreenWidth;
        private readonly float nearScreenHeight;
        public Vector4 LeftPlane { get; private set; }
        public Vector4 RightPlane { get; private set; }


        public Camera(float near, float far, float fov, float aspectRatio)
        {
            CameraPos = Vector3.Zero;
            Rotation = 0f;

            Near = near;
            Far = far;
            Fov = fov;
            AspectRatio = aspectRatio;
            FocalLength = 1 / (float)Math.Tan(fov / 2);

            nearScreenWidth = 2 * near * (float)Math.Tan(fov / 2);
            nearScreenHeight = nearScreenWidth / aspectRatio;

            LeftPlane = new Vector4(FocalLength / (float)Math.Sqrt(FocalLength * FocalLength + 1), 0, -1 / (float)Math.Sqrt(FocalLength * FocalLength + 1), 0);
            RightPlane = new Vector4(-FocalLength / (float)Math.Sqrt(FocalLength * FocalLength + 1), 0, -1 / (float)Math.Sqrt(FocalLength * FocalLength + 1), 0);
        }


        public Matrix ProjectionMatrix
        {
            get
            {
                return new Matrix(2 * Near / nearScreenWidth, 0, 0, 0,
                                  0, 2 * Near / nearScreenHeight, 0, 0,
                                  0, 0, (Far + Near) / (Far - Near), 1,
                                  0, 0, -2 * Near * Far / (Far - Near), 0);
            }
        }
    }
}