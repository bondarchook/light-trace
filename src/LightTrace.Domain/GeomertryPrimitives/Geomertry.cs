using LightTrace.Domain.Shading;
using Microsoft.Xna.Framework;
using RayTracer.Tracer;

namespace LightTrace.Domain.GeomertryPrimitives
{
	public abstract class Geomertry
	{
		public Matrix Transform { get; set; }
		public Material Material { get; set; }

		public abstract IntersectionInfo Intersect(Ray ray);

		protected Ray4 TransformToLocalRay(Ray ray)
		{
			Matrix invertedTransform = Matrix.Invert(Transform);

			Ray4 localRay = new Ray4(Vector4.Transform(ray.Position.ToV4(), invertedTransform), Vector4.Transform(ray.Direction.ToV4(0), invertedTransform));
			return localRay;
		}

		protected Vector3 TransformNormalToCameraCoords(Vector3 normal)
		{
			return Vector3.Normalize(Vector3.Transform(normal, Matrix.Invert(Matrix.Transpose(Transform))));
		}

		protected Vector4 TransformPointToCameraCoords(Vector4 point)
		{
			return Vector4.Transform(point, Transform);
		}
	}
}