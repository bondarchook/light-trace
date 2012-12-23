using Microsoft.Xna.Framework;

namespace RayTracer
{
    public class Light
    {
        public LightType Type { get; set; }
        public Vector3 Color { get; set; }
        public Vector3 Position { get; set; }
    }
}