using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ASCII_FPS.Scenes
{
    public class SceneGenerator
    {
        public const int size = 5;
        public const float tileSize = 100f;

        private Random rand;

        private ASCII_FPS game;

        private float monsterHP;
        private float monsterDamage;
        private int monstersPerRoom;

        private Point exitRoom;
        private bool[,,] corridorLayout;


        public SceneGenerator(ASCII_FPS game, int floor)
        {
            this.game = game;
            rand = new Random();
            monsterHP = 8f + floor * 2f;
            monsterDamage = 4f + floor;
            monstersPerRoom = 4 + (int)Math.Floor(floor / 3.0);
        }


        private MeshObject MakeFloor(float left, float right, float bottom, float top, float roomHeight)
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

        private List<Vector2[]> MakeRoomWalls(float width, float height, bool[] corridors, float wallThickness, float corridorWidth)
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


        private void GenerateCorridorLayout(int sizeX, int sizeY)
        {
            Random rand = new Random();
            corridorLayout = new bool[sizeX, sizeY, 4];
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
                        corridorLayout[p.X, p.Y, dir[i]] = true;
                        corridorLayout[q.X, q.Y, (dir[i] + 2) % 4] = true;
                        BFSqueue.Enqueue(q);
                    }
                }
            }
        }


        private void PopulateRoom(Scene scene, Zone zone, int x, int y)
        {
            float left = x * tileSize - size * tileSize / 2;
            float right = left + tileSize;
            float bottom = y * tileSize - size * tileSize / 2;
            float top = bottom + tileSize;
            Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);

            if (x == exitRoom.X && y == exitRoom.Y)
            {
                MeshObject exit = new MeshObject(ASCII_FPS.exitModel, ASCII_FPS.exitTexture,
                    new Vector3(roomCenter.X, -2f, roomCenter.Y));
                zone.AddMesh(exit);
                game.PlayerStats.exitPosition = roomCenter;
            }
            else if (x != size / 2 || y != size / 2)
            {
                int monsterCount = rand.Next(1, monstersPerRoom + 1);
                { // monsters
                    game.PlayerStats.totalMonsters += monsterCount;
                    Vector2 shift = monsterCount == 1 ? Vector2.Zero : new Vector2(30f, 0f);
                    float angleOffset = (float)(rand.NextDouble() * Math.PI * 2f);
                    for (int i = 0; i < monsterCount; i++)
                    {
                        Vector2 position = roomCenter + Vector2.Transform(shift, Mathg.RotationMatrix2D(angleOffset + i * (float)Math.PI * 2f / monsterCount));
                        MeshObject monster = PrimitiveMeshes.Tetrahedron(new Vector3(position.X, -1f, position.Y), 3f, ASCII_FPS.monsterTexture);
                        scene.AddGameObject(new Monster(monster, 3f, monsterHP, monsterDamage));
                    }
                }

                if (monsterCount != 1)
                {
                    if (rand.Next(4) == 0) // barrel
                    {
                        int rnd = rand.Next(6);
                        Collectible.Type type = rnd < 3 ? Collectible.Type.Skill : rnd < 5 ? Collectible.Type.Armor : Collectible.Type.Health;
                        AsciiTexture texture = rnd < 3 ? ASCII_FPS.barrelBlueTexture : rnd < 5 ?
                            ASCII_FPS.barrelGreenTexture : ASCII_FPS.barrelRedTexture;
                        MeshObject barrel = new MeshObject(ASCII_FPS.barrelModel, texture, new Vector3(roomCenter.X, -3f, roomCenter.Y));
                        scene.AddGameObject(new Collectible(barrel, type));
                    }
                    else if (rand.Next(2) == 0) // pillar
                    {
                        Vector3 offset = new Vector3(roomCenter.X, 0f, roomCenter.Y);
                        Vector2[] points = new Vector2[]
                        {
                            new Vector2(-5f, -5f),
                            new Vector2(5f, -5f),
                            new Vector2(5f, 5f),
                            new Vector2(-5f, 5f),
                            new Vector2(-5f, -5f)
                        };
                        SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ASCII_FPS.texture1, offset);
                    }
                }

                if (rand.Next(3) == 0) // cut corners
                {
                    Vector3 offset = new Vector3(roomCenter.X, 0f, roomCenter.Y);
                    List<Vector2[]> walls = new List<Vector2[]>
                    {
                        new Vector2[] { new Vector2(10f, 50f), new Vector2(50f, 10f) },
                        new Vector2[] { new Vector2(-50f, 10f), new Vector2(-10f, 50f) },
                        new Vector2[] { new Vector2(-10f, -50f), new Vector2(-50f, -10f) },
                        new Vector2[] { new Vector2(50f, -10f), new Vector2(10f, -50f) }
                    };
                    SceneGenUtils.AddWalls(scene, zone, walls, 4f, ASCII_FPS.texture1, offset);
                }
            }
        }


        public Scene Generate()
        {
            game.PlayerStats.totalMonsters = 0;
            game.PlayerStats.monsters = 0;
            Scene scene = new Scene(game);

            GenerateCorridorLayout(size, size);
            scene.CorridorLayout = corridorLayout;
            scene.Visited = new bool[size, size];
            Zone[,] zones = new Zone[size, size];


            exitRoom = new Point(rand.Next(size), rand.Next(size));
            while (exitRoom.X == size / 2 && exitRoom.Y == size / 2)
            {
                exitRoom = new Point(rand.Next(size), rand.Next(size));
            }
            scene.ExitRoom = exitRoom;


            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float left = x * tileSize - size * tileSize / 2;
                    float right = left + tileSize;
                    float bottom = y * tileSize - size * tileSize / 2;
                    float top = bottom + tileSize;
                    bool[] roomCorridors = new bool[4]
                    { 
                        corridorLayout[x, y, 0],
                        corridorLayout[x, y, 1],
                        corridorLayout[x, y, 2],
                        corridorLayout[x, y, 3]
                    };
                    List<Vector2[]> walls = MakeRoomWalls(tileSize, tileSize, roomCorridors, 2f, 10f);
                    Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);

                    zones[x, y] = new Zone(new RectangleF(left, bottom, tileSize, tileSize));
                    zones[x, y].AddMesh(MakeFloor(left, right, bottom, top, 4f));

                    List<Triangle> wallTriangles = new List<Triangle>();
                    foreach (Vector2[] wall in walls)
                    {
                        wallTriangles.AddRange(SceneGenUtils.MakeWall(wall, 4f, ASCII_FPS.texture1));
                    }
                    MeshObject wallObject = new MeshObject(wallTriangles, new Vector3(roomCenter.X, 0f, roomCenter.Y), 0f);
                    zones[x, y].AddMesh(wallObject);
                    
                    scene.AddZone(zones[x, y]);
                    foreach (Vector2[] wall in walls)
                    {
                        scene.AddWall(wall[0] + roomCenter, wall[1] + roomCenter);
                    }

                    PopulateRoom(scene, zones[x, y], x, y);
                }
            }


            // Create portals between adjacent zones
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float left = x * tileSize - size * tileSize / 2;
                    float right = left + tileSize;
                    float bottom = y * tileSize - size * tileSize / 2;
                    float top = bottom + tileSize;

                    if (corridorLayout[x, y, 0])
                        zones[x, y].AddPortal(new Portal(zones[x + 1, y], new Vector2(right, (top + bottom) / 2 + 5f),
                                                                          new Vector2(right, (top + bottom) / 2 - 5f)));
                    if (corridorLayout[x, y, 1])
                        zones[x, y].AddPortal(new Portal(zones[x, y + 1], new Vector2((left + right) / 2 - 5f, top),
                                                                          new Vector2((left + right) / 2 + 5f, top)));
                    if (corridorLayout[x, y, 2])
                        zones[x, y].AddPortal(new Portal(zones[x - 1, y], new Vector2(left, (top + bottom) / 2 - 5f),
                                                                          new Vector2(left, (top + bottom) / 2 + 5f)));
                    if (corridorLayout[x, y, 3])
                        zones[x, y].AddPortal(new Portal(zones[x, y - 1], new Vector2((left + right) / 2 + 5f, bottom),
                                                                          new Vector2((left + right) / 2 - 5f, bottom)));
                }
            }

            return scene;
        }
    }
}
