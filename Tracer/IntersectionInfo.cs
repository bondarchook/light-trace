using Microsoft.Xna.Framework;
using RayTracer.GeomertryPrimitives;

namespace RayTracer.Tracer
{
    public class IntersectionInfo
    {
        public IntersectionInfo(float distance, Geomertry geomertry)
        {
            Distance = distance;
            Geomertry = geomertry;
        }

        public float Distance { get; set; }
        public Vector3 IntersectionPoint { get; set; }
        public Vector3 Normal { get; set; }
        public Geomertry Geomertry { get; set; }
    }
}