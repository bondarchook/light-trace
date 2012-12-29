using System;
using Microsoft.Xna.Framework;
using RayTracer;
using RayTracer.Tracer;

namespace LightTrace.Domain.GeomertryPrimitives
{
	public class Triangle : Geomertry
	{
		public Triangle()
		{
		}

		public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{
			A = a;
			B = b;
			C = c;
		}

		public Vector3 A;
		public Vector3 B;
		public Vector3 C;

		//Normals
		public Vector3? Na;
		public Vector3? Nb;
		public Vector3? Nc;

		//Texture coordinates
		public Vector2? Ta;
		public Vector2? Tb;
		public Vector2? Tc;

		public override IntersectionInfo Intersect(Ray ray)
		{
			Vector3 loaclN = Vector3.Cross((B - A), (C - A));

			Vector4 a = A.ToV4();
			Vector4 n = loaclN.ToV4();

			Ray4 localRay = TransformToLocalRay(ray);

			float nd = Vector4.Dot(localRay.Direction, n);

			if (Math.Abs(nd - 0) > Global.Epsilon) // nd != 0
			{
				float intDistance = (Vector4.Dot(a, n) - Vector4.Dot(localRay.Position, n))/nd;

				if (intDistance < 0.0001)
				{
					return null;
				}

				Vector4 q = localRay.Position + localRay.Direction*intDistance;
				Vector3 qq = q.ToV3();

				float dot = Vector3.Dot(Vector3.Cross((B - A), (C - A)), loaclN);

				float alfa = Vector3.Dot(Vector3.Cross((C - B), (qq - B)), loaclN)/dot;
				float beta = Vector3.Dot(Vector3.Cross((A - C), (qq - C)), loaclN)/dot;
				float gamma = Vector3.Dot(Vector3.Cross((B - A), (qq - A)), loaclN)/dot;

				Vector3 barycentricCoordinates = new Vector3(alfa, beta, gamma);

				if (alfa >= 0 && beta >= 0 && gamma >= 0)
					return CreateIntersection(intDistance, localRay, loaclN, barycentricCoordinates);
			}

			return null;
		}

		private IntersectionInfo CreateIntersection(float intDistance, Ray4 localRay, Vector3 loaclN, Vector3 baryCoord)
		{
			var intersectionInfo = new IntersectionInfo(intDistance, this);

			Vector4 point = localRay.Position + localRay.Direction*intDistance;

			if (UseNormals())
			{
				Vector3 n = Na.Value*baryCoord.X + Nb.Value*baryCoord.Y + Nc.Value*baryCoord.Z;
				intersectionInfo.Normal = TransformNormalToCameraCoords(n);
			}
			else
			{
				intersectionInfo.Normal = TransformNormalToCameraCoords(loaclN);
			}

			point = TransformPointToCameraCoords(point);
			intersectionInfo.IntersectionPoint = point.ToV3();

			return intersectionInfo;
		}

		private bool UseNormals()
		{
			return Na.HasValue && Nb.HasValue && Nc.HasValue;
		}

		private bool UseTexture()
		{
			return Ta.HasValue && Tb.HasValue && Tc.HasValue;
		}
	}
}