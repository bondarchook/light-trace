using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightTrace.Domain;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Nodes;
using LightTrace.Domain.Shading;
using Microsoft.Xna.Framework;

namespace RayTracer.Tracer
{
	public class RayTracer
	{
		private readonly Scene _scene;

		public RayTracer(Scene scene)
		{
			_scene = scene;
		}

		public Vector3 CalculatePixelColor(int x, int y)
		{
			Ray primaryRay = _scene.Camera.CreatePrimaryRay(x, y);
			Vector3 finalColor = TraceRay(primaryRay, 0);
			return finalColor;
		}

		private Vector3 TraceRay(Ray ray, int depth)
		{
			Vector3 finalColor = _scene.EnvironmentColor;

			if (depth > _scene.MaxDepth)
			{
				return finalColor;
			}

			float minDist = Single.MaxValue;
			IntersectionInfo intersection = null;

//			long count = _scene.GetObjectsCount(ray);
////			long count = _scene.GetObjects(ray).Count();
//			long totalCount = _scene.Geomertries.Count;
//
//			double d = count/(double) totalCount;
//
//			return new Vector3((float) d*1000.5f);
////			finalColor += new Vector3((float) d*2000.5f);

			foreach (Geomertry geomertry in _scene.GetObjects(ray))
			{
				IntersectionInfo currentIntersection = geomertry.Intersect(ray);

				if (currentIntersection != null && minDist > currentIntersection.Distance)
				{
					minDist = currentIntersection.Distance;
					intersection = currentIntersection;
				}
			}

			if (intersection != null)
			{
				//return new Vector3(intersection.TexCoord.X, intersection.TexCoord.Y, 0.5f);

				Vector3 surfaceColor = CalculateSurfaceColor(intersection, ray);

				Material material = intersection.Geomertry.Material;
				Vector3 reflectiveColor = material.ReflectiveColor*material.Reflectivity;
				Vector3 reflectedColor = Vector3.Zero;

				// Backface culling. Do not reflect from back side of triangle
				float dot = Vector3.Dot(ray.Direction, intersection.Normal);
				if (dot <= 0)
				{
					if (reflectiveColor.X > 0 || reflectiveColor.Y > 0 || reflectiveColor.Z > 0)
					{
						Vector3 reflectDirection = Vector3.Reflect(ray.Direction, intersection.Normal);
						reflectedColor = TraceRay(ShiftRay(intersection.IntersectionPoint, reflectDirection), depth + 1);
					}

					finalColor = surfaceColor + reflectiveColor*reflectedColor;
				}
				else
				{
					finalColor = surfaceColor;
				}
			}
			return finalColor;
		}

		private Vector3 CalculateSurfaceColor(IntersectionInfo intersectionInfo, Ray ray)
		{
			Material material = intersectionInfo.Geomertry.Material;
			Vector3 diffuseColor = material.DiffuseColor.GetColor(intersectionInfo.TexCoord);
			Vector3 specularColor = material.SpecularColor;
			Vector3 emissionColor = material.EmissionColor;
			Vector3 ambientColor = material.AmbientColor;

			float shininess = material.Shininess;

			Vector3 lambert = Vector3.Zero;
			Vector3 specular = Vector3.Zero;

			Vector3 eyepos = ray.Position;
			Vector3 point = intersectionInfo.IntersectionPoint;

			Vector3 eyeDirection = eyepos - point;
			eyeDirection.Normalize();

			foreach (Light light in _scene.Lights)
			{
				Vector3 direction;
				float attenuation;
				float dist;

				light.Calculate(point, out direction, out dist, out attenuation);

				Vector3 half = (direction + eyeDirection);
				half.Normalize();

				if (CheckLight(intersectionInfo, direction, dist, light))
				{
					Vector3 lightColor = light.Color/attenuation;

					float nDotL = Vector3.Dot(intersectionInfo.Normal, direction);
					float nDotH = Vector3.Dot(intersectionInfo.Normal, half);
					lambert += diffuseColor*lightColor*Math.Max(nDotL, 0.0f);
					specular += specularColor*lightColor*(float) Math.Pow(Math.Max(nDotH, 0.0f), shininess);
				}
			}

			Vector3 finalColor = ambientColor + emissionColor + lambert + specular;

			return finalColor;
		}


		private bool CheckLight(IntersectionInfo intersection, Vector3 direction, float distanceToLight, Light light)
		{
			Vector3 point = intersection.IntersectionPoint;
			Ray ray = ShiftRay(point, direction);

			foreach (Geomertry geomertry in _scene.GetObjects(ray))
			{
				IntersectionInfo intersectionInfo = geomertry.Intersect(ray);
				if (intersectionInfo != null)
				{
					if (light is PointLight && intersectionInfo.Distance < distanceToLight)
						return false;

					if (light is DirectionalLight)
						return false;
				}
			}

			return true;
		}

		private Ray ShiftRay(Vector3 position, Vector3 direction)
		{
			return new Ray(position + direction*0.0005f, direction);
		}
	}
}