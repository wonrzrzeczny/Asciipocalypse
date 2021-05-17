﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.Scenes
{
    public static class SceneStructures
    {
        public delegate void Generator(Scene scene, Zone zone, Vector3 roomCenter);

        public static Generator PillarSmall(AsciiTexture texture, Vector2 center, Vector2 size) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            List<Vector2[]> walls = SceneGenUtils.MakeRect(-size.X / 2f, size.X / 2f, -size.Y / 2f, size.Y / 2f);
            Vector3 offset = roomCenter + new Vector3(center.X, 0f, center.Y);
            SceneGenUtils.AddWalls(scene, zone, walls, 4f, ObstacleLayer.Wall, texture, offset);
        };

        public static Generator PillarSmall(AsciiTexture texture) => PillarSmall(texture, Vector2.Zero, 10f * Vector2.One);

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
            PillarSmall(texture, new Vector2(-12.5f, -12.5f), 5f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(12.5f, -12.5f), 5f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(-12.5f, 12.5f), 5f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(12.5f, 12.5f), 5f * Vector2.One).Invoke(scene, zone, roomCenter);
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
            PillarSmall(texture, new Vector2(-30f, -30f), 10f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(30f, -30f), 10f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(-30f, 30f), 10f * Vector2.One).Invoke(scene, zone, roomCenter);
            PillarSmall(texture, new Vector2(30f, 30f), 10f * Vector2.One).Invoke(scene, zone, roomCenter);
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
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X + 30f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, -4f, floorTexture, true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z - 50f, roomCenter.Z - 30f, -4f, floorTexture, true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 30f, roomCenter.X + 30f, roomCenter.Z + 30f, roomCenter.Z + 50f, -4f, floorTexture, true));
            zone.AddMesh(SceneGenUtils.MakeFloor(roomCenter.X - 50f, roomCenter.X + 50f, roomCenter.Z - 50f, roomCenter.Z + 50f, 4f, floorTexture, false));

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

        public static Generator JungleWalls(AsciiTexture texture, float widthLow, float widthHigh, float[] corridors) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            float[] corridorsLow = corridors.Select(t => t > 0f ? t + 10f : t).ToArray();
            float[] corridorsHigh = corridors.Select(t => t > 0f ? t + 10f + 2f * (widthLow - widthHigh) : t).ToArray();
            List<Vector2[]> wallsLow = SceneGenUtils.MakeRoomWalls(SceneGenerator.tileSize, SceneGenerator.tileSize, corridorsLow, widthLow * 2f);
            List<Vector2[]> wallsHigh = SceneGenUtils.MakeRoomWalls(SceneGenerator.tileSize, SceneGenerator.tileSize, corridorsHigh, widthHigh * 2f);
            SceneGenUtils.AddWalls(scene, zone, wallsLow, -4f, -2f, ObstacleLayer.Gap, texture, roomCenter);
            SceneGenUtils.AddWalls(scene, zone, wallsHigh, -2f, 1f, ObstacleLayer.Wall, texture, roomCenter);

            new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) }
                .ForEach(((int, int) p) =>
                {
                    int xx = p.Item1;
                    int yy = p.Item2;
                    SceneGenUtils.AddFloor(zone, new Vector2(-50f * xx, 10f * yy), new Vector2((-50f + widthLow) * xx, 50f * yy), -2f, texture, true, roomCenter, 10f);
                    SceneGenUtils.AddFloor(zone, new Vector2((-50f + widthLow) * xx, (50f - widthLow) * yy), new Vector2(-10f * xx, 50f * yy), -2f, texture, true, roomCenter, 10f);
                });

            for (int i = 0; i < 4; i++)
            {
                if (corridors[i] == 0f)
                {
                    Vector2 v0 = new Vector2(i == 0 ? 50f : i == 2 ? -50f : -10f,
                                             i == 1 ? 50f : i == 3 ? -50f : -10f);
                    Vector2 v1 = new Vector2(i == 0 ? 50f - widthLow : i == 2 ? -50f + widthLow : 10f,
                                             i == 1 ? 50f - widthLow : i == 3 ? -50f + widthLow : 10f);
                    SceneGenUtils.AddFloor(zone, v0, v1, -2f, texture, true, roomCenter, 10f);
                }
            }
        };

        public static Generator FancyPillar(AsciiTexture texture, Vector2 offset, Vector2 sizeBase, Vector2 sizeCenter)
            => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            List<Vector2[]> baseWall = SceneGenUtils.MakeRect(offset.X - sizeBase.X / 2f, offset.X + sizeBase.X / 2f,
                                                              offset.Y - sizeBase.Y / 2f, offset.Y + sizeBase.Y / 2f);
            List<Vector2[]> centerWall = SceneGenUtils.MakeRect(offset.X - sizeCenter.X / 2f, offset.X + sizeCenter.X / 2f,
                                                                offset.Y - sizeCenter.Y / 2f, offset.Y + sizeCenter.Y / 2f);
            SceneGenUtils.AddWalls(scene, zone, baseWall, -4f, -2.5f, ObstacleLayer.Gap, texture, roomCenter);
            SceneGenUtils.AddWalls(scene, zone, baseWall, 2.5f, 4f, ObstacleLayer.Gap, texture, roomCenter);
            SceneGenUtils.AddWalls(scene, zone, centerWall, -2.5f, 2.5f, ObstacleLayer.Wall, texture, roomCenter);
            SceneGenUtils.AddFloor(zone, offset - sizeBase / 2f, offset + sizeBase / 2f, -2.5f, texture, true, roomCenter, 7f);
            SceneGenUtils.AddFloor(zone, offset - sizeBase / 2f, offset + sizeBase / 2f, 2.5f, texture, false, roomCenter, 7f);
        };

        public static Generator FancyPillar(AsciiTexture texture) => FancyPillar(texture, Vector2.Zero, 14f * Vector2.One, 8f * Vector2.One);

        public static Generator FancyPillars2(AsciiTexture texture, int orientation) => (Scene scene, Zone zone, Vector3 roomCenter) =>
        {
            Vector2 c0 = orientation == 0 ? new Vector2(0f, 10f) : new Vector2(10f, 0f);
            Vector2 c1 = -c0;
            Vector2 sizeBase = orientation == 0 ? new Vector2(28f, 8f) : new Vector2(8f, 28f);
            Vector2 sizeCenter = orientation == 0 ? new Vector2(25f, 5f) : new Vector2(5f, 25f);
            FancyPillar(texture, c0, sizeBase, sizeCenter).Invoke(scene, zone, roomCenter);
            FancyPillar(texture, c1, sizeBase, sizeCenter).Invoke(scene, zone, roomCenter);
        };
    }
}
