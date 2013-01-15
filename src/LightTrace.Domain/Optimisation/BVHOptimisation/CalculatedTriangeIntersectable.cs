using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class CalculatedTriangeIntersectable : Intersectable
	{
		public CalculatedTriangeIntersectable(CalculatedTriangle triangle) : base(triangle)
		{
			Vector3 a = triangle.A;
			Vector3 b = triangle.B;
			Vector3 c = triangle.C;

			Center = (a + b + c)/3;
			BoundingBox = BoundingBox.CreateFromPoints(new[] {a, b, c});
		}
	}
}