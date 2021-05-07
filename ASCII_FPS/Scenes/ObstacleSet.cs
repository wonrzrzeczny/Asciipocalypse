using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASCII_FPS.Scenes
{
    public class ObstacleSet
    {
        private const float gridCellSize = 100f;

        private class ObstacleChunk
        {
            private readonly List<Obstacle> obstacles;

            public ObstacleChunk()
            {
                obstacles = new List<Obstacle>();
            }


            public void AddObstacle(Obstacle obstacle)
            {
                obstacles.Add(obstacle);
            }

            public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask)
            {
                Vector2 from2 = new Vector2(from.X, from.Z);
                Vector2 direction2 = new Vector2(direction.X, direction.Z);
                Vector2 to2 = from2 + direction2;
                foreach (Obstacle obstacle in obstacles)
                {
                    if (ObstacleLayerMask.CheckMask(layerMask, obstacle.Layer))
                    {
                        Vector2 v0 = obstacle.Start;
                        Vector2 v1 = obstacle.End;
                        Vector2 normal2 = Vector2.Normalize(new Vector2((v1 - v0).Y, -(v1 - v0).X));
                        if (Mathg.Cross2D(v1 - v0, from2 - v0) > 0)
                            normal2 *= -1;
                        v0 += normal2 * radius;
                        v1 += normal2 * radius;

                        if (Mathg.Cross2D(v1 - v0, from2 - v0) * Mathg.Cross2D(v1 - v0, to2 - v0) < 0 &&
                            Mathg.Cross2D(to2 - from2, v0 - from2) * Mathg.Cross2D(to2 - from2, v1 - from2) < 0)
                            return false;
                    }
                }

                return true;
            }

            public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask, out Vector3 normal, out float relativeLength)
            {
                normal = Vector3.Zero;
                relativeLength = float.MaxValue;
                bool ret = true;

                Vector2 from2 = new Vector2(from.X, from.Z);
                Vector2 direction2 = new Vector2(direction.X, direction.Z);
                Vector2 to2 = from2 + direction2;
                foreach (Obstacle obstacle in obstacles)
                {
                    if (ObstacleLayerMask.CheckMask(layerMask, obstacle.Layer))
                    {
                        Vector2 v0 = obstacle.Start;
                        Vector2 v1 = obstacle.End;
                        Vector2 normal2 = Vector2.Normalize(new Vector2((v1 - v0).Y, -(v1 - v0).X));
                        Vector2 orth = new Vector2(normal2.Y, -normal2.X);
                        v0 += (normal2 + orth) * radius;
                        v1 += (normal2 - orth) * radius;

                        if (Mathg.Cross2D(v1 - v0, from2 - v0) * Mathg.Cross2D(v1 - v0, to2 - v0) < 0 &&
                            Mathg.Cross2D(to2 - from2, v0 - from2) * Mathg.Cross2D(to2 - from2, v1 - from2) < 0 &&
                            Vector2.Dot(normal2, Vector2.Normalize(direction2)) < 0.1f)
                        {
                            float t = Vector2.Dot(v0 - from2, normal2) / Vector2.Dot(direction2, normal2);
                            if (t < relativeLength)
                            {
                                relativeLength = t;
                                normal = new Vector3(normal2.X, 0f, normal2.Y);
                            }
                            ret = false;
                        }
                    }
                }

                return ret;
            }
        }

        private readonly List<Obstacle> allObstacles;
        private readonly Dictionary<(int, int), ObstacleChunk> chunks;


        public ObstacleSet()
        {
            allObstacles = new List<Obstacle>();
            chunks = new Dictionary<(int, int), ObstacleChunk>();
        }


        public void AddObstacle(Obstacle obstacle)
        {
            allObstacles.Add(obstacle);
            GetCellsOnPath(obstacle.Start, obstacle.End).ForEach((c) => AddToGridCell(c, obstacle));
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask)
        {
            Vector2 from2 = new Vector2(from.X, from.Z);
            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            Vector2 to2 = from2 + direction2;
            List<(int, int)> cells = GetCellsOnPath(from2, to2);
            foreach ((int, int) cell in cells)
            {
                if (chunks.TryGetValue(cell, out ObstacleChunk chunk))
                {
                    if (!chunk.CheckMovement(from, direction, radius, layerMask))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask, out Vector3 normal, out float relativeLength)
        {
            normal = Vector3.Zero;
            relativeLength = float.MaxValue;

            Vector2 from2 = new Vector2(from.X, from.Z);
            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            Vector2 to2 = from2 + direction2;
            List<(int, int)> cells = GetCellsOnPath(from2, to2);
            foreach ((int, int) cell in cells)
            {
                if (chunks.TryGetValue(cell, out ObstacleChunk chunk))
                {
                    if (!chunk.CheckMovement(from, direction, radius, layerMask, out normal, out relativeLength))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(allObstacles.Count);
            foreach (Obstacle obstacle in allObstacles)
            {
                GameSave.WriteVector2(writer, obstacle.Start);
                GameSave.WriteVector2(writer, obstacle.End);
                writer.Write((int)obstacle.Layer);
            }
        }

        public void Load(BinaryReader reader)
        {
            int wallCount = reader.ReadInt32();
            for (int i = 0; i < wallCount; i++)
            {
                Obstacle obstacle = new Obstacle(GameSave.ReadVector2(reader), GameSave.ReadVector2(reader), (ObstacleLayer)reader.ReadInt32());
                AddObstacle(obstacle);
            }
        }


        private (int, int) FindGridCell(Vector2 point)
        {
            int x = (int)Math.Floor(point.X / gridCellSize);
            int y = (int)Math.Floor(point.Y / gridCellSize);

            return (x, y);
        }

        private List<(int, int)> GetCellsOnPath(Vector2 from, Vector2 to)
        {
            List<(int, int)> ret = new List<(int, int)>();
            (int, int) fromCell = FindGridCell(from);
            (int, int) toCell = FindGridCell(to);
            ret.Add(fromCell);
            if (toCell != fromCell)
            {
                ret.Add(toCell);
            }
            return ret;
        }

        private void AddToGridCell((int, int) cell, Obstacle obstacle)
        {
            if (!chunks.TryGetValue(cell, out ObstacleChunk chunk))
            {
                chunk = new ObstacleChunk();
                chunks.Add(cell, chunk);
            }

            chunk.AddObstacle(obstacle);
        }
    }
}
