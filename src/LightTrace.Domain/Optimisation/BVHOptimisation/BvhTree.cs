using System.Collections.Generic;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;
using System.Linq;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTree
	{
		private BvhNode _root;
		private readonly Intersectable[] _intersectables;

		public BvhTree(BvhNode root, Intersectable[] intersectables)
		{
			_root = root;
			_intersectables = intersectables;
		}

		public IEnumerable<Geomertry> GetPotencialObjects(Ray ray)
		{
			List<Range> result = new List<Range>();

			GetObjects(ray, _root, result);

			return CreateEnumerable(result);
		}

		public long GetPotencialObjectsCount(Ray ray)
		{
			List<Range> result = new List<Range>();

			GetObjects(ray, _root, result);

			return result.Sum(range => range.End - range.Begin);
		}

		private void GetObjects(Ray ray, BvhNode node, List<Range> result)
		{
			if (!ray.Intersects(node.BoundingBox).HasValue)
				return;

			float? leftIntersection = null;
			float? rightIntersection = null;

			BvhNode leftNode = node.Left;
			BvhNode rightNode = node.Right;

			if (leftNode != null)
			{
				leftIntersection = ray.Intersects(leftNode.BoundingBox);
				if (leftIntersection.HasValue)
				{
					if (leftNode.IsLeaf)
					{
						result.Add(new Range(leftNode.LeftIndex, leftNode.RightIndex));
					}
					else
					{
						GetObjects(ray, leftNode, result);
					}
				}
			}

			if (rightNode != null)
			{
				rightIntersection = ray.Intersects(rightNode.BoundingBox);
				if (rightIntersection.HasValue)
				{
					if (rightNode.IsLeaf)
					{
						result.Add(new Range(rightNode.LeftIndex, rightNode.RightIndex));
					}
					else
					{
						GetObjects(ray, rightNode, result);
					}
				}
			}
		}

		private IEnumerable<Geomertry> CreateEnumerable(IList<Range> ranges)
		{
			foreach (Range range in ranges)
			{
				for (int i = range.Begin; i < range.End; i++)
				{
					yield return _intersectables[i].Geomertry;
				}
			}
		}
	}
}