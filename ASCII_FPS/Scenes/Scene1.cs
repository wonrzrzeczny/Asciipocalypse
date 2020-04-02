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

		private static void AddFloor(Scene scene, float tileWidth, float tileHeight, float roomHeight, int numberTilesX, int numberTilesY)
		{
			for (int i = 0; i < numberTilesX; i++)
			{
				for (int j = 0; j < numberTilesY; j++)
				{
					float left = i * tileWidth - numberTilesX * tileWidth / 2;
					float right = left + tileWidth;
					float top = j * tileHeight - numberTilesY * tileHeight / 2;
					float bottom = top + tileHeight;

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
					scene.AddDynamicMesh(new MeshObject(triangles, Vector3.Zero, 0f));
				}
			}
		}

		public static void AddWalls(Scene scene, bool[,,] corridors, float tileWidth, float tileHeight, float wallThickness, float corridorWidth, float roomHeight, int numberTilesX, int numberTilesY)
		{
			for (int i = 0; i < numberTilesX; i++)
			{
				for (int j = 0; j < numberTilesY; j++)
				{
					List<Triangle> roomTriangles = new List<Triangle>();

					float shiftX = (i + 0.5f) * tileWidth - numberTilesX * tileWidth / 2;
					float shiftY = (j + 0.5f) * tileHeight - numberTilesY * tileHeight / 2;

					float x = tileWidth / 2;
					float y0 = -tileHeight / 2;
					float yc0 = -corridorWidth / 2;
					float yc1 = corridorWidth / 2;
					float y1 = tileHeight / 2;

					Vector2 top = new Vector2(x, y0);
					Vector2 topCorridor = new Vector2(x, yc0);
					Vector2 bottomCorridor = new Vector2(x, yc1);
					Vector2 bottom = new Vector2(x, y1);
					
					for (int t = 0; t < 4; t++)
					{
						if (corridors[i, j, t])
						{
							roomTriangles.AddRange(MakeWall(top.X, top.Y, topCorridor.X, topCorridor.Y, roomHeight, ASCII_FPS.texture1));
							roomTriangles.AddRange(MakeWall(bottomCorridor.X, bottomCorridor.Y, bottom.X, bottom.Y, roomHeight, ASCII_FPS.texture1));
							scene.AddWall(top.X + shiftX, top.Y + shiftY, topCorridor.X + shiftX, topCorridor.Y + shiftY);
							scene.AddWall(bottomCorridor.X + shiftX, bottomCorridor.Y + shiftY, bottom.X + shiftX, bottom.Y + shiftY);
						}
						else
						{
							roomTriangles.AddRange(MakeWall(top.X, top.Y, bottom.X, bottom.Y, roomHeight, ASCII_FPS.texture1));
							scene.AddWall(top.X + shiftX, top.Y + shiftY, bottom.X + shiftX, bottom.Y + shiftY);
						}
						
						top = new Vector2(top.Y, -top.X);
						topCorridor = new Vector2(topCorridor.Y, -topCorridor.X);
						bottomCorridor = new Vector2(bottomCorridor.Y, -bottomCorridor.X);
						bottom = new Vector2(bottom.Y, -bottom.X);
					}

					scene.AddDynamicMesh(new MeshObject(roomTriangles, new Vector3(shiftX, 0f, shiftY), 0f));
				}
			}
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

			AddFloor(scene, tileSize, tileSize, 4f, size, size);
			AddWalls(scene, corridors, tileSize, tileSize, 2f, 10f, 4f, size, size);

			return scene;
		}
	}
}
