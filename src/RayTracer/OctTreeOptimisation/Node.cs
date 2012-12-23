using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RayTracer.GeomertryPrimitives;

namespace RayTracer.OctTreeOptimisation
{
    public class Node
    {
        public BoundingBox Box { get; set; }
        public IList<Geomertry> Objects { get; set; }
        public IList<Node> Nodes { get; set; }

        public Node()
        {
            Objects = new List<Geomertry>();
            Nodes = new List<Node>(8);
        }
    }
}