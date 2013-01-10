using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

	    public BvhTree BvhTree;
		protected bool UseOptimisation = true;

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
                    triangle.Prepare();

					Geomertries.Add(triangle);
				}
			}

			if (UseOptimisation && Geomertries.Any())
			{
				BuildTree();
			}
		}

		protected void BuildTree()
		{
			BvhTreeBuilder bvhTreeBuilder = new BvhTreeBuilder();

			var triangleCost = float.Parse(ConfigurationManager.AppSettings["TriangleIntersect"],CultureInfo.InvariantCulture);
			var boxCost = float.Parse(ConfigurationManager.AppSettings["BoundingBoxIntersect"], CultureInfo.InvariantCulture);

			BvhTree = bvhTreeBuilder.BuildBvhTree(Geomertries, 15, 1, triangleCost, boxCost);
		}

		public IEnumerable<Geomertry> GetObjects(Ray ray)
		{
			if (UseOptimisation)
			{
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
			if (UseOptimisation)
			{
				return BvhTree.GetPotencialObjectsCount(ray);
			}
			else
			{
				return Geomertries.Count;
			}
		}
	}
}