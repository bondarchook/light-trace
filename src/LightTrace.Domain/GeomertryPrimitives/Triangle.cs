using System;
using Microsoft.Xna.Framework;
using RayTracer;
using RayTracer.Tracer;

namespace LightTrace.Domain.GeomertryPrimitives
{
	public class Triangle : Geomertry
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

		public Triangle()
		{
		}

		public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{
			A = a;
			B = b;
			C = c;
            Prepare();
		}

	    public override void Prepare()
	    {
			base.Prepare();

            _normal = Vector3.Cross((B - A), (C - A));
	        _dotAn = Vector4.Dot(A.ToV4(), _normal.ToV4());
	        _det = Vector3.Dot(_normal, _normal);
	        _cb = C - B;
	        _ac = A - C;
	        _ba = B - A;
	    }

		public override IntersectionInfo Intersect(Ray ray)
		{
            Vector4 n = _normal.ToV4();

			Ray4 localRay = TransformToLocalRay(ray);

			float nd = Vector4.Dot(localRay.Direction, n);

			if (Math.Abs(nd - 0) > Global.Epsilon) // nd != 0
			{
				float intDistance = (_dotAn - Vector4.Dot(localRay.Position, n))/nd;

				if (intDistance < 0.0001)
				{
					return null;
				}

                Vector3 point = (localRay.Position + localRay.Direction * intDistance).ToV3();

                float alfa = Vector3.Dot(Vector3.Cross(_cb, (point - B)), _normal) / _det;
                float beta = Vector3.Dot(Vector3.Cross(_ac, (point - C)), _normal) / _det;
                float gamma = Vector3.Dot(Vector3.Cross(_ba, (point - A)), _normal) / _det;

				Vector3 barycentricCoordinates = new Vector3(alfa, beta, gamma);

				if (alfa >= 0 && beta >= 0 && gamma >= 0)
                    return new IntersectionInfo(intDistance, localRay, barycentricCoordinates, this);
			}

			return null;
		}

        public override void CalculateIntersection(IntersectionInfo info)
		{
		    Ray4 localRay = info.LocalRay;
		    Vector4 point = localRay.Position + localRay.Direction * info.Distance;
            Vector3 baryCoord = info.BaryCoord;

			if (UseNormals())
			{
			    Vector3 n = Na.Value * baryCoord.X + Nb.Value * baryCoord.Y + Nc.Value * baryCoord.Z;
				info.Normal = TransformNormalToCameraCoords(n);
			}
			else
			{
				info.Normal = TransformNormalToCameraCoords(_normal);
			}

			if (UseTexture())
			{
				Vector2 coord = Ta.Value*baryCoord.X+Tb.Value*baryCoord.Y+Tc.Value*baryCoord.Z;
				info.TexCoord = coord;
			}

			point = TransformPointToCameraCoords(point);
			info.IntersectionPoint = point.ToV3();
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