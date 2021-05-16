using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.Scenes
{
    public static class SceneStructures
    {
        public delegate void Generator(Scene scene, Zone zone, Vector3 roomCenter);

        public static Generator PillarSmall(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2[] points = new Vector2[]
            {
                new Vector2(-5f, -5f),
                new Vector2(5f, -5f),
                new Vector2(5f, 5f),
                new Vector2(-5f, 5f),
                new Vector2(-5f, -5f)
            };
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };

        public static Generator PillarBig(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
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
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };

        public static Generator Pillars4Inner(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
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

            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };


        public static Generator CutCorners(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            List<Vector2[]> walls = new List<Vector2[]>
            {
                new Vector2[] { new Vector2(10f, 50f), new Vector2(50f, 10f) },
                new Vector2[] { new Vector2(-50f, 10f), new Vector2(-10f, 50f) },
                new Vector2[] { new Vector2(-10f, -50f), new Vector2(-50f, -10f) },
                new Vector2[] { new Vector2(50f, -10f), new Vector2(10f, -50f) }
            };
            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };

        public static Generator Pillars4Outer(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
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

            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };


        public static Generator Arena(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
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
                SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, texture, roomCenter);
                for (int j = 0; j < points.Length; j++)
                {
                    points[j] = new Vector2(-points[j].Y, points[j].X);
                }
            }
        };

        public static Generator Pit(AsciiTexture floorTexture, AsciiTexture wallTexture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 50f, roomCenter.X - 30f, roomCenter.Z - 50f, roomCenter.Z + 50f, -4f, floorTexture, true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X + 30f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, -4f, floorTexture,  true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z - 50f, roomCenter.Z - 30f, -4f, floorTexture,  true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z + 30f, roomCenter.Z + 50f, -4f, floorTexture,  true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 50f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, 4f, floorTexture,  false));

            Vector2[] walls = new Vector2[]
            {
                new Vector2(-30f, -30f),
                new Vector2(-30f, 30f),
                new Vector2(30f, 30f),
                new Vector2(30f, -30f),
                new Vector2(-30f, -30f)
            };
            zone.AddMesh(new MeshObject(SceneGenUtils.MakeWall(walls, -28f, -4f, wallTexture), roomCenter, 0f));
            Vector2 offset = new Vector2(roomCenter.X, roomCenter.Z);
            for (int i = 0; i < 4; i++)
            {
                scene.AddObstacle(walls[i + 1] + offset, walls[i] + offset, ObstacleLayer.Gap);
            }
        };

        public static Generator PitPillar(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2[] pillar = new Vector2[]
            {
                new Vector2(-5f, -5f),
                new Vector2(5f, -5f),
                new Vector2(5f, 5f),
                new Vector2(-5f, 5f),
                new Vector2(-5f, -5f)
            };
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { pillar }, -20f, 4f, ObstacleLayer.Wall, texture, roomCenter);
        };



        public static Generator JungleBushes() => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) }
                .ForEach(((int, int) p) =>
                {
                    Vector3 position = roomCenter + new Vector3(p.Item1 * 10f, -4f, p.Item2 * 10f);
                    zone.AddMesh(new MeshObject(Assets.bushModel, Assets.bushTexture, position));
                });
        };

        public static Generator JungleWalls(AsciiTexture texture) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2[] wallLow = new Vector2[]
            {
                new Vector2(-50f, 10f),
                new Vector2(-40f, 10f),
                new Vector2(-40f, 40f),
                new Vector2(-10f, 40f),
                new Vector2(-10f, 50f)
            };
            Vector2[] wallHigh = new Vector2[]
            {
                new Vector2(-50f, 15f),
                new Vector2(-45f, 15f),
                new Vector2(-45f, 45f),
                new Vector2(-15f, 45f),
                new Vector2(-15f, 50f)
            };
            for (int i = 0; i < 4; i++)
            {
                SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { wallLow }, -4f, -2f, ObstacleLayer.Gap, texture, roomCenter);
                SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { wallHigh }, -2f, 1f, ObstacleLayer.Wall, texture, roomCenter);
                for (int t = 0; t < wallLow.Length; t++)
                {
                    wallLow[t] = new Vector2(wallLow[t].Y, -wallLow[t].X);
                    wallHigh[t] = new Vector2(wallHigh[t].Y, -wallHigh[t].X);
                }
            }

            new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) }
                .ForEach(((int, int) p) =>
                {
                    int xx = p.Item1;
                    int yy = p.Item2;
                    SceneGenUtils.AddFloor(zone, new Vector2(-50f * xx, 10f * yy), new Vector2(-40f * xx, 50f * yy), -2f, texture, true, roomCenter, 10f);
                    SceneGenUtils.AddFloor(zone, new Vector2(-40f * xx, 40f * yy), new Vector2(-10f * xx, 50f * yy), -2f, texture, true, roomCenter, 10f);
                });
        };
    }
}
