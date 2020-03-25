using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace OBJContentPipelineExtension
{
	[ContentImporter(".obj", DisplayName = "OBJ Model Importer", DefaultProcessor = "OBJProcessor")]
	public class OBJImporter : ContentImporter<OBJFile>
	{
		public override OBJFile Import(string filename, ContentImporterContext context)
		{
			string[] data = File.ReadAllLines(filename);
			
			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> texcoords = new List<Vector2>();
			List<int> triangleVertices = new List<int>();
			List<int> triangleTexcoords = new List<int>();

			foreach (string line in data)
			{
				string[] lineData = line.Replace('.', ',').Split(' ');

				if (line.StartsWith("v "))
				{
					float x = float.Parse(lineData[1]);
					float y = float.Parse(lineData[2]);
					float z = float.Parse(lineData[3]);
					vertices.Add(new Vector3(x, y, z));
				}

				if (line.StartsWith("vt "))
				{
					float x = float.Parse(lineData[1]);
					float y = float.Parse(lineData[2]);
					texcoords.Add(new Vector2(x, 1f - y));
				}

				if (line.StartsWith("f "))
				{
					int v0 = int.Parse(lineData[1].Split('/')[0]) - 1;
					int t0 = int.Parse(lineData[1].Split('/')[1]) - 1;
					for (int i = 3; i < lineData.Length; i++)
					{
						int v1 = int.Parse(lineData[i - 1].Split('/')[0]) - 1;
						int v2 = int.Parse(lineData[i].Split('/')[0]) - 1;
						int t1 = int.Parse(lineData[i - 1].Split('/')[1]) - 1;
						int t2 = int.Parse(lineData[i].Split('/')[1]) - 1;
						triangleVertices.Add(v0);
						triangleVertices.Add(v1);
						triangleVertices.Add(v2);
						triangleTexcoords.Add(t0);
						triangleTexcoords.Add(t1);
						triangleTexcoords.Add(t2);
					}
				}
			}

			OBJFile file = new OBJFile
			{
				vertices = vertices.ToArray(),
				texcoords = texcoords.ToArray(),
				triangleVertices = triangleVertices.ToArray(),
				triangleTexcoords = triangleTexcoords.ToArray()
			};

			return file;
		}
	}
}
