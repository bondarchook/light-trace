using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class SphereIntersectable : Intersectable
	{
		public SphereIntersectable(Sphere sphere) : base(sphere)
		{
			Vector3[] points = new Vector3[6];

			points[0] = Vector3.Transform(sphere.Center + Vector3.Up, sphere.Transform);
			points[1] = Vector3.Transform(sphere.Center + Vector3.Down, sphere.Transform);
			points[2] = Vector3.Transform(sphere.Center + Vector3.Left, sphere.Transform);
			points[3] = Vector3.Transform(sphere.Center + Vector3.Right, sphere.Transform);
			points[4] = Vector3.Transform(sphere.Center + Vector3.Forward, sphere.Transform);
			points[5] = Vector3.Transform(sphere.Center + Vector3.Backward, sphere.Transform);

			Center = sphere.Center;
			BoundingBox = BoundingBox.CreateFromPoints(points);
		}
	}
}