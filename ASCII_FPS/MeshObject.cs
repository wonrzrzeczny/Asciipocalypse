using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS
{
    public class MeshObject
    {
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }

        public readonly List<Triangle> triangles;

        public MeshObject()
        {
            triangles = new List<Triangle>();
            Position = Vector3.Zero;
            Rotation = 0f;
        }

        public MeshObject(List<Triangle> triangles)
        {
            this.triangles = triangles;
            Position = Vector3.Zero;
            Rotation = 0f;
        }

        public MeshObject(List<Triangle> triangles, Vector3 position, float rotation)
        {
            this.triangles = triangles;
            Position = position;
            Rotation = rotation;
        }


        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
        }

        public Matrix WorldSpaceMatrix
        {
            get
            {
                return Mathg.TranslationMatrix(Position) * Mathg.RotationMatrix(Rotation);
            }
        }
    }
}
