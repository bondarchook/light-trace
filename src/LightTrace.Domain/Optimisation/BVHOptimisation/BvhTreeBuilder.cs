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
//			var axisIndex = MaxAxisIndex(node.BoundingBox);
			var axisIndex = 2;

			Array.Sort(objects, (objA, objB) => objA.Center.AxisValue(axisIndex).CompareTo(objB.Center.AxisValue(axisIndex)));
			float[] centers = objects.Select(intersectable => intersectable.Center.AxisValue(axisIndex)).ToArray();

			if (objects.Length < _minObjectsPerLeaf || depht > _maxDepth)
			{
				node.IsLeaf = true;
				node.Objects = objects;
				//Console.WriteLine("{2} {1} {0}", depht, objects.Length, Math.Max(Math.Max(node.BoundingBox.Max.X - node.BoundingBox.Min.X,node.BoundingBox.Max.Y - node.BoundingBox.Min.Y),node.BoundingBox.Max.Z - node.BoundingBox.Min.Z));
				return;
			}

			//float splitPoint = (node.BoundingBox.Max.AxisValue(axisIndex) + node.BoundingBox.Min.AxisValue(axisIndex))/2;
			float splitPoint = (float) (_random.NextDouble()*(centers[centers.Length - 1] - centers[0]) + centers[0]);
			int splitIndex = Array.BinarySearch(centers, splitPoint);

			if (splitIndex < 0)
			{
				splitIndex = ~splitIndex;
			}

//			if (splitIndex == 0)
//				Console.WriteLine("{0} {1} {2} [{3} {4}]", splitPoint, centers[0], centers[centers.Length - 1], node.BoundingBox.Min.AxisValue(axisIndex) , node.BoundingBox.Max.AxisValue(axisIndex));

			long leftLenght = splitIndex;

			if (leftLenght > 0)
			{
				Intersectable[] left = new Intersectable[leftLenght];
				Array.Copy(objects, 0, left, 0, leftLenght);

				BoundingBox leftBox = BoundingBox.CreateFromPoints(left[0].BoundingBox.GetCorners());
				for (long i = 0; i < left.Length; i++)
				{
					//BoundingBox.CreateMerged(ref leftBox, ref left[i].BoundingBox, out leftBox);
					leftBox =  BoundingBox.CreateMerged(leftBox, left[i].BoundingBox);
//					leftBox =  BoundingBox.CreateFromPoints(left.Select(intersectable => intersectable.Center));
				}

				node.Left = new BvhNode(leftBox);
				BuildRecursive(node.Left, left, depht + 1);
			}

			long rightLenght = objects.LongLength - splitIndex;
			if (rightLenght > 0)
			{
				Intersectable[] right = new Intersectable[rightLenght];
				Array.Copy(objects, splitIndex, right, 0, rightLenght);

				BoundingBox rightBox = BoundingBox.CreateFromPoints(right[0].BoundingBox.GetCorners());
				for (int i = 0; i < right.Length; i++)
				{
//					BoundingBox.CreateMerged(ref rightBox, ref right[i].BoundingBox, out rightBox);
					rightBox = BoundingBox.CreateMerged(rightBox, right[i].BoundingBox);
//					rightBox = BoundingBox.CreateFromPoints(right.Select(intersectable => intersectable.Center));

				}

				node.Right = new BvhNode(rightBox);
				BuildRecursive(node.Right, right, depht + 1);
			}
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