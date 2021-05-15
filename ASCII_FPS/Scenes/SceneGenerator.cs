using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ASCII_FPS.Scenes
{
    public abstract class SceneGenerator
    {
        public const int size = 5;
        public const float tileSize = 100f;

        protected readonly Random rand;

        protected readonly ASCII_FPS game;

        protected Point exitRoom;
        protected bool[,] accessible;
        protected bool[,,] corridorLayout;
        protected float[,,] corridorWidths;

        protected abstract AsciiTexture WallTexture { get; }
        protected abstract AsciiTexture FloorTexture { get; }


        public SceneGenerator(ASCII_FPS game)
        {
            this.game = game;
            rand = new Random();
        }


        public Scene Generate()
        {
            game.PlayerStats.totalMonsters = 0;
            game.PlayerStats.monsters = 0;
            Scene scene = new Scene(game);
            Zone[,] zones = new Zone[size, size];
            scene.Visited = new bool[size, size];

            GenerateLayout(out accessible, out corridorLayout, out corridorWidths);
            scene.CorridorLayout = corridorLayout;

            // Select exit room
            exitRoom = new Point(rand.Next(size), rand.Next(size));
            while (!accessible[exitRoom.X, exitRoom.Y] || (exitRoom.X == size / 2 && exitRoom.Y == size / 2))
            {
                exitRoom = new Point(rand.Next(size), rand.Next(size));
            }
            scene.ExitRoom = exitRoom;

            scene.Collectibles = DistributeCollectibles();

            GenerateRooms(scene, zones);
            GeneratePortals(zones);

            return scene;
        }


        private void GenerateRooms(Scene scene, Zone[,] zones)
        {
            // Generating geometry and colliders
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (accessible[x, y])
                    {
                        float left = x * tileSize - size * tileSize / 2;
                        float right = left + tileSize;
                        float bottom = y * tileSize - size * tileSize / 2;
                        float top = bottom + tileSize;
                        zones[x, y] = new Zone(new RectangleF(left, bottom, tileSize, tileSize));
                        scene.AddZone(zones[x, y]);

                        float[] roomCorridors = Enumerable.Range(0, 4).Select(t => corridorWidths[x, y, t]).ToArray();

                        // Walls
                        List<Vector2[]> walls = SceneGenUtils.MakeRoomWalls(tileSize, tileSize, roomCorridors, 2f);
                        Vector2 roomCenter = new Vector2((left + right) / 2, (top + bottom) / 2);
                        List<Triangle> wallTriangles = new List<Triangle>();
                        foreach (Vector2[] wall in walls)
                        {
                            wallTriangles.AddRange(SceneGenUtils.MakeWall(wall, -4f, 4f, WallTexture));
                            scene.AddObstacle(wall[0] + roomCenter, wall[1] + roomCenter, ObstacleLayer.Wall);
                        }
                        MeshObject wallObject = new MeshObject(wallTriangles, new Vector3(roomCenter.X, 0f, roomCenter.Y), 0f);
                        zones[x, y].AddMesh(wallObject);


                        bool flagIsSpecial = (x == exitRoom.X && y == exitRoom.Y) || (x == size / 2 && y == size / 2);
                        PopulateSchemeFlags flags = new PopulateSchemeFlags
                        {
                            IsSpecial = flagIsSpecial,
                            ClearCenter = scene.Collectibles[x, y] == null && !flagIsSpecial,
                            NotJoint = true,
                            ClearPerimeter = true,
                            ClearFloor = scene.Collectibles[x, y] == null && !flagIsSpecial
                        };

                        for (int t = 0; t < 4; t++)
                        {
                            if (corridorLayout[x, y, t] && corridorWidths[x, y, t] > 15f)
                            {
                                flags.NotJoint = false;
                            }
                        }


                        if (scene.Collectibles[x, y] != null)
                        {
                            Collectible.Type type = scene.Collectibles[x, y].Value;
                            AsciiTexture texture = null;
                            switch (type)
                            {
                                case Collectible.Type.Armor:
                                    texture = Assets.barrelGreenTexture;
                                    break;
                                case Collectible.Type.Health:
                                    texture = Assets.barrelRedTexture;
                                    break;
                                case Collectible.Type.Skill:
                                    texture = Assets.barrelBlueTexture;
                                    break;
                            }
                            MeshObject barrel = new MeshObject(Assets.barrelModel, texture, new Vector3(roomCenter.X, -3f, roomCenter.Y));
                            scene.AddGameObject(new Collectible(barrel, type, x, y));
                        }

                        if (x == exitRoom.X && y == exitRoom.Y)
                        {
                            flags.ClearCenter = false;
                            flags.ClearPerimeter = false;
                            flags.ClearFloor = false;

                            MeshObject exit = new MeshObject(Assets.exitModel, Assets.exitTexture,
                                new Vector3(roomCenter.X, -2f, roomCenter.Y));
                            zones[x, y].AddMesh(exit);
                            game.PlayerStats.exitPosition = new Vector2(roomCenter.X, roomCenter.Y);
                        }


                        PopulateRoomResults results = PopulateRoom(scene, zones[x, y], x, y, flags);

                        if (results.GenerateFloor)
                        {
                            zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, -4f, FloorTexture, true));
                            zones[x, y].AddMesh(SceneGenUtils.MakeFloor(left, right, bottom, top, 4f, FloorTexture, false));
                        }
                    }
                }
            }
        }

        private void GeneratePortals(Zone[,] zones)
        {
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
        }


        protected abstract void GenerateLayout(out bool[,] accessible, out bool[,,] corridorLayout, out float[,,] corridorWidths);

        protected abstract Collectible.Type?[,] DistributeCollectibles();

        protected abstract PopulateRoomResults PopulateRoom(Scene scene, Zone zone, int x, int y, PopulateSchemeFlags flags);

        protected struct PopulateSchemeFlags
        {
            public bool IsSpecial { get; set; }
            public bool ClearCenter { get; set; }
            public bool NotJoint { get; set; }
            public bool ClearPerimeter { get; set; }
            public bool ClearFloor { get; set; }
        }

        protected struct PopulateRoomResults
        {
            public bool GenerateFloor { get; set; }
        }
    }
}
