using System;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class Intersectable
	{
		public Geomertry Geomertry;
		public Vector3 Center;
		public BoundingBox BoundingBox;

		public Intersectable(Geomertry geomertry)
		{
			Geomertry = geomertry;
		}

		public static Intersectable CreateIntersectable(Geomertry geomertry)
		{
			if (geomertry is Triangle)
				return new TriangeIntersectable((Triangle) geomertry);
			else if (geomertry is CalculatedTriangle)
			{
				return new CalculatedTriangeIntersectable((CalculatedTriangle) geomertry);
			}
			else if (geomertry is Sphere)
			{
				return new SphereIntersectable((Sphere) geomertry);
			}

			throw new Exception("Unsuported geometry type");
		}
	}
}