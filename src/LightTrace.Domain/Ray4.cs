using Microsoft.Xna.Framework;

namespace RayTracer.Tracer
{
	public class Ray4
	{
		public Ray4(Vector4 position, Vector4 direction)
		{
			Position = position;
			Direction = direction;
		}

		public Vector4 Position { get; set; }
		public Vector4 Direction { get; set; }
	}
}