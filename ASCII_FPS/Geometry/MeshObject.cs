using Microsoft.Xna.Framework;
using OBJContentPipelineExtension;
using System.Collections.Generic;

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

        public MeshObject(OBJFile objFile, AsciiTexture texture, Vector3 position)
        {
            Position = position;
            triangles = new List<Triangle>();
            for (int i = 0; i < objFile.triangleVertices.Length; i += 3)
            {
                Vector3 v0 = objFile.vertices[objFile.triangleVertices[i]];
                Vector3 v1 = objFile.vertices[objFile.triangleVertices[i + 1]];
                Vector3 v2 = objFile.vertices[objFile.triangleVertices[i + 2]];
                Vector2 uv0 = objFile.texcoords[objFile.triangleTexcoords[i]];
                Vector2 uv1 = objFile.texcoords[objFile.triangleTexcoords[i + 1]];
                Vector2 uv2 = objFile.texcoords[objFile.triangleTexcoords[i + 2]];
                triangles.Add(new Triangle(v0, v1, v2, texture, uv0, uv1, uv2));
            }
        }


        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
        }

        public Matrix WorldSpaceMatrix
        {
            get
            {
                return Mathg.RotationMatrix(Rotation) * Mathg.TranslationMatrix(Position);
            }
        }
    }
}
