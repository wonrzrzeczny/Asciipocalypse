using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS
{
    public class Triangle
    {
        public Vector3 V0 { get; set; }
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        public Vector2 UV0 { get; set; }
        public Vector2 UV1 { get; set; }
        public Vector2 UV2 { get; set; }

        public AsciiTexture Texture { get; set; }

        public Vector3 Normal { get; }

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, AsciiTexture texture, Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            UV0 = uv0;
            UV1 = uv1;
            UV2 = uv2;
            Texture = texture;
            Normal = Vector3.Cross(v1 - v0, v2 - v0);
        }

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, AsciiTexture texture) : this(v0, v1, v2, texture, Vector2.Zero, Vector2.Zero, Vector2.Zero) { }



        public void Save(BinaryWriter writer)
        {
            GameSave.WriteVector3(writer, V0);
            GameSave.WriteVector3(writer, V1);
            GameSave.WriteVector3(writer, V2);

            GameSave.WriteVector2(writer, UV0);
            GameSave.WriteVector2(writer, UV1);
            GameSave.WriteVector2(writer, UV2);

            GameSave.WriteVector3(writer, Normal);
        }

        public static Triangle Load(BinaryReader reader)
        {
            Vector3 v0 = GameSave.ReadVector3(reader);
            Vector3 v1 = GameSave.ReadVector3(reader);
            Vector3 v2 = GameSave.ReadVector3(reader);

            Vector2 uv0 = GameSave.ReadVector2(reader);
            Vector2 uv1 = GameSave.ReadVector2(reader);
            Vector2 uv2 = GameSave.ReadVector2(reader);
            
            Vector3 normal = GameSave.ReadVector3(reader);

            return new Triangle(v0, v1, v2, ASCII_FPS.texture1, uv0, uv1, uv2);
        }
    }
}