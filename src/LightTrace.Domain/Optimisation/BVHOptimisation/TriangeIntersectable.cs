using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class TriangeIntersectable : Intersectable
	{
		public TriangeIntersectable(Triangle triangle) : base(triangle)
		{
			Vector3 a = Vector3.Transform(triangle.A, triangle.Transform);
			Vector3 b = Vector3.Transform(triangle.B, triangle.Transform);
			Vector3 c = Vector3.Transform(triangle.C, triangle.Transform);

			Center = (a + b + c)/3;
			BoundingBox = BoundingBox.CreateFromPoints(new[] {a, b, c});
		}
	}
}