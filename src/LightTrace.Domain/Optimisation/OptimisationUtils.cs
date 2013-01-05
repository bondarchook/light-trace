using System;
using System.Collections.Generic;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation
{
	public class OptimisationUtils
	{
		public static BoundingBox GetBoundingBox(IEnumerable<Geomertry> geomertries)
		{
			float maxX = Single.MinValue;
			float maxY = Single.MinValue;
			float maxZ = Single.MinValue;
			float minX = Single.MaxValue;
			float minY = Single.MaxValue;
			float minZ = Single.MaxValue;

			foreach (var geomertry in geomertries)
			{
				if (geomertry is Triangle)
				{
					Triangle triangle = (Triangle)geomertry;

					Vector3 at = Vector3.Transform(triangle.A, triangle.Transform);
					Vector3 bt = Vector3.Transform(triangle.B, triangle.Transform);
					Vector3 ct = Vector3.Transform(triangle.C, triangle.Transform);

					maxX = Math.Max(maxX, Math.Max(Math.Max(at.X, bt.X), ct.X));
					maxY = Math.Max(maxY, Math.Max(Math.Max(at.Y, bt.Y), ct.Y));
					maxZ = Math.Max(maxZ, Math.Max(Math.Max(at.Z, bt.Z), ct.Z));

					minX = Math.Min(minX, Math.Min(Math.Min(at.X, bt.X), ct.X));
					minY = Math.Min(minY, Math.Min(Math.Min(at.Y, bt.Y), ct.Y));
					minZ = Math.Min(minZ, Math.Min(Math.Min(at.Z, bt.Z), ct.Z));
				}
				else if (geomertry is Sphere)
				{
					Sphere sphere = (Sphere)geomertry;

					Vector3 center = Vector3.Transform(sphere.Center, sphere.Transform);

					float rx = sphere.Radius * sphere.Transform.M11;
					float ry = sphere.Radius * sphere.Transform.M22;
					float rz = sphere.Radius * sphere.Transform.M33;
					float maxR = Math.Max(rx, Math.Max(ry, rz));


					maxX = Math.Max(maxX, center.X + maxR);
					maxY = Math.Max(maxY, center.Y + maxR);
					maxZ = Math.Max(maxZ, center.Z + maxR);

					minX = Math.Min(minX, center.X - maxR);
					minY = Math.Min(minY, center.Y - maxR);
					minZ = Math.Min(minZ, center.Z - maxR);
				}
				else
				{
					throw new Exception("Unsupported geometry");
				}
			}

			return new BoundingBox(new Vector3(minX - 1, minY - 1, minZ - 1), new Vector3(maxX + 1, maxY + 1, maxZ + 1));
		}
	}
}