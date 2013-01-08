using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;
using RayTracer;
using RayTracer.UI;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTreeBuilder
	{
		private Intersectable[] _objects;

		private int _maxDepth = 4;
		private int _minObjectsPerLeaf = 2;
		private float _triangleCost = 1;
		private float _boxCost = 1;

		private readonly ILoger _loger;

		public BvhTreeBuilder()
		{
			_loger = Context.Instance.Loger;
		}

		public BvhTree BuildBvhTree(IEnumerable<Geomertry> geomertries, int maxDepth, int minObjPerLeaf, float triangleCost, float boxCost)
		{
			_maxDepth = maxDepth;
			_minObjectsPerLeaf = minObjPerLeaf;
			_triangleCost = triangleCost;
			_boxCost = boxCost;

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			_objects = geomertries.Select(Intersectable.CreateIntersectable).ToArray();

			BoundingBox rootBoundingBox = OptimisationUtils.GetBoundingBox(geomertries);
			BvhNode root = new BvhNode(rootBoundingBox);

			BuildRecursive(root, _objects, 0);

			stopwatch.Stop();
			_loger.Log(Level.Info, string.Format("BVH tree build time: {0} ms", stopwatch.ElapsedMilliseconds));

			return new BvhTree(root);
		}

		private void BuildRecursive(BvhNode node, Intersectable[] objects, int depht)
		{
			//var axisIndex = MaxAxisIndex(node.BoundingBox);
			int axisIndex;

			float? splitPoint = FindSplit(node, objects, out axisIndex);

			if (!splitPoint.HasValue || objects.Length < _minObjectsPerLeaf || depht > _maxDepth)
			{
				node.IsLeaf = true;
				node.Objects = objects;
				//Console.WriteLine("{2} {1} {0}", depht, objects.Length, Math.Max(Math.Max(node.BoundingBox.Max.X - node.BoundingBox.Min.X,node.BoundingBox.Max.Y - node.BoundingBox.Min.Y),node.BoundingBox.Max.Z - node.BoundingBox.Min.Z));
				return;
			}

			Array.Sort(objects, (objA, objB) => objA.Center.AxisValue(axisIndex).CompareTo(objB.Center.AxisValue(axisIndex)));
			float[] centers = objects.Select(intersectable => intersectable.Center.AxisValue(axisIndex)).ToArray();

			int splitIndex = Array.BinarySearch(centers, splitPoint);
			splitIndex = splitIndex < 0 ? ~splitIndex : splitIndex;


			long leftLenght = splitIndex;
			if (leftLenght > 0)
			{
				node.Left = new BvhNode();
				CreateAndSplitChildNode(node.Left, objects, 0, leftLenght, depht + 1);
			}

			long rightLenght = objects.LongLength - splitIndex;
			if (rightLenght > 0)
			{
				node.Right = new BvhNode();
				CreateAndSplitChildNode(node.Right, objects, splitIndex, rightLenght, depht + 1);
			}
		}

		private void CreateAndSplitChildNode(BvhNode node, Intersectable[] objects, int splitIndex, long lenght, int depht)
		{
			var right = CopyArraySection(objects, splitIndex, lenght);

			node.BoundingBox = CalculateBoundingBox(right);

			BuildRecursive(node, right, depht);
		}

		private float? FindSplit(BvhNode node, Intersectable[] objects, out int axisIndexa)
		{
			float sP = BoxSurface(node.BoundingBox);

			int bestAxis = MaxAxisIndex(node.BoundingBox);
			float? bestSplit = null;
			float minC = float.MaxValue;

			for (int axis = 0; axis < 3; axis++)
			{
				Array.Sort(objects, (objA, objB) => objA.Center.AxisValue(axis).CompareTo(objB.Center.AxisValue(axis)));
				float[] centers = objects.Select(intersectable => intersectable.Center.AxisValue(axis)).ToArray();

				float min = node.BoundingBox.Min.AxisValue(axis);
				float max = node.BoundingBox.Max.AxisValue(axis);
				float step = (max - min)/10;

				for (float split = min + step; split < max; split += step)
				{
					int splitIndex = Array.BinarySearch(centers, split);
					splitIndex = splitIndex < 0 ? ~splitIndex : splitIndex;

					int nL = splitIndex;
					int nR = objects.Length - splitIndex;

					float sL = BoxSurface(CalculateBoundingBox(objects, 0, nL));
					float sR = BoxSurface(CalculateBoundingBox(objects, splitIndex, nR));

					float c = _boxCost + (sL/sP)*nL*_triangleCost + (sR/sP)*nR*_triangleCost;

					if (c < minC)
					{
						minC = c;
						bestSplit = split;
						bestAxis = axis;
					}
				}
			}

			if (minC < objects.Length*_triangleCost)
			{
				axisIndexa = bestAxis;
				return bestSplit;
			}
			else
			{
				axisIndexa = 1;
				return null;
			}
		}

		private static float BoxSurface(BoundingBox box)
		{
			BoundingBox parentBox = box;
			float a = parentBox.Max.X - parentBox.Min.X;
			float b = parentBox.Max.Y - parentBox.Min.Y;
			float c = parentBox.Max.Z - parentBox.Min.Z;
			return a*b + a*c + b*c;
		}

		private static Intersectable[] CopyArraySection(Intersectable[] objects, long startIndex, long length)
		{
			Intersectable[] left = new Intersectable[length];
			Array.Copy(objects, startIndex, left, 0, length);
			return left;
		}

		private static BoundingBox CalculateBoundingBox(Intersectable[] objects)
		{
			return CalculateBoundingBox(objects, 0, objects.Length);
		}

		private static BoundingBox CalculateBoundingBox(Intersectable[] objects, long startIndex, long endIndex)
		{
			BoundingBox box = BoundingBox.CreateFromPoints(objects[0].BoundingBox.GetCorners());
			for (long i = startIndex; i < endIndex; i++)
			{
				BoundingBox.CreateMerged(ref box, ref objects[i].BoundingBox, out box);
			}
			return box;
		}

		private static int MaxAxisIndex(BoundingBox boundingBox)
		{
			int maxAxisIndex = 0;
			float longestAxis = float.MinValue;

			for (int i = 0; i < 3; i++)
			{
				float length = boundingBox.Max.AxisValue(i) - boundingBox.Min.AxisValue(i);
				if (length > longestAxis)
				{
					longestAxis = length;
					maxAxisIndex = i;
				}
			}
			return maxAxisIndex;
		}
	}
}