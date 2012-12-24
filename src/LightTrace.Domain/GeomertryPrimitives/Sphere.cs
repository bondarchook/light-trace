using System;
using LightTrace.Domain;
using Microsoft.Xna.Framework;
using RayTracer.Tracer;

namespace RayTracer.GeomertryPrimitives
{
    public class Sphere : Geomertry
    {
        public float Radius { get; set; }
        public Vector3 Center { get; set; }

        public Sphere()
        {
        }

        public Sphere(Vector3 center, float radius)
        {
            Radius = radius;
            Center = center;
        }

        public Sphere(float x, float y, float z, float radius)
        {
            Radius = radius;
            Center = new Vector3(x, y, z);
        }

        public override IntersectionInfo Intersect(Ray ray)
        {
            float intDist;
            Vector4 center = Center.ToV4();

            Ray4 localRay = TransformToLocalRay(ray);

            double a = Vector4.Dot(localRay.Direction, localRay.Direction);
            double b = 2.0*Vector4.Dot(localRay.Direction, localRay.Position - center);
            double c = Vector4.Dot(localRay.Position - center, localRay.Position - center) - Radius*Radius;

            double d = b*b - 4.0*a*c;

            if (d < 0)
            {
                return null;
            }

            if (Math.Abs(d - 0.0) < Global.Epsilon) // d == 0
            {
                var t = (float) (-b/2.0*a);
                intDist = t;
                return null;
            }
            else
            {
                double dd = Math.Sqrt(d);

                double t1 = (-b - dd)/(2*a);
                double t2 = (-b + dd)/(2*a);

                double min = Math.Min(t1, t2);
                double max = Math.Max(t1, t2);

                if (max < 0)
                    return null;
                else
                {
                    if (t1 >= 0 && t2 >= 0)
                        intDist = (float) min;
                    else
                    {
                        intDist = (float) max;
                    }
                }
            }

            return CreateIntersection(intDist, localRay, center);
        }

        private IntersectionInfo CreateIntersection(float intDistance, Ray4 ray, Vector4 center)
        {
            var intersectionInfo = new IntersectionInfo(intDistance, this);

            Vector4 point = ray.Position + ray.Direction*intDistance;

            Vector3 normal = point.ToV3() - center.ToV3();
            intersectionInfo.Normal = TransformNormalToCameraCoords(normal);

            point = TransformPointToCameraCoords(point);

            intersectionInfo.IntersectionPoint = point.ToV3();

            return intersectionInfo;
        }
    }
}