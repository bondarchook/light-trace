using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Nodes;
using LightTrace.Domain.Optimisation.BVHOptimisation;
using LightTrace.Domain.Optimisation.OctTreeOptimisation;
using Microsoft.Xna.Framework;

namespace LightTrace.SimpleSceneReader
{
	public class SimpleScene : Scene
	{
		public SimpleScene()
		{
			Lights = new List<Light>();
			Geomertries = new List<Geomertry>();
			EnvironmentColor = Vector3.Zero;
		}

		public override void PrepareScene()
		{
			Camera.PrepareCamera();

			foreach (Geomertry triangle in Geomertries)
			{
				triangle.Prepare();
			}

			UseOptimisation = true;

			if (UseOptimisation && Geomertries.Any())
			{
				BuildTree();
			}
		}
	}
}