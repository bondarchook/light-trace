using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.OctTreeOptimisation
{
	public class OctTreeBuilder
	{
		private int _minObjectsInCube = 5;
		private int _maxDepth = 3;
//        private readonly ILoger _loger;

		public OctTree Build(IList<Geomertry> objects, int maxDepth, int minObjectsInCube)
		{
			return Build(OptimisationUtils.GetBoundingBox(objects), objects, maxDepth, minObjectsInCube);
		}

		public OctTree Build(BoundingBox box, IList<Geomertry> objects, int maxDepth, int minObjectsInCube)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Node root = new Node();

			root.Box = box;
			root.Objects = objects;

			_maxDepth = maxDepth;
			_minObjectsInCube = minObjectsInCube;

			_minObjectsInCube = Math.Min(_minObjectsInCube, objects.Count);

			Devide(root, 0);

//            _loger.Log(Level.Info, string.Format("Oct tree build time: {0} ms", stopwatch.ElapsedMilliseconds));

			return new OctTree(root);
		}

		private void Devide(Node node, int level)
		{
			if (level > _maxDepth)
			{
				return;
			}

			Vector3 min = node.Box.Min;
			float xs = (node.Box.Max.X - min.X)/2;
			float ys = (node.Box.Max.Y - min.Y)/2;
			float zs = (node.Box.Max.Z - min.Z)/2;


			for (int x = 0; x <= 1; x++)
			{
				for (int y = 0; y <= 1; y++)
				{
					for (int z = 0; z <= 1; z++)
					{
						Node subNode = new Node();

						var bMin = new Vector3(min.X + xs*x, min.Y + ys*y, min.Z + zs*z);
						var bMax = new Vector3(min.X + xs*x + xs, min.Y + ys*y + ys, min.Z + zs*z + zs);

						BoundingBox box = new BoundingBox(bMin, bMax);
						subNode.Box = box;
						subNode.Objects = node.Objects.Where(t => ContaiseGeomertry(box, t)).ToList();

						if (subNode.Objects.Any())
						{
							node.Nodes.Add(subNode);

							if (subNode.Objects.Count >= _minObjectsInCube)
							{
								Devide(subNode, level + 1);
							}
						}
					}
				}
			}
			node.Objects = null;
		}

		private static bool ContaiseGeomertry(BoundingBox box, Geomertry geomertry)
		{
			if (geomertry is Triangle)
			{
				Triangle triangle = (Triangle) geomertry;

				Vector3 at = Vector3.Transform(triangle.A, triangle.Transform);
				Vector3 bt = Vector3.Transform(triangle.B, triangle.Transform);
				Vector3 ct = Vector3.Transform(triangle.C, triangle.Transform);


				return box.Contains(at) == ContainmentType.Contains ||
				       box.Contains(bt) == ContainmentType.Contains ||
				       box.Contains(ct) == ContainmentType.Contains;
			}
			else if (geomertry is Sphere)
			{
				Sphere sphere = (Sphere) geomertry;

				Vector3 center = Vector3.Transform(sphere.Center, sphere.Transform);
				float rx = sphere.Radius*sphere.Transform.M11;
				float ry = sphere.Radius*sphere.Transform.M22;
				float rz = sphere.Radius*sphere.Transform.M33;
				float maxR = Math.Max(rx, Math.Max(ry, rz));

				float a = box.Max.X - box.Min.X;
				float b = box.Max.Y - box.Min.Y;
				float c = box.Max.Z - box.Min.Z;

				float halfDiagonal = (float) ((Math.Sqrt(a*a + b*b + c*c))/2.0);
				float distance = Vector3.Distance(center, (box.Max + box.Min)/2.0f);

				return distance <= halfDiagonal + maxR;
			}

			throw new Exception("Unsupported geometry");
		}
	}
}