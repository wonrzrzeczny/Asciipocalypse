using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
    }
}
