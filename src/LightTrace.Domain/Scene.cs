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

		public Scene()
		{
			Nodes = new List<Node>();
			MaxDepth = 5;
		}

		public void PrepareScene()
		{
			if (!Nodes.OfType<Camera>().Any())
				throw new Exception("No camera in scene");

			Camera = Nodes.OfType<Camera>().First();
			Camera.PrepareCamera();

			Lights = Nodes.OfType<Light>().ToList();
			Triangles = new List<Triangle>();

			Random random = new Random();

			foreach (var geometry in Nodes.OfType<MeshGeometry>())
			{
				foreach (var triangle in geometry.Triangles)
				{
					Material material = new Material();
					material.AmbientColor = new Vector3((float) (random.NextDouble()*0.7 + 0.2), (float) (random.NextDouble()*0.7 + 0.2), (float) (random.NextDouble()*0.7 + 0.2));

					triangle.Material = material;
					triangle.Transform = geometry.Translation*geometry.Rotation;

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