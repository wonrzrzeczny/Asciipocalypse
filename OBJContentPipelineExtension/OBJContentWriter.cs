using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBJContentPipelineExtension
{
	[ContentTypeWriter]
	public class OBJContentWriter : ContentTypeWriter<OBJFile>
	{
		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return "OBJContentPipelineExtension.OBJContentReader, OBJContentPipelineExtension";
		}

		protected override void Write(ContentWriter output, OBJFile value)
		{
			output.Write((int)value.vertices.Length);
			output.Write((int)value.texcoords.Length);
			output.Write((int)value.triangleVertices.Length);
			output.Write((int)value.triangleTexcoords.Length);

			foreach (Vector3 v in value.vertices)
				output.Write(v);
			foreach (Vector2 v in value.texcoords)
				output.Write(v);
			foreach (int v in value.triangleVertices)
				output.Write(v);
			foreach (int v in value.triangleTexcoords)
				output.Write(v);
		}
	}
}
