using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace OBJContentPipelineExtension
{
	public class OBJContentReader : ContentTypeReader<OBJFile>
	{
		protected override OBJFile Read(ContentReader input, OBJFile existingInstance)
		{
			OBJFile obj = new OBJFile();

			int verticesCount = input.ReadInt32();
			int texcoordsCount = input.ReadInt32();
			int triangleVerticesCount = input.ReadInt32();
			int triangleTexcoordsCount = input.ReadInt32();

			obj.vertices = new Vector3[verticesCount];
			obj.texcoords = new Vector2[texcoordsCount];
			obj.triangleVertices = new int[triangleVerticesCount];
			obj.triangleTexcoords = new int[triangleTexcoordsCount];

			for (int i = 0; i < verticesCount; i++)
				obj.vertices[i] = input.ReadVector3();

			for (int i = 0; i < texcoordsCount; i++)
				obj.texcoords[i] = input.ReadVector2();

			for (int i = 0; i < triangleVerticesCount; i++)
				obj.triangleVertices[i] = input.ReadInt32();

			for (int i = 0; i < triangleTexcoordsCount; i++)
				obj.triangleTexcoords[i] = input.ReadInt32();

			return obj;
		}
	}
}
