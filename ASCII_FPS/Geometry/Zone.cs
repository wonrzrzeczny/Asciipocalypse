using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS
{
    public class Zone
    {
        public RectangleF Bounds { get; }

        public List<MeshObject> meshes;
        public List<Portal> portals;

        public Zone(RectangleF bounds)
        {
            Bounds = bounds;
            meshes = new List<MeshObject>();
            portals = new List<Portal>();
        }

        public void AddMesh(MeshObject mesh)
        {
            meshes.Add(mesh);
        }

        public void AddTriangle(Triangle triangle)
        {
            meshes.Add(new MeshObject(new List<Triangle>(new Triangle[] { triangle })));
        }

        public void AddPortal(Portal portal)
        {
            portals.Add(portal);
        }



        public void Save(BinaryWriter writer)
        {
            writer.Write(Bounds.X);
            writer.Write(Bounds.Y);
            writer.Write(Bounds.Width);
            writer.Write(Bounds.Height);

            writer.Write(meshes.Count);
            foreach (MeshObject mesh in meshes)
            {
                mesh.Save(writer);
            }

            /*writer.Write(portals.Count);
            foreach (Portal portal in portals)
            {
                //writer.Write(portal.)
            }*/
        }

        public static Zone Load(BinaryReader reader)
        {
            RectangleF bounds = new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Zone zone = new Zone(bounds);

            int meshCount = reader.ReadInt32();
            for (int i = 0; i < meshCount; i++)
            {
                zone.AddMesh(MeshObject.Load(reader));
            }

            return zone;
        }
    }
}
