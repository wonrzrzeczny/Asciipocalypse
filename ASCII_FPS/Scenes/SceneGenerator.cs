using ASCII_FPS.GameComponents;
using ASCII_FPS.GameComponents.Enemies;
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
        private float[] monsterChances;

        private Point exitRoom;
        private bool[,] generated;
        private bool[,,] corridorLayout;
        private float[,,] corridorWidths;


        public SceneGenerator(ASCII_FPS game, int floor)
        {
            this.game = game;
            rand = new Random();
            monsterHP = 8f + floor * 2f;
            monsterDamage = 4f + floor;
            monstersPerRoom = 4 + (int)Math.Floor(floor / 3.0);
            monsterChances = new float[]
            {
                floor < 2 ? 0f : 0.3f * (1 - 1 / (0.7f * (floor - 1) + 1f)),
                floor < 3 ? 0f : 0.1f * (1 - 1 / (0.3f * (floor - 2) + 1f)),
                floor < 4 ? 0f : 0.2f * (1 - 1 / (0.4f * (floor - 3) + 1f))
            };
        }


        public Scene Generate()
        {
            game.PlayerStats.totalMonsters = 0;
            game.PlayerStats.monsters = 0;
            Scene scene = new Scene(game);

            corridorLayout = SceneGenUtils.GenerateCorridorLayout(size, size, out generated);
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

            // Select two rooms connected with a corridor to join (5 tries, max 1 joint pair)
            for (int t = 0; t < 5; t++)
            {
                int x = rand.Next(size);
                int y = rand.Next(size);
                int d = rand.Next(4);
                if (corridorLayout[x, y, d] && (x != size / 2 || y != size / 2))
                {
                    Point[] shift = new Point[4] { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
                    if (x + shift[d].X != size / 2 || y + shift[d].Y != size / 2)
                    {
                        corridorWidths[x, y, d] = 80f;
                        corridorWidths[x + shift[d].X, y + shift[d].Y, d ^ 2] = 80f;
                        break;
                    }
                }
            }

            // Select exit room
            exitRoom = new Point(rand.Next(size), rand.Next(size));
            while (!generated[exitRoom.X, exitRoom.Y] || (exitRoom.X == size / 2 && exitRoom.Y == size / 2))
            {
                exitRoom = new Point(rand.Next(size), rand.Next(size));
            }
            scene.ExitRoom = exitRoom;

            // Distribute collectibles (3 x skill point, 2 x armor refill, 1 x hp refill)
            // 10 attempts per each collectible
            scene.Collectibles = new Collectible.Type?[size, size];
            for (int b = 0; b < 6; b++)
            {
                for (int t = 0; t < 10; t++)
                {
                    int x = rand.Next(size);
                    int y = rand.Next(size);
                    if ((x != size / 2 || y != size / 2) && (x != exitRoom.X || y != exitRoom.Y) && generated[x, y] && scene.Collectibles[x, y] == null)
                    {
                        scene.Collectibles[x, y] = b < 3 ? Collectible.Type.Skill
                            : b < 5 ? Collectible.Type.Armor : Collectible.Type.Health;
                        break;
                    }
                }
            }

            // Generating geometry and colliders
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (generated[x, y])
                    {
                        float left = x * tileSize - size * tileSize / 2;
                        float right = left + tileSize;
                        float bottom = y * tileSize - size * tileSize / 2;
                        float top = bottom + tileSize;
                        zones[x, y] = new Zone(new RectangleF(left, bottom, tileSize, tileSize));
                        scene.AddZone(zones[x, y]);

                        float[] roomCorridors = new float[4]
                        {
                            corridorWidths[x, y, 0],
                            corridorWidths[x, y, 1],
                            corridorWidths[x, y, 2],
                            corridorWidths[x, y, 3]
                        };

                        // Walls
                        List<Vector2[]> walls = SceneGenUtils.MakeRoomWalls(tileSize, tileSize, roomCorridors, 2f);
                        Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);
                        List<Triangle> wallTriangles = new List<Triangle>();
                        foreach (Vector2[] wall in walls)
                        {
                            wallTriangles.AddRange(SceneGenUtils.MakeWall(wall, -4f, 4f, ASCII_FPS.texture1));
                            scene.AddObstacle(wall[0] + roomCenter, wall[1] + roomCenter, ObstacleLayer.Wall);
                        }
                        MeshObject wallObject = new MeshObject(wallTriangles, new Vector3(roomCenter.X, 0f, roomCenter.Y), 0f);
                        zones[x, y].AddMesh(wallObject);


                        PopulateRoomResults results = PopulateRoom(scene, zones[x, y], x, y);

                        if (results.GenerateFloor)
                        {
                            zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, -4f, true));
                            zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, 4f, false));
                        }
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

            for (int t = 0; t < 4; t++)
            {
                if (corridorLayout[x, y, t] && corridorWidths[x, y, t] > 15f)
                {
                    flags.NotJoint = false;
                }
            }

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
                int monsterCount = rand.Next(scene.Collectibles[x, y] == null ? 1 : 2, monstersPerRoom + 1);
                game.PlayerStats.totalMonsters += monsterCount;
                Vector2 shift = monsterCount == 1 ? Vector2.Zero : new Vector2(30f, 0f);
                float angleOffset = (float)(rand.NextDouble() * Math.PI * 2f);
                for (int i = 0; i < monsterCount; i++)
                {
                    Vector2 position = new Vector2(roomCenter.X, roomCenter.Z);
                    position += Vector2.Transform(shift, Mathg.RotationMatrix2D(angleOffset + i * (float)Math.PI * 2f / monsterCount));

                    float rnd = (float)rand.NextDouble();
                    if (rnd < monsterChances[0])
                    {
                        scene.AddGameObject(new ShotgunDude(new Vector3(position.X, -1f, position.Y), monsterHP, monsterDamage));
                    }
                    else if (rnd < monsterChances[0] + monsterChances[1])
                    {
                        scene.AddGameObject(new SpinnyBoi(new Vector3(position.X, -1f, position.Y), monsterHP * 2, monsterDamage));
                    }
                    else if (rnd < monsterChances[0] + monsterChances[1] + monsterChances[2])
                    {
                        scene.AddGameObject(new Spooper(new Vector3(position.X, -1f, position.Y), monsterHP * 1.5f, monsterDamage));
                    }
                    else
                    {
                        scene.AddGameObject(new BasicMonster(new Vector3(position.X, -1f, position.Y), monsterHP, monsterDamage));
                    }
                }

                if (monsterCount != 1)
                {
                    flags.SingleEnemy = false;
                }
                else
                {
                    flags.ClearCenter = false;
                }

                if (scene.Collectibles[x, y] != null)
                {
                    flags.ClearCenter = false;
                    flags.ClearFloor = false;

                    Collectible.Type type = scene.Collectibles[x, y].Value;
                    AsciiTexture texture = null;
                    switch (type)
                    {
                        case Collectible.Type.Armor:
                            texture = ASCII_FPS.barrelGreenTexture;
                            break;
                        case Collectible.Type.Health:
                            texture = ASCII_FPS.barrelRedTexture;
                            break;
                        case Collectible.Type.Skill:
                            texture = ASCII_FPS.barrelBlueTexture;
                            break;
                    }
                    MeshObject barrel = new MeshObject(ASCII_FPS.barrelModel, texture, new Vector3(roomCenter.X, -3f, roomCenter.Z));
                    scene.AddGameObject(new Collectible(barrel, type, x, y));
                }
            }
            else
            {
                flags.ClearCenter = false;
                flags.ClearFloor = false;
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
            int rng = rand.Next(6);
            if (flags.SingleEnemy)
            {
                if (rng == 0 || rng == 1) // arena
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
                if (rng == 2 || rng == 3) // void
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
                    zone.AddMesh(new MeshObject(SceneGenUtils.MakeWall(walls, -28f, -4f, ASCII_FPS.texture1), roomCenter, 0f));
                    Vector2 offset = new Vector2(roomCenter.X, roomCenter.Z);
                    for (int i = 0; i < 4; i++)
                    {
                        scene.AddObstacle(walls[i + 1] + offset, walls[i] + offset, ObstacleLayer.Gap);
                    }

                    if (rng == 3 && flags.ClearCenter) // pillar
                    {
                        Vector2[] pillar = new Vector2[]
                        {
                            new Vector2(-5f, -5f),
                            new Vector2(5f, -5f),
                            new Vector2(5f, 5f),
                            new Vector2(-5f, 5f),
                            new Vector2(-5f, -5f)
                        };
                        SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { pillar }, -20f, 4f, ObstacleLayer.Wall, ASCII_FPS.texture1, roomCenter);
                    }
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
