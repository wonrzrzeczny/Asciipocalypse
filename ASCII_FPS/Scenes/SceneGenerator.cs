using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private float[,,] corridorWidths;


        public SceneGenerator(ASCII_FPS game, int floor)
        {
            this.game = game;
            rand = new Random();
            monsterHP = 8f + floor * 2f;
            monsterDamage = 4f + floor;
            monstersPerRoom = 4 + (int)Math.Floor(floor / 3.0);
        }


        public Scene Generate()
        {
            game.PlayerStats.totalMonsters = 0;
            game.PlayerStats.monsters = 0;
            Scene scene = new Scene(game);

            corridorLayout = SceneGenUtils.GenerateCorridorLayout(size, size);
            scene.CorridorLayout = corridorLayout;
            scene.Visited = new bool[size, size];
            Zone[,] zones = new Zone[size, size];
            corridorWidths = new float[size, size, 4];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int t = 0; t < 4; t++)
                    {
                        if (corridorLayout[x, y, t])
                        {
                            corridorWidths[x, y, t] = 10f;
                        }
                    }
                }
            }


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
                    float[] roomCorridors = new float[4]
                    {
                        corridorWidths[x, y, 0],
                        corridorWidths[x, y, 1],
                        corridorWidths[x, y, 2],
                        corridorWidths[x, y, 3]
                    };

                    List<Vector2[]> walls = SceneGenUtils.MakeRoomWalls(tileSize, tileSize, roomCorridors, 2f);
                    Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);

                    zones[x, y] = new Zone(new RectangleF(left, bottom, tileSize, tileSize));

                    List<Triangle> wallTriangles = new List<Triangle>();
                    foreach (Vector2[] wall in walls)
                    {
                        wallTriangles.AddRange(SceneGenUtils.MakeWall(wall, -4f, 4f, ASCII_FPS.texture1));
                    }
                    MeshObject wallObject = new MeshObject(wallTriangles, new Vector3(roomCenter.X, 0f, roomCenter.Y), 0f);
                    zones[x, y].AddMesh(wallObject);

                    scene.AddZone(zones[x, y]);
                    foreach (Vector2[] wall in walls)
                    {
                        scene.AddObstacle(wall[0] + roomCenter, wall[1] + roomCenter, ObstacleLayer.Wall);
                    }

                    PopulateRoomResults results = PopulateRoom(scene, zones[x, y], x, y);

                    if (results.GenerateFloor)
                    {
                        zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, -4f, true));
                        zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, 4f, false));
                    }
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

                    Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);

                    int dx = 1;
                    int dy = 0;
                    Vector2 wallCenter = new Vector2(tileSize / 2, 0f);
                    Vector2 unit = new Vector2(0, 1);

                    for (int t = 0; t < 4; t++)
                    {
                        if (corridorLayout[x, y, t])
                        {
                            float width = corridorWidths[x, y, t];
                            Vector2 start = roomCenter + wallCenter + unit * width / 2;
                            Vector2 end = roomCenter + wallCenter - unit * width / 2;

                            zones[x, y].AddPortal(new Portal(zones[x + dx, y + dy], start, end));
                        }

                        wallCenter = new Vector2(-wallCenter.Y, wallCenter.X);
                        unit = new Vector2(-unit.Y, unit.X);
                        int tmp = dx;
                        dx = -dy;
                        dy = tmp;
                    }
                }
            }

            return scene;
        }


        private PopulateRoomResults PopulateRoom(Scene scene, Zone zone, int x, int y)
        {
            PopulateRoomResults results = new PopulateRoomResults
            {
                GenerateFloor = true
            };

            PopulateSchemeFlags flags = new PopulateSchemeFlags
            {
                ClearCenter = true,
                NotJoint = true,
                SingleEnemy = true,
                ClearFloor = true
            };

            float left = x * tileSize - size * tileSize / 2;
            float right = left + tileSize;
            float bottom = y * tileSize - size * tileSize / 2;
            float top = bottom + tileSize;
            Vector3 roomCenter = new Vector3((left + right) / 2, 0f, (top + bottom) / 2);

            if (x == exitRoom.X && y == exitRoom.Y)
            {
                flags.ClearCenter = false;
                flags.SingleEnemy = false;
                flags.ClearFloor = false;

                MeshObject exit = new MeshObject(ASCII_FPS.exitModel, ASCII_FPS.exitTexture,
                    new Vector3(roomCenter.X, -2f, roomCenter.Z));
                zone.AddMesh(exit);
                game.PlayerStats.exitPosition = new Vector2(roomCenter.X, roomCenter.Z);
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
                        Vector2 position = new Vector2(roomCenter.X, roomCenter.Z);
                        position += Vector2.Transform(shift, Mathg.RotationMatrix2D(angleOffset + i * (float)Math.PI * 2f / monsterCount));
                        MeshObject monster = PrimitiveMeshes.Tetrahedron(new Vector3(position.X, -1f, position.Y), 3f, ASCII_FPS.monsterTexture);
                        scene.AddGameObject(new Monster(monster, 3f, monsterHP, monsterDamage));
                    }
                }

                if (monsterCount != 1)
                {
                    flags.SingleEnemy = false;

                    if (rand.Next(4) == 0) // barrel
                    {
                        flags.ClearCenter = false;
                        flags.ClearFloor = false;

                        int rnd = rand.Next(6);
                        Collectible.Type type = rnd < 3 ? Collectible.Type.Skill : rnd < 5 ? Collectible.Type.Armor : Collectible.Type.Health;
                        AsciiTexture texture = rnd < 3 ? ASCII_FPS.barrelBlueTexture : rnd < 5 ?
                            ASCII_FPS.barrelGreenTexture : ASCII_FPS.barrelRedTexture;
                        MeshObject barrel = new MeshObject(ASCII_FPS.barrelModel, texture, new Vector3(roomCenter.X, -3f, roomCenter.Z));
                        scene.AddGameObject(new Collectible(barrel, type));
                    }
                }
                else
                {
                    flags.ClearCenter = false;
                }
            }
            else
            {
                flags.ClearCenter = false;
                flags.SingleEnemy = false;
            }

            if (rand.Next(8) == 0 || (flags.SingleEnemy && rand.Next(2) == 0)) // special room
            {
                PopulateSpecialRoom(scene, zone, roomCenter, flags, ref results);
            }
            else
            {
                PopulateRoomCenter(scene, zone, roomCenter, flags, ref results);
                PopulateRoomWalls(scene, zone, roomCenter, flags, ref results);
            }

            return results;
        }

        private void PopulateSpecialRoom(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rng = rand.Next(3);
            if (flags.SingleEnemy)
            {
                if (rng == 0) // arena
                {
                    Vector2[] points = new Vector2[]
                    {
                        new Vector2(27.5f, -15f),
                        new Vector2(32.5f, -15f),
                        new Vector2(32.5f, 15f),
                        new Vector2(27.5f, 15f),
                        new Vector2(27.5f, -15f)
                    };

                    for (int i = 0; i < 4; i++)
                    {
                        SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
                        for (int j = 0; j < points.Length; j++)
                        {
                            points[j] = new Vector2(-points[j].Y, points[j].X);
                        }
                    }
                }
            }
            if (flags.ClearFloor)
            {
                if (rng == 1) // void
                {
                    results.GenerateFloor = false;

                    zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 50f, roomCenter.X - 30f, roomCenter.Z - 50f, roomCenter.Z + 50f, -4f, true));
                    zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X + 30f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, -4f, true));
                    zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z - 50f, roomCenter.Z - 30f, -4f, true));
                    zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z + 30f, roomCenter.Z + 50f, -4f, true));
                    zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 50f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, 4f, false));

                    Vector2[] walls = new Vector2[]
                    {
                        new Vector2(-30f, -30f),
                        new Vector2(-30f, 30f),
                        new Vector2(30f, 30f),
                        new Vector2(30f, -30f),
                        new Vector2(-30f, -30f)
                    };
                    SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { walls }, -20f, -4f, ObstacleLayer.Gap, ASCII_FPS.texture1, roomCenter);
                }
            }
        }

        private void PopulateRoomCenter(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rnd = rand.Next(4);
            if (flags.ClearCenter)
            {
                if (rnd == 0) // square pillar
                {
                    Vector2[] points = new Vector2[]
                    {
                        new Vector2(-5f, -5f),
                        new Vector2(5f, -5f),
                        new Vector2(5f, 5f),
                        new Vector2(-5f, 5f),
                        new Vector2(-5f, -5f)
                    };
                    SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
                }
                else if (rnd == 1) // oct pillar
                {
                    Vector2[] points = new Vector2[]
                    {
                        new Vector2(-4f, -10f),
                        new Vector2(4f, -10f),
                        new Vector2(10f, -4f),
                        new Vector2(10f, 4f),
                        new Vector2(4f, 10f),
                        new Vector2(-4f, 10f),
                        new Vector2(-10f, 4f),
                        new Vector2(-10f, -4f),
                        new Vector2(-4f, -10f)
                    };
                    SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
                }
            }
            else
            {
                if (rnd == 2) // 4 pillars
                {
                    Vector2[] points = new Vector2[]
                    {
                            new Vector2(-15f, -15f),
                            new Vector2(-10f, -15f),
                            new Vector2(-10f, -10f),
                            new Vector2(-15f, -10f),
                            new Vector2(-15f, -15f)
                    };

                    List<Vector2[]> walls = new List<Vector2[]>();
                    for (int x = 0; x < 2; x++)
                    {
                        for (int y = 0; y < 2; y++)
                        {
                            Vector2 shift = new Vector2(x, y) * 25f;
                            Vector2[] wall = points.Select((Vector2 v) => { return v + shift; }).ToArray();
                            walls.Add(wall);
                        }
                    }

                    SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
                }
            }
        }

        private void PopulateRoomWalls(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rnd = rand.Next(4);
            if (flags.NotJoint && rnd == 0) // cut corners
            {
                List<Vector2[]> walls = new List<Vector2[]>
                {
                    new Vector2[] { new Vector2(10f, 50f), new Vector2(50f, 10f) },
                    new Vector2[] { new Vector2(-50f, 10f), new Vector2(-10f, 50f) },
                    new Vector2[] { new Vector2(-10f, -50f), new Vector2(-50f, -10f) },
                    new Vector2[] { new Vector2(50f, -10f), new Vector2(10f, -50f) }
                };
                SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
            }
            else if (rnd == 1) // 4 pillars
            {
                Vector2[] points = new Vector2[]
                {
                        new Vector2(-35f, -35f),
                        new Vector2(-25f, -35f),
                        new Vector2(-25f, -25f),
                        new Vector2(-35f, -25f),
                        new Vector2(-35f, -35f)
                };

                List<Vector2[]> walls = new List<Vector2[]>();
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        Vector2 shift = new Vector2(x, y) * 60f;
                        Vector2[] wall = points.Select((Vector2 v) => { return v + shift; }).ToArray();
                        walls.Add(wall);
                    }
                }

                SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
            }
        }


        private struct PopulateSchemeFlags
        {
            public bool ClearCenter { get; set; }
            public bool NotJoint { get; set; }
            public bool SingleEnemy { get; set; }
            public bool ClearFloor { get; set; }

            public uint Mask
            {
                get
                {
                    uint ret = 0;
                    if (ClearCenter)    ret += 1;
                    if (NotJoint)       ret += 2;
                    if (SingleEnemy)    ret += 4;
                    if (ClearFloor)     ret += 8;
                    return ret;
                }
            }


            public bool Fulfills(PopulateSchemeFlags pred)
            {
                uint mask = Mask;
                return (mask | pred.Mask) == mask;
            }
        }

        private struct PopulateRoomResults
        {
            public bool GenerateFloor { get; set; }
        }
    }
}
