using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace RayTracer.Tracer
{
	public class IntersectionInfo
	{
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

	    public float Distance { get; set; }
		public Vector3 IntersectionPoint { get; set; }
		public Vector3 Normal { get; set; }
		public Vector2 TexCoord { get; set; }
        public Vector3 BaryCoord { get; set; }
        public Ray4 LocalRay { get; set; }
		public Geomertry Geomertry { get; set; }
	}
}