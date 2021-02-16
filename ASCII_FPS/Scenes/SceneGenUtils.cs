using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.Scenes
{
    public static class SceneGenUtils
    {
        public static List<Triangle> MakeWall(Vector2[] points, float h, AsciiTexture texture)
        {
            List<Triangle> triangles = new List<Triangle>();
            for (int i = 0; i < points.Length - 1; i++)
            {
                float x0 = points[i].X;
                float z0 = points[i].Y;
                float x1 = points[i + 1].X;
                float z1 = points[i + 1].Y;
                float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

                Triangle triangle1 = new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture,
                    new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f));
                Triangle triangle2 = new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
                    new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f));
                triangles.Add(triangle1);
                triangles.Add(triangle2);
            }

            return triangles;
        }

        public static void AddWalls(Scene scene, Zone zone, List<Vector2[]> walls, float h, AsciiTexture texture, Vector3 offset)
        {
            Vector2 offset2D = new Vector2(offset.X, offset.Z);
            foreach (Vector2[] wall in walls)
            {
                List<Triangle> wallTriangles = MakeWall(wall, h, texture);
                MeshObject meshObject = new MeshObject(wallTriangles, offset, 0f);
                zone.AddMesh(meshObject);
                for (int i = 0; i < wall.Length - 1; i++)
                {
                    scene.AddWall(wall[i] + offset2D, wall[i + 1] + offset2D);
                }
            }
        }

        public static MeshObject MakeFloor(float left, float right, float bottom, float top, float roomHeight)
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
                new Triangle(tlh, brh, trh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitX),
                new Triangle(tll, brl, bll, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY),
                new Triangle(tlh, blh, brh, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitY, Vector2.One)
            };
            return new MeshObject(triangles, Vector3.Zero, 0f);
        }

        public static List<Vector2[]> MakeRoomWalls(float width, float height, float[] corridors, float wallThickness)
        {
            List<Vector2[]> result = new List<Vector2[]>();

            float x = (width - wallThickness) / 2;
            float x1 = width / 2;
            float y0 = (height - wallThickness) / 2;
            float y1 = -y0;

            Vector2 vecTopFront = new Vector2(x, y0);
            Vector2 vecTopBack = new Vector2(x1, y0);
            Vector2 vecBottomFront = new Vector2(x, y1);
            Vector2 vecBottomBack = new Vector2(x1, y1);
            Vector2 unit = -Vector2.UnitY;

            for (int t = 0; t < 4; t++)
            {
                if (corridors[t] > 0f)
                {
                    float wallLength = x - corridors[t] / 2;

                    result.Add(new Vector2[2] { vecTopFront, vecTopFront + unit * wallLength });
                    result.Add(new Vector2[2] { vecTopFront + unit * wallLength, vecTopBack + unit * wallLength });

                    result.Add(new Vector2[2] { vecBottomBack - unit * wallLength, vecBottomFront - unit * wallLength });
                    result.Add(new Vector2[2] { vecBottomFront - unit * wallLength, vecBottomFront });
                }
                else
                {
                    result.Add(new Vector2[2] { vecTopFront, vecBottomFront });
                }

                vecTopFront = new Vector2(-vecTopFront.Y, vecTopFront.X);
                vecTopBack = new Vector2(-vecTopBack.Y, vecTopBack.X);
                vecBottomFront = new Vector2(-vecBottomFront.Y, vecBottomFront.X);
                vecBottomBack = new Vector2(-vecBottomBack.Y, vecBottomBack.X);
                unit = new Vector2(-unit.Y, unit.X);
            }

            return result;
        }

        public static bool[,,] GenerateCorridorLayout(int sizeX, int sizeY)
        {
            Random rand = new Random();
            bool[,,] corridorLayout = new bool[sizeX, sizeY, 4];
            bool[,] vis = new bool[sizeX, sizeY];
            Queue<Point> BFSqueue = new Queue<Point>();

            int startX = rand.Next(sizeX);
            int startY = rand.Next(sizeY);
            BFSqueue.Enqueue(new Point(startX, startY));
            vis[startX, startY] = true;

            while (BFSqueue.Count > 0)
            {
                Point p = BFSqueue.Dequeue();
                int[] dir = new int[4] { 0, 1, 2, 3 };
                Point[] shift = new Point[4] { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
                for (int i = 0; i < 4; i++)
                {
                    int j = rand.Next(4);
                    int t = dir[i];
                    dir[i] = dir[j];
                    dir[j] = t;
                }

                for (int i = 0; i < 4; i++)
                {
                    Point q = p + shift[dir[i]];
                    if (q.X >= 0 && q.X < sizeX && q.Y >= 0 && q.Y < sizeY && !vis[q.X, q.Y])
                    {
                        vis[q.X, q.Y] = true;
                        corridorLayout[p.X, p.Y, dir[i]] = true;
                        corridorLayout[q.X, q.Y, (dir[i] + 2) % 4] = true;
                        BFSqueue.Enqueue(q);
                    }
                }
            }

            return corridorLayout;
        }
    }
}
