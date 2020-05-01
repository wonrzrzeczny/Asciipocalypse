using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ASCII_FPS.Scenes
{
    public static class SceneGenerator
    {
        public const int size = 5;
        public const float tileSize = 100f;


        private static List<Triangle> MakeWall(float x0, float z0, float x1, float z1, float h, AsciiTexture texture)
        {
            float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

            Triangle triangle1 = new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture,
                new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f));
            Triangle triangle2 = new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
                new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f));

            return new List<Triangle>(new Triangle[] { triangle1, triangle2 });
        }

        private static MeshObject MakeFloor(float left, float right, float bottom, float top, float roomHeight)
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

        public static List<Vector2[]> MakeRoomWalls(float width, float height, bool[] corridors, float wallThickness, float corridorWidth)
        {
            List<Vector2[]> result = new List<Vector2[]>();

            float x = (width - wallThickness) / 2;
            float x1 = width / 2;
            float yc0 = corridorWidth / 2;
            float yc1 = -corridorWidth / 2;
            float y0 = (height - wallThickness) / 2;
            float y1 = -y0;

            Vector2 vecTop = new Vector2(x, y0);
            Vector2 vecTopCorridor = new Vector2(x, yc0);
            Vector2 vecTopCorridorMiddle = new Vector2(x1, yc0);
            Vector2 vecBottomCorridor = new Vector2(x, yc1);
            Vector2 vecBottomCorridorMiddle = new Vector2(x1, yc1);
            Vector2 vecBottom = new Vector2(x, y1);
                    
            for (int t = 0; t < 4; t++)
            {
                if (corridors[t])
                {
                    result.Add(new Vector2[2] { vecTop, vecTopCorridor });
                    result.Add(new Vector2[2] { vecTopCorridor, vecTopCorridorMiddle });
                    
                    result.Add(new Vector2[2] { vecBottomCorridorMiddle, vecBottomCorridor });
                    result.Add(new Vector2[2] { vecBottomCorridor, vecBottom });
                }
                else
                {
                    result.Add(new Vector2[2] { vecTop, vecBottom });
                }
                        
                vecTop = new Vector2(-vecTop.Y, vecTop.X);
                vecTopCorridor = new Vector2(-vecTopCorridor.Y, vecTopCorridor.X);
                vecTopCorridorMiddle = new Vector2(-vecTopCorridorMiddle.Y, vecTopCorridorMiddle.X);
                vecBottomCorridor = new Vector2(-vecBottomCorridor.Y, vecBottomCorridor.X);
                vecBottomCorridorMiddle = new Vector2(-vecBottomCorridorMiddle.Y, vecBottomCorridorMiddle.X);
                vecBottom = new Vector2(-vecBottom.Y, vecBottom.X);
            }

            return result;
        }

        public static bool[,,] GenerateCorridors(int sizeX, int sizeY)
        {
            Random rand = new Random();
            bool[,,] ret = new bool[sizeX, sizeY, 4];
            bool[,] vis = new bool[sizeX, sizeY];
            Queue<Point> BFSqueue = new Queue<Point>();
            int startX = rand.Next(size);
            int startY = rand.Next(size);
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
                        ret[p.X, p.Y, dir[i]] = true;
                        ret[q.X, q.Y, (dir[i] + 2) % 4] = true;
                        BFSqueue.Enqueue(q);
                    }
                }
            }
            return ret;
        }

        public static Scene Generate(float monsterHealth, float monsterDamage, int maxMonsters)
        {
            ASCII_FPS.playerStats.totalMonsters = 0;
            ASCII_FPS.playerStats.monsters = 0;
            Random rand = new Random();
            Scene scene = new Scene();

            bool[,,] corridors = GenerateCorridors(size, size);
            HUD.corridorLayout = corridors;
            Zone[,] zones = new Zone[size, size];

            int exitX = rand.Next(size);
            int exitY = rand.Next(size);
            while (exitX == size / 2 && exitY == size / 2)
            {
                exitX = rand.Next(size);
                exitY = rand.Next(size);
            }
            HUD.exitRoom = new Point(exitX, exitY);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float left = x * tileSize - size * tileSize / 2;
                    float right = left + tileSize;
                    float bottom = y * tileSize - size * tileSize / 2;
                    float top = bottom + tileSize;
                    bool[] roomCorridors = new bool[4] { corridors[x, y, 0], corridors[x, y, 1], corridors[x, y, 2], corridors[x, y, 3] };
                    List<Vector2[]> walls = MakeRoomWalls(tileSize, tileSize, roomCorridors, 2f, 10f);
                    Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);

                    zones[x, y] = new Zone(new RectangleF(left, bottom, tileSize, tileSize));
                    zones[x, y].AddMesh(MakeFloor(left, right, bottom, top, 4f));

                    List<Triangle> wallTriangles = new List<Triangle>();
                    foreach (Vector2[] wall in walls)
                    {
                        wallTriangles.AddRange(MakeWall(wall[0].X, wall[0].Y, wall[1].X, wall[1].Y, 4f, ASCII_FPS.texture1));
                    }
                    MeshObject wallObject = new MeshObject(wallTriangles, new Vector3(roomCenter.X, 0f, roomCenter.Y), 0f);
                    zones[x, y].AddMesh(wallObject);
                    
                    scene.AddZone(zones[x, y]);
                    foreach (Vector2[] wall in walls)
                    {
                        scene.AddWall(wall[0] + roomCenter, wall[1] + roomCenter);
                    }


                    if (x == exitX && y == exitY)
                    {
                        MeshObject exit = new MeshObject(ASCII_FPS.exitModel, ASCII_FPS.exitTexture,
                            new Vector3(roomCenter.X, -2f, roomCenter.Y));
                        zones[x, y].AddMesh(exit);
                        ASCII_FPS.playerStats.exitPosition = roomCenter;
                    }
                    else if (x != size / 2 || y != size / 2)
                    {
                        int cnt = rand.Next(1, maxMonsters + 1);
                        ASCII_FPS.playerStats.totalMonsters += cnt;
                        Vector2 shift = cnt == 1 ? Vector2.Zero : new Vector2(30f, 0f);
                        float offset = (float)(rand.NextDouble() * Math.PI * 2f);
                        for (int i = 0; i < cnt; i++)
                        {
                            Vector2 position = roomCenter + Vector2.Transform(shift, Mathg.RotationMatrix2D(offset + i * (float)Math.PI * 2f / cnt));
                            MeshObject monster = PrimitiveMeshes.Tetrahedron(new Vector3(position.X, -1f, position.Y), 3f, ASCII_FPS.monsterTexture);
                            scene.AddGameObject(new Monster(monster, 3f, monsterHealth, monsterDamage));
                        }

                        if (cnt != 1 && rand.Next(4) == 0)
                        {
                            int rnd = rand.Next(6);
                            Collectible.Type type = rnd < 3 ? Collectible.Type.Skill : rnd < 5 ? Collectible.Type.Armor : Collectible.Type.Health;
                            AsciiTexture texture = rnd < 3 ? ASCII_FPS.barrelBlueTexture : rnd < 5 ? 
                                ASCII_FPS.barrelGreenTexture : ASCII_FPS.barrelRedTexture;
                            MeshObject barrel = new MeshObject(ASCII_FPS.barrelModel, texture, new Vector3(roomCenter.X, -3f, roomCenter.Y));
                            scene.AddGameObject(new Collectible(barrel, type));
                        }
                    }
                }
            }

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float left = x * tileSize - size * tileSize / 2;
                    float right = left + tileSize;
                    float bottom = y * tileSize - size * tileSize / 2;
                    float top = bottom + tileSize;

                    if (corridors[x, y, 0])
                        zones[x, y].AddPortal(new Portal(zones[x + 1, y], new Vector2(right, (top + bottom) / 2 + 5f),
                                                                          new Vector2(right, (top + bottom) / 2 - 5f)));
                    if (corridors[x, y, 1])
                        zones[x, y].AddPortal(new Portal(zones[x, y + 1], new Vector2((left + right) / 2 - 5f, top),
                                                                          new Vector2((left + right) / 2 + 5f, top)));
                    if (corridors[x, y, 2])
                        zones[x, y].AddPortal(new Portal(zones[x - 1, y], new Vector2(left, (top + bottom) / 2 - 5f),
                                                                          new Vector2(left, (top + bottom) / 2 + 5f)));
                    if (corridors[x, y, 3])
                        zones[x, y].AddPortal(new Portal(zones[x, y - 1], new Vector2((left + right) / 2 + 5f, bottom),
                                                                          new Vector2((left + right) / 2 - 5f, bottom)));
                }
            }

            return scene;
        }
    }
}
