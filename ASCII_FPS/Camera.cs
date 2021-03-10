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
        public Vector3 LeftPlane { get; private set; }
        public Vector3 RightPlane { get; private set; }
        public Vector3 BottomPlane { get; private set; }
        public Vector3 TopPlane { get; private set; }


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

            LeftPlane = Vector3.Normalize(new Vector3(FocalLength, 0f, 1f));
            RightPlane = Vector3.Normalize(new Vector3(-FocalLength, 0f, 1f));

            BottomPlane = Vector3.Normalize(new Vector3(0f, FocalLength / aspectRatio, 1f));
            TopPlane = Vector3.Normalize(new Vector3(0f, -FocalLength / aspectRatio, 1f));
        }


        public Matrix ProjectionMatrix
        {
            get
            {
                return new Matrix(2 * Near / nearScreenWidth,  0,                            0,                               0,
                                  0,                           2 * Near / nearScreenHeight,  0,                               0,
                                  0,                           0,                            (Far + Near) / (Far - Near),     1,
                                  0,                           0,                            -2 * Near * Far / (Far - Near),  0);
            }
        }

        public Matrix CameraSpaceMatrix
        {
            get
            {
                return Mathg.TranslationMatrix(-CameraPos) * Mathg.RotationMatrix(-Rotation);
            }
        }

        public Vector3 Forward
        {
            get
            {
                return new Vector3((float)Math.Sin(Rotation), 0f, (float)Math.Cos(Rotation));
            }
        }

        public Vector3 Right
        {
            get
            {
                return new Vector3((float)Math.Cos(Rotation), 0f, (float)-Math.Sin(Rotation));
            }
        }
    }
}