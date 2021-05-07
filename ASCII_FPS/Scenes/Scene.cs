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
        public Collectible.Type?[,] Collectibles { get; set; }
        public Point ExitRoom { get; set; }

        private ASCII_FPS game;
        private ObstacleSet obstacles;

        public Scene(ASCII_FPS game)
        {
            this.game = game;
            zones = new List<Zone>();
            obstacles = new ObstacleSet();
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
            obstacles.AddObstacle(new Obstacle(v0, v1, layer));
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask)
        {
            return obstacles.CheckMovement(from, direction, radius, layerMask);
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius)
        {
            return CheckMovement(from, direction, radius, ObstacleLayerMask.Everything);
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, int layerMask, out Vector3 normal, out float relativeLength)
        {
            return obstacles.CheckMovement(from, direction, radius, layerMask, out normal, out relativeLength);
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

                    Collectible.Type? type = Collectibles[i, j];
                    writer.Write(type == null ? -1 : (int)type.Value);
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

            obstacles.Save(writer);
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
            scene.Collectibles = new Collectible.Type?[xsize, ysize];
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    scene.Visited[i, j] = reader.ReadBoolean();
                    for (int k = 0; k < 4; k++)
                    {
                        scene.CorridorLayout[i, j, k] = reader.ReadBoolean();
                    }
                    int type = reader.ReadInt32();
                    if (type != -1)
                    {
                        scene.Collectibles[i, j] = (Collectible.Type)type;
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

            scene.obstacles.Load(reader);

            return scene;
        }
    }
}