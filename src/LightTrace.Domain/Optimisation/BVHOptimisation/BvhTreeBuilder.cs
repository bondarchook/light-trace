using System;
using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTreeBuilder
	{
		private Intersectable[] _intersectables;
		private float[] _centers;

		private int _maxDepth = 4;
		private int _minObjectsPerLeaf = 2;

		public BvhTree BuildBVHTree(IEnumerable<Geomertry> geomertries, int maxDepth, int minObjPerLeaf)
		{
			_maxDepth = maxDepth;
			_minObjectsPerLeaf = minObjPerLeaf;

			BoundingBox rootBoundingBox = OptimisationUtils.GetBoundingBox(geomertries);

			int maxAxisIndex = 0;
			float longestAxis = float.MinValue;

			for (int i = 0; i < 3; i++)
			{
				float length = rootBoundingBox.Max.AxisValue(i) - rootBoundingBox.Min.AxisValue(i);
				if (length > longestAxis)
				{
					longestAxis = length;
					maxAxisIndex = i;
				}
			}

			List<Intersectable> list = geomertries.Select(Intersectable.CreateIntersectable).ToList();
			list.Sort((objA, objB) => objA.Center.AxisValue(maxAxisIndex).CompareTo(objB.Center.AxisValue(maxAxisIndex)));
			_intersectables = list.ToArray();

			_centers = list.Select(intersectable => intersectable.Center.AxisValue(maxAxisIndex)).ToArray();

			BvhNode root = new BvhNode(rootBoundingBox, 0, _intersectables.Count());
			BuildRecursive(root, maxAxisIndex, 0);

			return new BvhTree(root, _intersectables);
		}

		private void BuildRecursive(BvhNode node, int axisIndex, int depht)
		{
			int leftIndex = node.LeftIndex;
			int rightIndex = node.RightIndex;

			if (rightIndex - leftIndex < _minObjectsPerLeaf || depht > _maxDepth)
			{
				node.IsLeaf = true;
				return;
			}

			float splitPoint = (node.BoundingBox.Max.AxisValue(axisIndex) + node.BoundingBox.Min.AxisValue(axisIndex))/2;
			int splitIndex = Array.BinarySearch(_centers, splitPoint);

			if (splitIndex < 0)
			{
				splitIndex = ~splitIndex;
			}

			BoundingBox leftBox = new BoundingBox();
			for (int i = leftIndex; i < splitIndex; i++)
			{
				BoundingBox.CreateMerged(ref leftBox, ref _intersectables[i].BoundingBox, out leftBox);
			}

			BoundingBox rightBox = new BoundingBox();
			for (int i = splitIndex; i < rightIndex; i++)
			{
				BoundingBox.CreateMerged(ref rightBox, ref _intersectables[i].BoundingBox, out rightBox);
			}

			node.Left = new BvhNode(leftBox, leftIndex, splitIndex);
			node.Right = new BvhNode(rightBox, splitIndex, rightIndex);

			BuildRecursive(node.Left, axisIndex, depht + 1);
			BuildRecursive(node.Right, axisIndex, depht + 1);
		}
	}
}