using LightTrace.Domain.Shading;
using Microsoft.Xna.Framework;
using RayTracer.Tracer;

namespace LightTrace.Domain.GeomertryPrimitives
{
	public abstract class Geomertry
	{
		public Matrix Transform;
		public Material Material;
		private Matrix _invertedTransform;
		private Matrix _invertedTransposedTransform;

		public abstract IntersectionInfo Intersect(Ray ray);
	    public abstract void CalculateIntersection(IntersectionInfo info);

		public virtual void Prepare()
		{
			_invertedTransform = Matrix.Invert(Transform);
			_invertedTransposedTransform = Matrix.Invert(Matrix.Transpose(Transform));
		}

		protected Ray4 TransformToLocalRay(Ray ray)
		{
			Ray4 localRay = new Ray4(Vector4.Transform(ray.Position.ToV4(), _invertedTransform), Vector4.Transform(ray.Direction.ToV4(0), _invertedTransform));
			return localRay;
		}

		protected Vector3 TransformNormalToCameraCoords(Vector3 normal)
		{
			return Vector3.Normalize(Vector3.Transform(normal, _invertedTransposedTransform));
		}

		protected Vector4 TransformPointToCameraCoords(Vector4 point)
		{
			return Vector4.Transform(point, Transform);
		}
	}
}