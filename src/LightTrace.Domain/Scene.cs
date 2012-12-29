using System;
using System.Collections.Generic;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Nodes;
using System.Linq;
using LightTrace.Domain.OctTreeOptimisation;
using Microsoft.Xna.Framework;
using RayTracer;
using Node = LightTrace.Domain.Nodes.Node;

namespace LightTrace.Domain
{
	public class Scene
	{
		public string Id;
		public string Name;
		public IList<Node> Nodes;
		public IList<Light> Lights;
		public IList<Geomertry> Triangles;

		public Camera Camera;

		public int MaxDepth;
		public Vector3 EnvironmentColor;
		
		private OctTree _tree;
		private bool _useOctTree = true;

		public Scene()
		{
			Nodes = new List<Node>();
			MaxDepth = 1;
			EnvironmentColor = new Vector3(0.1f);
		}

		public void PrepareScene()
		{
			if (!Nodes.OfType<Camera>().Any())
				throw new Exception("No camera in scene");

			Camera = Nodes.OfType<Camera>().First();
			Camera.PrepareCamera();

			Lights = Nodes.OfType<Light>().ToList();
			Triangles = new List<Geomertry>();


			foreach (var geometry in Nodes.OfType<MeshGeometry>())
			{
				foreach (var triangle in geometry.Triangles)
				{
					triangle.Transform = geometry.Scale*geometry.Rotation * geometry.Translation;

					Triangles.Add(triangle);
				}
			}

			OctTreeBuilder builder = new OctTreeBuilder();
			if (_useOctTree && Triangles.Any())
			{
				_tree = builder.Build(Triangles, 7, 10);
			}

		}

		public IEnumerable<Geomertry> GetObjects(Ray ray)
		{
			if (_useOctTree)
			{
				IEnumerable<Geomertry> triangles = _tree.GetPotencialObjects(ray);
				return triangles;
			}
			else
			{
				return Triangles;
			}
		}
	}
}