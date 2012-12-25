using System;
using System.Collections.Generic;
using LightTrace.Domain.Nodes;
using System.Linq;
using Microsoft.Xna.Framework;
using RayTracer;
using RayTracer.GeomertryPrimitives;

namespace LightTrace.Domain
{
	public class Scene
	{
		public string Id;
		public string Name;
		public IList<Node> Nodes;
		public IList<Light> Lights;
		public IList<Triangle> Triangles;

		public Camera Camera;

		public int MaxDepth;
		public Vector3 EnvironmentColor;

		public Scene()
		{
			Nodes = new List<Node>();
			MaxDepth = 5;
			EnvironmentColor = new Vector3(0.1f);
		}

		public void PrepareScene()
		{
			if (!Nodes.OfType<Camera>().Any())
				throw new Exception("No camera in scene");

			Camera = Nodes.OfType<Camera>().First();
			Camera.PrepareCamera();

			Lights = Nodes.OfType<Light>().ToList();
			Triangles = new List<Triangle>();


			foreach (var geometry in Nodes.OfType<MeshGeometry>())
			{
				foreach (var triangle in geometry.Triangles)
				{
					triangle.Transform = geometry.Scale*geometry.Rotation * geometry.Translation;

					Triangles.Add(triangle);
				}
			}

		}

		public IEnumerable<Geomertry> GetObjects(Ray ray)
		{
			return Triangles;
		}
	}
}