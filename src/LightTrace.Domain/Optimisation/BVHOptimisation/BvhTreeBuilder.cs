using System;
using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTreeBuilder
	{
		private Intersectable[] _objects;

		private int _maxDepth = 4;
		private int _minObjectsPerLeaf = 2;
		private Random _random;

		public BvhTree BuildBvhTree(IEnumerable<Geomertry> geomertries, int maxDepth, int minObjPerLeaf)
		{
			_random = new Random();
			_maxDepth = maxDepth;
			_minObjectsPerLeaf = minObjPerLeaf;

			_objects = geomertries.Select(Intersectable.CreateIntersectable).ToArray();

//				list.Sort((objA, objB) => objA.Center.AxisValue(axis).CompareTo(objB.Center.AxisValue(axis)));
//				_centers[axis] = new float[list.Count];

			BoundingBox rootBoundingBox = OptimisationUtils.GetBoundingBox(geomertries);
			BvhNode root = new BvhNode(rootBoundingBox);

			var ax = MaxAxisIndex(rootBoundingBox);

			BuildRecursive(root, _objects, 0);

			Console.WriteLine("{0}", Math.Max(Math.Max(rootBoundingBox.Max.X - rootBoundingBox.Min.X, rootBoundingBox.Max.Y - rootBoundingBox.Min.Y), rootBoundingBox.Max.Z - rootBoundingBox.Min.Z));

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

			//float splitPoint = (node.BoundingBox.Max.AxisValue(axisIndex) + node.BoundingBox.Min.AxisValue(axisIndex))/2;
			//float splitPoint = (float) (_random.NextDouble()*(centers[centers.Length - 1] - centers[0]) + centers[0]);

			int splitIndex = Array.BinarySearch(centers, splitPoint);
			splitIndex = splitIndex < 0 ? ~splitIndex : splitIndex;


			long leftLenght = splitIndex;
			if (leftLenght > 0)
			{
				var left = CopyArraySection(objects, 0, leftLenght);

				BoundingBox leftBox = CalculateBoundingBox(left);

				node.Left = new BvhNode(leftBox);
				BuildRecursive(node.Left, left, depht + 1);
			}

			long rightLenght = objects.LongLength - splitIndex;
			if (rightLenght > 0)
			{
				var right = CopyArraySection(objects, splitIndex, rightLenght);

				BoundingBox rightBox = CalculateBoundingBox(right);

				node.Right = new BvhNode(rightBox);
				BuildRecursive(node.Right, right, depht + 1);
			}
		}

		private float? FindSplit(BvhNode node, Intersectable[] objects, out int axisIndexa)
		{
			float sP = BoxSurface(node.BoundingBox);

			int bestAxis = MaxAxisIndex(node.BoundingBox);
			float? bestSplit=null;
			float minC = float.MaxValue;

			for (int axis = 0; axis < 3; axis++)
			{
				Array.Sort(objects, (objA, objB) => objA.Center.AxisValue(axis).CompareTo(objB.Center.AxisValue(axis)));
				float[] centers = objects.Select(intersectable => intersectable.Center.AxisValue(axis)).ToArray();

				float min = node.BoundingBox.Min.AxisValue(axis);
				float max = node.BoundingBox.Max.AxisValue(axis);
				float step = (max - min)/50;

				for (float split = min + step; split < max; split += step)
				{
					int splitIndex = Array.BinarySearch(centers, split);
					splitIndex = splitIndex < 0 ? ~splitIndex : splitIndex;

					int nL = splitIndex;
					int nR = objects.Length - splitIndex;

					float sL = BoxSurface(CalculateBoundingBox(objects, 0, nL));
					float sR = BoxSurface(CalculateBoundingBox(objects, splitIndex, nR));

					float c = (sL/sP)*nL + (sR/sP)*nR;

					if (c < minC)
					{
						minC = c;
						bestSplit = split;
						bestAxis = axis;
					}
				}
			}

			if (minC < objects.Length)
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