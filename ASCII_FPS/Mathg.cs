using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS
{
    public static class Mathg
    {
        public static Matrix RotationMatrix(float angle)
        {
            return new Matrix((float)Math.Cos(angle),  0,  -(float)Math.Sin(angle),  0,
                              0,                       1,  0,                        0,
                              (float)Math.Sin(angle),  0,  (float)Math.Cos(angle),   0,
                              0,                       0,  0,                        1);
        }

        public static Matrix RotationMatrix2D(float angle)
        {
            return new Matrix((float)Math.Cos(angle),  -(float)Math.Sin(angle), 0,  0,
                              (float)Math.Sin(angle),  (float)Math.Cos(angle),  0,  0,
                              0,                       0,                       0,  0,
                              0,                       0,                       0,  0);

        }

        public static Matrix TranslationMatrix(Vector3 translation)
        {
            return new Matrix(1,              0,              0,              0,
                              0,              1,              0,              0,
                              0,              0,              1,              0,
                              translation.X,  translation.Y,  translation.Z,  1);
        }

        public static Vector3 Barycentric(Vector2 p, Vector2 v0, Vector2 v1, Vector2 v2)
        {
            float dotp1 = Vector2.Dot(p - v0, v1 - v0);
            float dotp2 = Vector2.Dot(p - v0, v2 - v0);

            float dot11 = Vector2.Dot(v1 - v0, v1 - v0);
            float dot22 = Vector2.Dot(v2 - v0, v2 - v0);
            float dot12 = Vector2.Dot(v1 - v0, v2 - v0);
            float det = dot11 * dot22 - dot12 * dot12;

            float bar1 = (dot22 * dotp1 - dot12 * dotp2) / det;
            float bar2 = (dot11 * dotp2 - dot12 * dotp1) / det;
            float bar0 = 1 - bar1 - bar2;

            return new Vector3(bar0, bar1, bar2);
        }

        public static Vector3 OrthogonalComponent(Vector3 vector, Vector3 normal)
        {
            return 0.5f * (vector + Vector3.Reflect(vector, normal));
        }

        public static float Cross2D(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public static byte ColorTo8Bit(Vector3 c)
        {
            byte r = (byte)(Clamp(c.X, 0, 0.9f) * 8);
            byte g = (byte)(Clamp(c.Y, 0, 0.9f) * 8);
            byte b = (byte)(Clamp(c.Z, 0, 0.8f) * 4);

            return (byte)(r + (g << 3) + (b << 6));
        }


        public static T DiscreteChoice<T>(Random rng, T[] elems, float[] weights)
        {
            float sum = weights.Sum();
            float choice = (float)rng.NextDouble() * sum;

            int pos = 0;
            while (choice > weights[pos])
            {
                choice -= weights[pos];
                pos++;
            }

            return elems[pos];
        }

        public static T DiscreteChoiceFn<T>(Random rng, Func<T>[] elemFuncs, float[] weights)
        {
            Func<T> elemFunc = DiscreteChoice(rng, elemFuncs, weights);
            return elemFunc.Invoke();
        }
    }
}
