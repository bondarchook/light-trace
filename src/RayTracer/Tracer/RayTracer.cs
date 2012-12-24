﻿using System;
using LightTrace.Domain;
using LightTrace.Domain.Nodes;
using Microsoft.Xna.Framework;
using RayTracer.GeomertryPrimitives;

namespace RayTracer.Tracer
{
	public class RayTracer
	{
		private readonly LightTrace.Domain.Scene _scene;

		public RayTracer(LightTrace.Domain.Scene scene)
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
			Vector3 finalColor = Vector3.Zero;

			if (depth > _scene.MaxDepth)
			{
				return finalColor;
			}

			float minDist = Single.MaxValue;
			IntersectionInfo minIntInfo = null;

			foreach (Geomertry geomertry in _scene.GetObjects(ray))
			{
				IntersectionInfo intersectionInfo = geomertry.Intersect(ray);

				if (intersectionInfo != null && minDist > intersectionInfo.Distance)
				{
					minDist = intersectionInfo.Distance;
					minIntInfo = intersectionInfo;
				}
			}

			if (minIntInfo != null)
			{
				Vector3 surfaceColor = CalculateSurfaceColor(minIntInfo, ray);

				return surfaceColor;

				Vector3 specularColor = minIntInfo.Geomertry.Material.SpecularColor;
				Vector3 reflectedColor = Vector3.Zero;

				float dot = Vector3.Dot(ray.Direction, minIntInfo.Normal);
				if (dot <= 0)
				{
					Vector3 reflectDirection = Vector3.Reflect(ray.Direction, minIntInfo.Normal);

					if (specularColor.X > 0 || specularColor.Y > 0 || specularColor.Z > 0)
					{
						reflectedColor = TraceRay(ShiftRay(minIntInfo.IntersectionPoint, reflectDirection), depth + 1);
					}

					finalColor = surfaceColor + specularColor*reflectedColor;
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
			Vector3 eyepos = ray.Position;

			Material material = intersectionInfo.Geomertry.Material;
			Vector3 diffuseColor = material.DiffuseColor;
			Vector3 specularColor = material.SpecularColor;
			Vector3 emissionColor = material.EmissionColor;
			Vector3 ambientColor = material.AmbientColor;

			float shininess = material.Shininess;

			Vector3 lambert = Vector3.Zero;
			Vector3 specular = Vector3.Zero;

			return ambientColor;

			Vector3 point = intersectionInfo.IntersectionPoint;

			Vector3 eyeDirection = eyepos - point;
			eyeDirection.Normalize();

			foreach (LightTrace.Domain.Nodes.Light light in _scene.Lights)
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


		private bool CheckLight(IntersectionInfo intersection, Vector3 direction, float distanceToLight, LightTrace.Domain.Nodes.Light light)
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