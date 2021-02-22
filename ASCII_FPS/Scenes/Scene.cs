using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ASCII_FPS.Scenes
{
    public class Scene
    {
        public List<Zone> zones;
        public List<GameObject> gameObjects;

        public Camera Camera { get; set; }

        public int TotalTriangles { get; private set; }
        public bool[,,] CorridorLayout { get; set; }
        public bool[,] Visited { get; set; }
        public Point ExitRoom { get; set; }

        private ASCII_FPS game;
        private List<Obstacle> obstacles;

        public Scene(ASCII_FPS game)
        {
            this.game = game;
            zones = new List<Zone>();
            obstacles = new List<Obstacle>();
            gameObjects = new List<GameObject>();
        }

        public void AddZone(Zone zone)
        {
            zones.Add(zone);
            foreach (MeshObject mesh in zone.meshes)
            {
                TotalTriangles += mesh.triangles.Count;
            }
        }

        public void AddObstacle(Vector2 v0, Vector2 v1, ObstacleLayer layer)
        {
            obstacles.Add(new Obstacle(v0, v1, layer));
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

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius)
        {
            return CheckMovement(from, direction, radius, ObstacleLayerMask.Everything);
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

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, out Vector3 normal, out float relativeLength)
        {
            return CheckMovement(from, direction, radius, ObstacleLayerMask.Everything, out normal, out relativeLength);
        }

        // Given start position and movement vector, return the real movement vector taking into account collisions
        public Vector3 SmoothMovement(Vector3 from, Vector3 direction, float radius, int layerMask)
        {
            Vector3 ret = direction;
            while (!CheckMovement(from, ret, radius, layerMask, out Vector3 normal, out float relativeLength))
            {
                relativeLength *= 0.9f;
                if (relativeLength < 0.01f)
                    relativeLength = 0f;
                ret = relativeLength * ret + Mathg.OrthogonalComponent((1f - relativeLength) * ret, normal);
            }
            
            return ret;
        }

        public Vector3 SmoothMovement(Vector3 from, Vector3 direction, float radius)
        {
            return SmoothMovement(from, direction, radius, ObstacleLayerMask.Everything);
        }


        private List<GameObject> gameObjectsToAdd;
        private bool updating = false;
        public void UpdateGameObjects(float deltaTime)
        {
            updating = true;
            gameObjectsToAdd = new List<GameObject>();
            List<GameObject> newGameObjects = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(deltaTime);
                if (!gameObject.Destroy)
                    newGameObjects.Add(gameObject);
            }
            foreach (GameObject gameObject in gameObjectsToAdd)
            {
                newGameObjects.Add(gameObject);
            }
            gameObjects = newGameObjects;
            updating = false;
        }

        public void AddGameObject(GameObject gameObject)
        {
            gameObject.Game = game;

            if (!updating)
            {
                gameObjects.Add(gameObject);
            }
            else
            {
                gameObjectsToAdd.Add(gameObject);
            }
        }



        public void Save(BinaryWriter writer)
        {
            GameSave.WriteVector3(writer, Camera.CameraPos);
            writer.Write(Camera.Rotation);

            writer.Write(ExitRoom.X);
            writer.Write(ExitRoom.Y);
            int xsize = Visited.GetLength(0);
            int ysize = Visited.GetLength(1);
            writer.Write(xsize);
            writer.Write(ysize);
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    writer.Write(Visited[i, j]);
                    for (int k = 0; k < 4; k++)
                    {
                        writer.Write(CorridorLayout[i, j, k]);
                    }
                }
            }

            writer.Write(zones.Count);
            foreach (Zone zone in zones)
            {
                zone.Save(writer);
            }

            writer.Write(gameObjects.Count);
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Save(writer);
            }

            writer.Write(obstacles.Count);
            foreach (Obstacle obstacle in obstacles)
            {
                GameSave.WriteVector2(writer, obstacle.Start);
                GameSave.WriteVector2(writer, obstacle.End);
                writer.Write((int)obstacle.Layer);
            }
        }

        public static Scene Load(BinaryReader reader, ASCII_FPS game)
        {
            Scene scene = new Scene(game);
            scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f)
            {
                CameraPos = GameSave.ReadVector3(reader),
                Rotation = reader.ReadSingle()
            };

            scene.ExitRoom = new Point(reader.ReadInt32(), reader.ReadInt32());
            int xsize = reader.ReadInt32();
            int ysize = reader.ReadInt32();
            scene.Visited = new bool[xsize, ysize];
            scene.CorridorLayout = new bool[xsize, ysize, 4];
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    scene.Visited[i, j] = reader.ReadBoolean();
                    for (int k = 0; k < 4; k++)
                    {
                        scene.CorridorLayout[i, j, k] = reader.ReadBoolean();
                    }
                }
            }

            Zone[] zones = Zone.LoadAll(reader);
            foreach (Zone zone in zones)
            {
                scene.AddZone(zone);
            }

            int gameObjectsCount = reader.ReadInt32();
            for (int i = 0; i < gameObjectsCount; i++)
            {
                scene.AddGameObject(GameObject.Load(reader));
            }

            int wallCount = reader.ReadInt32();
            for (int i = 0; i < wallCount; i++)
            {
                scene.AddObstacle(GameSave.ReadVector2(reader), GameSave.ReadVector2(reader), (ObstacleLayer)reader.ReadInt32());
            }

            return scene;
        }
    }
}