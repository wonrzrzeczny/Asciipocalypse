using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ASCII_FPS
{
    public class Scene
    {
        public List<MeshObject> dynamicMeshes;
		public List<Zone> zones;

		public int TotalTriangles { get; private set; }

        private List<Vector2[]> walls;

        public Scene()
        {
			dynamicMeshes = new List<MeshObject>();
			zones = new List<Zone>();
            walls = new List<Vector2[]>();
        }

		public void AddZone(Zone zone)
		{
			zones.Add(zone);
			foreach (MeshObject mesh in zone.meshes)
			{
				TotalTriangles += mesh.triangles.Count;
			}
		}

        public void AddDynamicMesh(MeshObject mesh)
        {
			dynamicMeshes.Add(mesh);
			TotalTriangles += mesh.triangles.Count;
        }

		public void RemoveDynamicMesh(MeshObject mesh)
		{
			TotalTriangles -= mesh.triangles.Count;
			dynamicMeshes.Remove(mesh);
		}

        public void AddWall(float x0, float z0, float x1, float z1)
        {
			walls.Add(new Vector2[2] { new Vector2(x0, z0), new Vector2(x1, z1) });
        }

		public bool CheckMovement(Vector3 from, Vector3 direction, float radius)
		{
			Vector2 from2 = new Vector2(from.X, from.Z);
			Vector2 direction2 = new Vector2(direction.X, direction.Z);
			Vector2 to2 = from2 + direction2;
			foreach (Vector2[] wall in walls)
			{
				Vector2 v0 = wall[0];
				Vector2 v1 = wall[1];
				Vector2 normal2 = Vector2.Normalize(new Vector2((v1 - v0).Y, -(v1 - v0).X));
				if (Mathg.Cross2D(v1 - v0, from2 - v0) > 0)
					normal2 *= -1;
				v0 += normal2 * radius;
				v1 += normal2 * radius;

				if (Mathg.Cross2D(v1 - v0, from2 - v0) * Mathg.Cross2D(v1 - v0, to2 - v0) < 0 &&
					Mathg.Cross2D(to2 - from2, v0 - from2) * Mathg.Cross2D(to2 - from2, v1 - from2) < 0)
					return false;
			}

			return true;
		}

		public bool CheckMovement(Vector3 from, Vector3 direction, float radius, out Vector3 normal, out float relativeLength)
		{
			normal = Vector3.Zero;
			relativeLength = float.MaxValue;
			bool ret = true;

			Vector2 from2 = new Vector2(from.X, from.Z);
			Vector2 direction2 = new Vector2(direction.X, direction.Z);
			Vector2 to2 = from2 + direction2;
			foreach (Vector2[] wall in walls)
			{
				Vector2 v0 = wall[0];
				Vector2 v1 = wall[1];
				Vector2 normal2 = Vector2.Normalize(new Vector2((v1 - v0).Y, -(v1 - v0).X));
				if (Mathg.Cross2D(v1 - v0, from2 - v0) > 0)
					normal2 *= -1;
				v0 += normal2 * radius;
				v1 += normal2 * radius;

				if (Mathg.Cross2D(v1 - v0, from2 - v0) * Mathg.Cross2D(v1 - v0, to2 - v0) < 0 &&
					Mathg.Cross2D(to2 - from2, v0 - from2) * Mathg.Cross2D(to2 - from2, v1 - from2) < 0)
				{
					float t = Vector2.Dot(v0 - from2, normal2) / Vector2.Dot(direction2, normal2);
					if (t < relativeLength)
					{
						relativeLength = t;
						normal = new Vector3(normal2.X, 0f, normal2.Y);
					}
					ret = false;
				}
			}

			return ret;
		}


		// Given start position and movement vector, return the real movement vector taking into account collisions
		public Vector3 SmoothMovement(Vector3 from, Vector3 direction, float radius)
		{
			Vector3 ret = direction;
			while (!CheckMovement(from, ret, radius, out Vector3 normal, out float relativeLength))
			{
				relativeLength *= 0.9f;
				if (relativeLength < 0.01f)
					relativeLength = 0f;
				ret = relativeLength * ret + Mathg.OrthogonalComponent((1f - relativeLength) * ret, normal);
			}
			
			return ret;
		}
	}
}