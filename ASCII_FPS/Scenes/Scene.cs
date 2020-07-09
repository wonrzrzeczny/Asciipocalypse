﻿using ASCII_FPS.GameComponents;
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

        public int TotalTriangles { get; private set; }

        private List<Vector2[]> walls;
        public Camera Camera { get; set; }

        public Scene()
        {
            zones = new List<Zone>();
            walls = new List<Vector2[]>();
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

        public void AddWall(Vector2 v0, Vector2 v1)
        {
            AddWall(v0.X, v0.Y, v1.X, v1.Y);
        }

        public void AddWall(float x0, float z0, float x1, float z1)
        {
            walls.Add(new Vector2[2] { new Vector2(x0, z0), new Vector2(x1, z1) });
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius)
        {
            Vector2 from2 = new Vector2(from.X, from.Z);
            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            Vector2 to2 = from2 + direction2;
            foreach (Vector2[] wall in walls)
            {
                Vector2 v0 = wall[0];
                Vector2 v1 = wall[1];
                Vector2 normal2 = Vector2.Normalize(new Vector2((v1 - v0).Y, -(v1 - v0).X));
                if (Mathg.Cross2D(v1 - v0, from2 - v0) > 0)
                    normal2 *= -1;
                v0 += normal2 * radius;
                v1 += normal2 * radius;

                if (Mathg.Cross2D(v1 - v0, from2 - v0) * Mathg.Cross2D(v1 - v0, to2 - v0) < 0 &&
                    Mathg.Cross2D(to2 - from2, v0 - from2) * Mathg.Cross2D(to2 - from2, v1 - from2) < 0)
                    return false;
            }

            return true;
        }

        public bool CheckMovement(Vector3 from, Vector3 direction, float radius, out Vector3 normal, out float relativeLength)
        {
            normal = Vector3.Zero;
            relativeLength = float.MaxValue;
            bool ret = true;

            Vector2 from2 = new Vector2(from.X, from.Z);
            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            Vector2 to2 = from2 + direction2;
            foreach (Vector2[] wall in walls)
            {
                Vector2 v0 = wall[0];
                Vector2 v1 = wall[1];
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

            return ret;
        }


        // Given start position and movement vector, return the real movement vector taking into account collisions
        public Vector3 SmoothMovement(Vector3 from, Vector3 direction, float radius)
        {
            Vector3 ret = direction;
            while (!CheckMovement(from, ret, radius, out Vector3 normal, out float relativeLength))
            {
                relativeLength *= 0.9f;
                if (relativeLength < 0.01f)
                    relativeLength = 0f;
                ret = relativeLength * ret + Mathg.OrthogonalComponent((1f - relativeLength) * ret, normal);
            }
            
            return ret;
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
            gameObject.Scene = this;

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

            writer.Write(HUD.exitRoom.X);
            writer.Write(HUD.exitRoom.Y);
            int xsize = HUD.visited.GetLength(0);
            int ysize = HUD.visited.GetLength(1);
            writer.Write(xsize);
            writer.Write(ysize);
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    writer.Write(HUD.visited[i, j]);
                    for (int k = 0; k < 4; k++)
                    {
                        writer.Write(HUD.corridorLayout[i, j, k]);
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

            writer.Write(walls.Count);
            foreach (Vector2[] wall in walls)
            {
                GameSave.WriteVector2(writer, wall[0]);
                GameSave.WriteVector2(writer, wall[1]);
            }
        }

        public static Scene Load(BinaryReader reader)
        {
            Scene scene = new Scene();

            scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f)
            {
                CameraPos = GameSave.ReadVector3(reader),
                Rotation = reader.ReadSingle()
            };

            HUD.exitRoom = new Point(reader.ReadInt32(), reader.ReadInt32());
            int xsize = reader.ReadInt32();
            int ysize = reader.ReadInt32();
            HUD.visited = new bool[xsize, ysize];
            HUD.corridorLayout = new bool[xsize, ysize, 4];
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    HUD.visited[i, j] = reader.ReadBoolean();
                    for (int k = 0; k < 4; k++)
                    {
                        HUD.corridorLayout[i, j, k] = reader.ReadBoolean();
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
                scene.AddWall(GameSave.ReadVector2(reader), GameSave.ReadVector2(reader));
            }

            return scene;
        }
    }
}