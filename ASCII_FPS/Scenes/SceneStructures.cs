using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.Scenes
{
    public static class SceneStructures
    {
        public delegate void Generator(Scene scene, Zone zone, Vector3 roomCenter);

        public static Generator PillarSmall => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2[] points = new Vector2[]
            {
                new Vector2(-5f, -5f),
                new Vector2(5f, -5f),
                new Vector2(5f, 5f),
                new Vector2(-5f, 5f),
                new Vector2(-5f, -5f)
            };
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };

        public static Generator PillarBig => (Scene scene, Zone zone, Vector3 roomCenter) =>
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
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };

        public static Generator Pillars4Inner => (Scene scene, Zone zone, Vector3 roomCenter) =>
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

            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };


        public static Generator CutCorners => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            List<Vector2[]> walls = new List<Vector2[]>
            {
                new Vector2[] { new Vector2(10f, 50f), new Vector2(50f, 10f) },
                new Vector2[] { new Vector2(-50f, 10f), new Vector2(-10f, 50f) },
                new Vector2[] { new Vector2(-10f, -50f), new Vector2(-50f, -10f) },
                new Vector2[] { new Vector2(50f, -10f), new Vector2(10f, -50f) }
            };
            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };

        public static Generator Pillars4Outer => (Scene scene, Zone zone, Vector3 roomCenter) =>
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

            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };


        public static Generator Arena => (Scene scene, Zone zone, Vector3 roomCenter) =>
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
                SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { points }, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
                for (int j = 0; j < points.Length; j++)
                {
                    points[j] = new Vector2(-points[j].Y, points[j].X);
                }
            }
        };

        public static Generator Pit => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
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
            zone.AddMesh(new MeshObject(SceneGenUtils.MakeWall(walls, -28f, -4f, Assets.texture1), roomCenter, 0f));
            Vector2 offset = new Vector2(roomCenter.X, roomCenter.Z);
            for (int i = 0; i < 4; i++)
            {
                scene.AddObstacle(walls[i + 1] + offset, walls[i] + offset, ObstacleLayer.Gap);
            }
        };

        public static Generator PitPillar => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2[] pillar = new Vector2[]
            {
                new Vector2(-5f, -5f),
                new Vector2(5f, -5f),
                new Vector2(5f, 5f),
                new Vector2(-5f, 5f),
                new Vector2(-5f, -5f)
            };
            SceneGenUtils.AddWalls(scene, zone, new List<Vector2[]> { pillar }, -20f, 4f, ObstacleLayer.Wall, Assets.texture1, roomCenter);
        };
    }
}
