using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ASCII_FPS.Scenes
{
	public static partial class Scenes
	{
		private static List<Triangle> MakeWall(float x0, float z0, float x1, float z1, float h, AsciiTexture texture)
		{
			float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

			Triangle triangle1 = new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture,
				new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f));
			Triangle triangle2 = new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
				new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f));

			return new List<Triangle>(new Triangle[] { triangle1, triangle2 });
		}

		private static MeshObject MakeFloor(float left, float right, float top, float bottom, float roomHeight)
		{
			Vector3 trl = new Vector3(right, -roomHeight, top);
			Vector3 trh = new Vector3(right, roomHeight, top);

			Vector3 tll = new Vector3(left, -roomHeight, top);
			Vector3 tlh = new Vector3(left, roomHeight, top);

			Vector3 brl = new Vector3(right, -roomHeight, bottom);
			Vector3 brh = new Vector3(right, roomHeight, bottom);

			Vector3 bll = new Vector3(left, -roomHeight, bottom);
			Vector3 blh = new Vector3(left, roomHeight, bottom);

			List<Triangle> triangles = new List<Triangle>
			{
				new Triangle(tll, trl, brl, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One),
				new Triangle(tlh, trh, brh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY),
				new Triangle(tll, brl, bll, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One),
				new Triangle(tlh, brh, blh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY)
			};
			return new MeshObject(triangles, Vector3.Zero, 0f);
		}

		public static MeshObject MakeRoomWalls(float left, float right, float top, float bottom,
											   bool[] corridors, float wallThickness, float corridorWidth, float roomHeight)
		{
			List<Triangle> roomTriangles = new List<Triangle>();

			float shiftX = (left + right) / 2;
			float shiftY = (top + bottom) / 2;

			float x = (right - left) / 2;
			float yc0 = -corridorWidth / 2;
			float yc1 = corridorWidth / 2;
			float y1 = (bottom - top) / 2;
			float y0 = -y1;

			Vector2 vecTop = new Vector2(x, y0);
			Vector2 vecTopCorridor = new Vector2(x, yc0);
			Vector2 vecBottomCorridor = new Vector2(x, yc1);
			Vector2 vecBottom = new Vector2(x, y1);
					
			for (int t = 0; t < 4; t++)
			{
				if (corridors[t])
				{
					roomTriangles.AddRange(MakeWall(vecTop.X, vecTop.Y, vecTopCorridor.X, vecTopCorridor.Y, roomHeight, ASCII_FPS.texture1));
					roomTriangles.AddRange(MakeWall(vecBottomCorridor.X, vecBottomCorridor.Y, vecBottom.X, vecBottom.Y, roomHeight, ASCII_FPS.texture1));
					//scene.AddWall(top.X + shiftX, top.Y + shiftY, topCorridor.X + shiftX, topCorridor.Y + shiftY);
					//scene.AddWall(bottomCorridor.X + shiftX, bottomCorridor.Y + shiftY, bottom.X + shiftX, bottom.Y + shiftY);
				}
				else
				{
					roomTriangles.AddRange(MakeWall(vecTop.X, vecTop.Y, vecBottom.X, vecBottom.Y, roomHeight, ASCII_FPS.texture1));
					//scene.AddWall(top.X + shiftX, top.Y + shiftY, bottom.X + shiftX, bottom.Y + shiftY);
				}
						
				vecTop = new Vector2(vecTop.Y, -vecTop.X);
				vecTopCorridor = new Vector2(vecTopCorridor.Y, -vecTopCorridor.X);
				vecBottomCorridor = new Vector2(vecBottomCorridor.Y, -vecBottomCorridor.X);
				vecBottom = new Vector2(vecBottom.Y, -vecBottom.X);
			}

			return new MeshObject(roomTriangles, new Vector3(shiftX, 0f, shiftY), 0f);
		}

		public static bool[,,] GenerateCorridors(int sizeX, int sizeY)
		{
			Random rand = new Random();
			bool[,,] ret = new bool[sizeX, sizeY, 4];
			for (int i = 0; i < sizeX - 1; i++)
			{
				for (int j = 0; j < sizeY - 1; j++)
				{
					ret[i, j, 0] = rand.Next(8) > 0; // Right
					ret[i, j, 1] = rand.Next(8) > 0; // Down
					ret[i + 1, j, 2] = ret[i, j, 0]; // Left
					ret[i, j + 1, 3] = ret[i, j, 1]; // Up
				}
			}
			return ret;
		}

		public static Scene Level1()
		{
			Scene scene = new Scene();

			int size = 5;
			float tileSize = 100f;
			bool[,,] corridors = GenerateCorridors(size, size);
			Zone[,] zones = new Zone[size, size];

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					float left = x * tileSize - size * tileSize / 2;
					float right = left + tileSize;
					float top = y * tileSize - size * tileSize / 2;
					float bottom = top + tileSize;
					bool[] roomCorridors = new bool[4] { corridors[x, y, 0], corridors[x, y, 1], corridors[x, y, 2], corridors[x, y, 3] };

					zones[x, y] = new Zone(new RectangleF(left, top, tileSize, tileSize));
					zones[x, y].AddMesh(MakeFloor(left, right, top, bottom, 4f));
					zones[x, y].AddMesh(MakeRoomWalls(left, right, top, bottom, roomCorridors, 2f, 10f, 4f));
					
					scene.AddZone(zones[x, y]);
				}
			}

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					float left = x * tileSize - size * tileSize / 2;
					float right = left + tileSize;
					float top = y * tileSize - size * tileSize / 2;
					float bottom = top + tileSize;

					if (corridors[x, y, 0])
						zones[x, y].AddPortal(new Portal(zones[x + 1, y], new Vector2(right, (top + bottom) / 2 - 10f),
																		  new Vector2(right, (top + bottom) / 2 + 10f)));
					if (corridors[x, y, 1])
						zones[x, y].AddPortal(new Portal(zones[x, y + 1], new Vector2((left + right) / 2 + 10f, bottom),
																		  new Vector2((left + right) / 2 - 10f, bottom)));
					if (corridors[x, y, 2])
						zones[x, y].AddPortal(new Portal(zones[x - 1, y], new Vector2(left, (top + bottom) / 2 + 10f),
																		  new Vector2(left, (top + bottom) / 2 - 10f)));
					if (corridors[x, y, 3])
						zones[x, y].AddPortal(new Portal(zones[x, y - 1], new Vector2((left + right) / 2 - 10f, top),
																		  new Vector2((left + right) / 2 + 10f, top)));
				}
			}

			return scene;
		}
	}
}
