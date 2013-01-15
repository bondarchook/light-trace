using Microsoft.Xna.Framework;

namespace RayTracer.Tracer
{
	public class Ray4
	{
		public Vector4 Position;
		public Vector4 Direction;

		public Ray4(Vector4 position, Vector4 direction)
		{
			Position = position;
			Direction = direction;
		}
	}
}