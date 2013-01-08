using System;
using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Nodes;
using LightTrace.Domain.Optimisation.BVHOptimisation;
using LightTrace.Domain.Optimisation.OctTreeOptimisation;
using LightTrace.Domain.Shading;
using Microsoft.Xna.Framework;
using Node = LightTrace.Domain.Nodes.Node;

namespace LightTrace.Domain
{
	public class Scene
	{
		public string Id;
		public string Name;
		public IList<Node> Nodes;
		public IList<Light> Lights;
		public IList<Geomertry> Geomertries;
		public IList<Texture> Textures;

		public Camera Camera;

		public int MaxDepth;
		public Vector3 EnvironmentColor;
		public string OutputFileName;

		protected OctTree _tree;
	    public BvhTree BvhTree;
		protected bool _useOctTree = true;

		public Scene()
		{
			Nodes = new List<Node>();
			Textures = new List<Texture>();
			MaxDepth = 5;
			EnvironmentColor = new Vector3(0.1f);
		}

		public virtual void PrepareScene()
		{
			if (!Nodes.OfType<Camera>().Any())
				throw new Exception("No camera in scene");

			Camera = Nodes.OfType<Camera>().First();
			Camera.PrepareCamera();

			Lights = Nodes.OfType<Light>().ToList();
			Geomertries = new List<Geomertry>();

			foreach (Texture texture in Textures)
			{
				texture.PrepareTexture();
			}

			foreach (var geometry in Nodes.OfType<MeshGeometry>())
			{
				foreach (var triangle in geometry.Triangles)
				{
					triangle.Transform = geometry.Scale*geometry.Rotation*geometry.Translation;

					Geomertries.Add(triangle);
				}
			}

			if (_useOctTree && Geomertries.Any())
			{
//				OctTreeBuilder builder = new OctTreeBuilder();
//				_tree = builder.Build(Geomertries, 6, 5);

				BvhTreeBuilder bvhTreeBuilder = new BvhTreeBuilder();
				BvhTree = bvhTreeBuilder.BuildBvhTree(Geomertries, 5, 1);
			}
		}

		public IEnumerable<Geomertry> GetObjects(Ray ray)
		{
			if (_useOctTree)
			{
				//IEnumerable<Geomertry> triangles = _tree.GetPotencialObjects(ray);
				IEnumerable<Geomertry> triangles = BvhTree.GetPotencialObjects(ray);
				return triangles;
			}
			else
			{
				return Geomertries;
			}
		}

		public long GetObjectsCount(Ray ray)
		{
			if (_useOctTree)
			{
				//IEnumerable<Geomertry> triangles = _tree.GetPotencialObjects(ray);
				return BvhTree.GetPotencialObjectsCount(ray);
			}
			else
			{
				return Geomertries.Count;
			}
		}
	}
}