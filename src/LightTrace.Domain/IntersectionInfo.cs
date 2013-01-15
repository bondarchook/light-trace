using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace RayTracer.Tracer
{
	public class IntersectionInfo
	{
		public float Distance;
		public Vector3 IntersectionPoint;
		public Vector3 Normal;
		public Vector2 TexCoord;
		public Vector3 BaryCoord;
		public Ray4 LocalRay;
		public Ray OriginalRay;
		public Geomertry Geomertry;

		public IntersectionInfo(float distance, Ray originalRay, Geomertry geomertry)
	    {
	        Distance = distance;
			OriginalRay = originalRay;
	        Geomertry = geomertry;
	    }

	    public IntersectionInfo(float distance, Ray originalRay, Vector3 baryCoord, Geomertry geomertry)
	    {
	        Distance = distance;
	        BaryCoord = baryCoord;
			OriginalRay = originalRay;
			Geomertry = geomertry;
	    }

		public IntersectionInfo(float distance, Ray4 localRay, Geomertry geomertry)
	    {
	        Distance = distance;
	        LocalRay = localRay;
	        Geomertry = geomertry;
	    }

	    public IntersectionInfo(float distance, Ray4 localRay, Vector3 baryCoord, Geomertry geomertry)
	    {
	        Distance = distance;
	        BaryCoord = baryCoord;
	        LocalRay = localRay;
	        Geomertry = geomertry;
	    }

	    public void Calculate()
        {
            Geomertry.CalculateIntersection(this);
	    }
	}
}