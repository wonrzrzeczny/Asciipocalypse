using ASCII_FPS.GameComponents;
using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ASCII_FPS
{
    public class Rasterizer
    {
        private Console console;
        private float[,] offset; // depth offset

        private float[,] zBuffer;
        private Triangle[,] tBuffer; // triangle to be rendered at pixel
        private Vector3[,] bBuffer; // perspective correct barycentric coordinates for triangle verticex attribute interpolation
        private Random rand;

        private const string fogString = "@&#8x*,:. ";

        public Rasterizer(Console console)
        {
            rand = new Random();
            this.console = console;
            zBuffer = new float[console.Width, console.Height];
            tBuffer = new Triangle[console.Width, console.Height];
            bBuffer = new Vector3[console.Width, console.Height];
            offset = new float[console.Width, console.Height];
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    offset[i, j] = (float)rand.NextDouble() - 0.5f;
                }
            }
        }

        public void Raster(Scene scene)
        {
            Camera camera = scene.Camera;

            // Reset console
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    console.Data[i, j] = ' ';
                    console.Color[i, j] = 255;
                    zBuffer[i, j] = 1;
                    bBuffer[i, j] = Vector3.Zero;
                    tBuffer[i, j] = null;
                }
            }

            List<Triangle> triangles = new List<Triangle>();
            ASCII_FPS.triangleCount = 0;
            ASCII_FPS.triangleCountClipped = 0;
            ASCII_FPS.zonesRendered = 0;

            // Extract dynamic meshes
            Matrix cameraSpaceMatrix = camera.CameraSpaceMatrix;
            foreach (GameObject gameObject in scene.gameObjects)
            {
                MeshObject mesh = gameObject.MeshObject;
                ASCII_FPS.triangleCount += mesh.triangles.Count;

                Matrix meshToCameraMatrix = mesh.WorldSpaceMatrix * cameraSpaceMatrix;
                foreach (Triangle triangle in mesh.triangles)
                {
                    Vector3 v0 = Vector3.Transform(triangle.V0, meshToCameraMatrix);
                    Vector3 v1 = Vector3.Transform(triangle.V1, meshToCameraMatrix);
                    Vector3 v2 = Vector3.Transform(triangle.V2, meshToCameraMatrix);
                    triangles.Add(new Triangle(v0, v1, v2, triangle.Texture, triangle.UV0, triangle.UV1, triangle.UV2));
                }
            }
            
            // Clipping
            triangles = ClipTriangles(triangles, camera);
            ASCII_FPS.triangleCountClipped += triangles.Count;

            // Rendering
            RenderTriangles(triangles, camera, 0, console.Width);


            // Find first zone
            Zone firstZone = null;
            foreach (Zone zone in scene.zones)
            {
                if (zone.Bounds.TestPoint(new Vector2(camera.CameraPos.X, camera.CameraPos.Z)))
                {
                    firstZone = zone;
                    break;
                }
            }

            if (firstZone != null)
            {
                ProcessZone(camera, firstZone, 0, console.Width);
            }


            // Shading
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    Triangle triangle = tBuffer[i, j];

                    if (triangle != null)
                    {
                        float z = zBuffer[i, j];
                        Vector3 bar = bBuffer[i, j];

                        int fogId = (z < 0) ? 0 : Math.Min((int)(Math.Pow(z, 10) * fogString.Length + offset[i, j]), fogString.Length - 1);
                        console.Data[i, j] = fogString[fogId];

                        // Sample from texture
                        Vector2 uv = bar.X * triangle.UV0 + bar.Y * triangle.UV1 + bar.Z * triangle.UV2;
                        console.Color[i, j] = Mathg.ColorTo8Bit(triangle.Texture.Sample(uv));
                    }
                }
            }
        }



        // Clipping triangles with near, left and right plane - warning, very ugly code ahead
        public List<Triangle> ClipTriangles(List<Triangle> triangles, Camera camera)
        {
            Vector3[] planeNormals = new Vector3[] { camera.LeftPlane, -Vector3.Forward, camera.RightPlane, camera.BottomPlane, camera.TopPlane };
            Vector3[] planeOffsets = new Vector3[] { Vector3.Zero, -camera.Near * Vector3.Forward, Vector3.Zero, Vector3.Zero, Vector3.Zero };

            List<Triangle> trianglesIn = new List<Triangle>(triangles);
            for (int i = 0; i < planeNormals.Length; i++)
            {
                Vector3 normal = planeNormals[i];
                Vector3 offset = planeOffsets[i];

                List<Triangle> trianglesOut = new List<Triangle>();

                foreach (Triangle triangle in trianglesIn)
                {
                    if (Vector3.Dot(triangle.V0, triangle.Normal) < 0f) // backface culling 
                    {
                        bool s0 = Vector3.Dot(triangle.V0 - offset, normal) > 0;
                        bool s1 = Vector3.Dot(triangle.V1 - offset, normal) > 0;
                        bool s2 = Vector3.Dot(triangle.V2 - offset, normal) > 0;

                        float t0 = Vector3.Dot(offset - triangle.V2, normal) / Vector3.Dot(triangle.V1 - triangle.V2, normal);
                        float t1 = Vector3.Dot(offset - triangle.V0, normal) / Vector3.Dot(triangle.V2 - triangle.V0, normal);
                        float t2 = Vector3.Dot(offset - triangle.V1, normal) / Vector3.Dot(triangle.V0 - triangle.V1, normal);

                        Vector3 v0 = t0 * triangle.V1 + (1 - t0) * triangle.V2; // intersection with v1--v2
                        Vector3 v1 = t1 * triangle.V2 + (1 - t1) * triangle.V0; // intersection with v2--v0
                        Vector3 v2 = t2 * triangle.V0 + (1 - t2) * triangle.V1; // intersection with v0--v1

                        Vector2 uv0 = t0 * triangle.UV1 + (1 - t0) * triangle.UV2; // uv corresponding to v0
                        Vector2 uv1 = t1 * triangle.UV2 + (1 - t1) * triangle.UV0; // uv corresponding to v1
                        Vector2 uv2 = t2 * triangle.UV0 + (1 - t2) * triangle.UV1; // uv corresponding to v2

                        if (s0 && s1 && s2) // fully beyond
                            trianglesOut.Add(triangle);
                        else if (s0 && !s1 && !s2) // one vertex beyond
                            trianglesOut.Add(new Triangle(triangle.V0, v2, v1, triangle.Texture, triangle.UV0, uv2, uv1));
                        else if (s1 && !s2 && !s0)
                            trianglesOut.Add(new Triangle(triangle.V1, v0, v2, triangle.Texture, triangle.UV1, uv0, uv2));
                        else if (s2 && !s0 && !s1)
                            trianglesOut.Add(new Triangle(triangle.V2, v1, v0, triangle.Texture, triangle.UV2, uv1, uv0));
                        else if (!s0 && s1 && s2) // two vertices beyond
                        {
                            trianglesOut.Add(new Triangle(triangle.V2, v1, triangle.V1, triangle.Texture, triangle.UV2, uv1, triangle.UV1));
                            trianglesOut.Add(new Triangle(v1, v2, triangle.V1, triangle.Texture, uv1, uv2, triangle.UV1));
                        }
                        else if (!s1 && s2 && s0)
                        {
                            trianglesOut.Add(new Triangle(triangle.V0, v2, triangle.V2, triangle.Texture, triangle.UV0, uv2, triangle.UV2));
                            trianglesOut.Add(new Triangle(v2, v0, triangle.V2, triangle.Texture, uv2, uv0, triangle.UV2));
                        }
                        else if (!s2 && s0 && s1)
                        {
                            trianglesOut.Add(new Triangle(triangle.V1, v0, triangle.V0, triangle.Texture, triangle.UV1, uv0, triangle.UV0));
                            trianglesOut.Add(new Triangle(v0, v1, triangle.V0, triangle.Texture, uv0, uv1, triangle.UV0));
                        }
                    }
                }

                trianglesIn = trianglesOut;
            }

            return trianglesIn;
        }

        // Render triangles, only for pixels with x in [boundsLeft, boundsRight)
        private void RenderTriangles(List<Triangle> triangles, Camera camera, int boundsLeft, int boundsRight)
        {
            Matrix projectionMatrix = camera.ProjectionMatrix;
            foreach (Triangle triangle in triangles)
            {
                Vector4 v0 = Vector4.Transform(new Vector4(triangle.V0, 1), projectionMatrix);
                Vector4 v1 = Vector4.Transform(new Vector4(triangle.V1, 1), projectionMatrix);
                Vector4 v2 = Vector4.Transform(new Vector4(triangle.V2, 1), projectionMatrix);

                Vector2 p0 = new Vector2(v0.X, -v0.Y) / v0.W;
                Vector2 p1 = new Vector2(v1.X, -v1.Y) / v1.W;
                Vector2 p2 = new Vector2(v2.X, -v2.Y) / v2.W;

                float z0 = v0.Z / v0.W;
                float z1 = v1.Z / v1.W;
                float z2 = v2.Z / v2.W;

                float minX = Math.Min(p0.X, Math.Min(p1.X, p2.X));
                float maxX = Math.Max(p0.X, Math.Max(p1.X, p2.X));
                float minY = Math.Min(p0.Y, Math.Min(p1.Y, p2.Y));
                float maxY = Math.Max(p0.Y, Math.Max(p1.Y, p2.Y));
                int minI = Math.Max(boundsLeft, (int)((minX + 1f) * 0.5f * console.Width));
                int maxI = Math.Min(boundsRight, (int)((maxX + 1f) * 0.5f * console.Width) + 1);
                int minJ = Math.Max(0, (int)((minY + 1f) * 0.5f * console.Height));
                int maxJ = Math.Min(console.Height, (int)((maxY + 1f) * 0.5f * console.Height) + 1);

                // Four corners of rectangle
                Vector2 topLeft = new Vector2(2f * minI / console.Width - 1f, 2f * minJ / console.Height - 1f);
                Vector2 bottomLeft = new Vector2(2f * minI / console.Width - 1f, 2f * maxJ / console.Height - 1f);
                Vector2 topRight = new Vector2(2f * maxI / console.Width - 1f, 2f * minJ / console.Height - 1f);
                Vector2 bottomRight = new Vector2(2f * maxI / console.Width - 1f, 2f * maxJ / console.Height - 1f);

                // Barycentric coordinates of corners
                Vector3 barTopLeft = Mathg.Barycentric(topLeft, p0, p1, p2);
                Vector3 barBottomLeft = Mathg.Barycentric(bottomLeft, p0, p1, p2);
                Vector3 barTopRight = Mathg.Barycentric(topRight, p0, p1, p2);
                Vector3 barBottomRight = Mathg.Barycentric(bottomRight, p0, p1, p2);

                for (int i = minI; i < maxI; i++)
                {
                    // Barycentric coordinates of points on top and bottom edge, interpolated from corners
                    float t = (float)(i - minI) / (maxI - minI);
                    Vector3 barTop = barTopLeft * (1 - t) + barTopRight * t;
                    Vector3 barBottom = barBottomLeft * (1 - t) + barBottomRight * t;

                    for (int j = minJ; j < maxJ; j++)
                    {
                        // Barycentric coordinates of point (i, j), interpolated from top and bottom
                        float t2 = (float)(j - minJ) / (maxJ - minJ);
                        Vector3 bar = barTop * (1 - t2) + barBottom * t2;

                        if (bar.X >= -0.01f && bar.Y >= -0.01f && bar.Z >= -0.01f)
                        {
                            float z = z0 * bar.X + z1 * bar.Y + z2 * bar.Z;

                            if (z > -1 && z < 1 && z < zBuffer[i, j])
                            {
                                zBuffer[i, j] = z;
                                tBuffer[i, j] = triangle;
                                bBuffer[i, j] = new Vector3(bar.X / v0.W, bar.Y / v1.W, bar.Z / v2.W) 
                                    / (bar.X / v0.W + bar.Y / v1.W + bar.Z / v2.W);
                            }
                        }
                    }
                }
            }
        }

        // Render given zone with given bounds and analyze portals leading out of it
        private void ProcessZone(Camera camera, Zone zone, int boundsLeft, int boundsRight)
        {
            ASCII_FPS.zonesRendered++;
            List<Triangle> triangles = new List<Triangle>();

            // Extract meshes
            Matrix cameraSpaceMatrix = camera.CameraSpaceMatrix;
            foreach (MeshObject mesh in zone.meshes)
            {
                ASCII_FPS.triangleCount += mesh.triangles.Count;

                Matrix meshToCameraMatrix = mesh.WorldSpaceMatrix * cameraSpaceMatrix;
                foreach (Triangle triangle in mesh.triangles)
                {
                    Vector3 v0 = Vector3.Transform(triangle.V0, meshToCameraMatrix);
                    Vector3 v1 = Vector3.Transform(triangle.V1, meshToCameraMatrix);
                    Vector3 v2 = Vector3.Transform(triangle.V2, meshToCameraMatrix);
                    triangles.Add(new Triangle(v0, v1, v2, triangle.Texture, triangle.UV0, triangle.UV1, triangle.UV2));
                }
            }

            // Clipping
            triangles = ClipTriangles(triangles, camera);
            ASCII_FPS.triangleCountClipped += triangles.Count;

            // Rendering
            RenderTriangles(triangles, camera, boundsLeft, boundsRight);


            // Check for zones connected with portals
            foreach (Portal portal in zone.portals)
            {
                // Transform into camera space
                Vector3 start = Vector3.Transform(new Vector3(portal.Start.X, 0f, portal.Start.Y), cameraSpaceMatrix);
                Vector3 end = Vector3.Transform(new Vector3(portal.End.X, 0f, portal.End.Y), cameraSpaceMatrix);
                Vector3 normal = Vector3.TransformNormal(portal.Normal, cameraSpaceMatrix);

                if (Vector3.Dot(normal, start) < -1e-3 // Portal is facing camera
                    && (start.Z > 1e-3 || end.Z > 1e-3)) // Portal is in front of the camera
                {
                    int newBoundsLeft = boundsLeft;
                    int newBoundsRight = boundsRight;

                    if (Vector3.Dot(camera.LeftPlane, start) > 0)
                    {
                        float xProjected = camera.FocalLength * start.X / start.Z;
                        newBoundsLeft = Math.Max(newBoundsLeft, (int)Math.Floor((xProjected + 1f) * 0.5f * console.Width));
                    }

                    if (Vector3.Dot(camera.RightPlane, end) > 0)
                    {
                        float xProjected = camera.FocalLength * end.X / end.Z;
                        newBoundsRight = Math.Min(newBoundsRight, (int)Math.Ceiling((xProjected + 1f) * 0.5f * console.Width));
                    }

                    // If the other side of a portal is visible then call recursively
                    if (newBoundsLeft < newBoundsRight)
                    {
                        ProcessZone(camera, portal.Zone, newBoundsLeft, newBoundsRight);
                    }
                }
            }
        }
    }
}
 