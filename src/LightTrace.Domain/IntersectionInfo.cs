using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

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
		public Vector2 TexCoord { get; set; }
		public Geomertry Geomertry { get; set; }
	}
}