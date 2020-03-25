using Microsoft.Xna.Framework;

namespace OBJContentPipelineExtension
{
	public struct OBJFile
	{
		public Vector3[] vertices;
		public Vector2[] texcoords;

		public int[] triangleVertices;
		public int[] triangleTexcoords;
	}
}
