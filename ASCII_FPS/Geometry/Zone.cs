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

            writer.Write(portals.Count);
            foreach (Portal portal in portals)
            {
                GameSave.WriteVector2(writer, portal.Start);
                GameSave.WriteVector2(writer, portal.End);
                GameSave.WriteRectangleF(writer, portal.Zone.Bounds);
            }
        }

        // This one needs to be rewritten :/
        public static Zone[] LoadAll(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            Zone[] zones = new Zone[count];
            List<RectangleF>[] portals = new List<RectangleF>[count];

            for (int i = 0; i < count; i++)
            {
                RectangleF bounds = new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Zone zone = new Zone(bounds);

                int meshCount = reader.ReadInt32();
                for (int j = 0; j < meshCount; j++)
                {
                    zone.AddMesh(MeshObject.Load(reader));
                }

                int portalCount = reader.ReadInt32();
                portals[i] = new List<RectangleF>();
                for (int j = 0; j < portalCount; j++)
                {
                    Vector2 start = GameSave.ReadVector2(reader);
                    Vector2 end = GameSave.ReadVector2(reader);
                    zone.AddPortal(new Portal(null, start, end));
                    portals[i].Add(GameSave.ReadRectangleF(reader));
                }

                zones[i] = zone;
            }
            
            for (int i = 0; i < count; i++)
            {
                int portalCount = zones[i].portals.Count;
                for (int j = 0; j < portalCount; j++)
                {
                    RectangleF bounds = portals[i][j];
                    Zone zone = zones.Where((Zone z) => { return z.Bounds.Equals(bounds); }).First();
                    Vector2 start = zones[i].portals[j].Start;
                    Vector2 end = zones[i].portals[j].End;
                    zones[i].portals[j] = new Portal(zone, start, end);
                }
            }

            return zones;
        }
    }
}
