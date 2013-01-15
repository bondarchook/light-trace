using System;
using Microsoft.Xna.Framework;
using RayTracer;
using RayTracer.Tracer;

namespace LightTrace.Domain.GeomertryPrimitives
{
	public class CalculatedTriangle : Geomertry
	{
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

		private Vector3 _normal;
		private Vector3 _cb;
		private Vector3 _ac;
		private Vector3 _ba;
		private float _dotAn;
		private float _det;

		public CalculatedTriangle()
		{
		}

		public CalculatedTriangle(Vector3 a, Vector3 b, Vector3 c, Matrix transform)
		{
			A = Vector3.Transform(a, transform);
			B = Vector3.Transform(b, transform);
			C = Vector3.Transform(c, transform);
			Transform = transform;
		}

		public override void Prepare()
		{
			base.Prepare();

			_normal = Vector3.Normalize(Vector3.Cross((B - A), (C - A)));
			_dotAn = Vector3.Dot(A, _normal);
			_det = Vector3.Dot(_normal, _normal);
			_cb = C - B;
			_ac = A - C;
			_ba = B - A;
		}

		public override IntersectionInfo Intersect(Ray ray)
		{
			float nd = Vector3.Dot(ray.Direction, _normal);

			if (Math.Abs(nd - 0) > Global.Epsilon) // nd != 0
			{
				float intDistance = (_dotAn - Vector3.Dot(ray.Position, _normal))/nd;

				if (intDistance < 0.0001)
				{
					return null;
				}

				Vector3 point = (ray.Position + ray.Direction*intDistance);

				float alfa = Vector3.Dot(Vector3.Cross(_cb, (point - B)), _normal)/_det;

				if (alfa < 0)
					return null;

				float beta = Vector3.Dot(Vector3.Cross(_ac, (point - C)), _normal)/_det;

				if (beta < 0)
					return null;

				float gamma = Vector3.Dot(Vector3.Cross(_ba, (point - A)), _normal)/_det;

				if (gamma < 0)
					return null;

				Vector3 barycentricCoordinates = new Vector3(alfa, beta, gamma);
				return new IntersectionInfo(intDistance, ray, barycentricCoordinates, this);
			}

			return null;
		}

		public override void CalculateIntersection(IntersectionInfo info)
		{
			Ray localRay = info.OriginalRay;
			Vector3 point = localRay.Position + localRay.Direction*info.Distance;
			Vector3 baryCoord = info.BaryCoord;

			if (UseNormals())
			{
				Vector3 n = Na.Value*baryCoord.X + Nb.Value*baryCoord.Y + Nc.Value*baryCoord.Z;
				info.Normal = n;
			}
			else
			{
				info.Normal = _normal;
			}

			if (UseTexture())
			{
				Vector2 coord = Ta.Value*baryCoord.X + Tb.Value*baryCoord.Y + Tc.Value*baryCoord.Z;
				info.TexCoord = coord;
			}

			info.IntersectionPoint = point;
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