using System.Collections.Generic;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;
using System.Linq;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTree
	{
		private BvhNode _root;

		public BvhTree(BvhNode root)
		{
			_root = root;
		}

		public IEnumerable<Geomertry> GetPotencialObjects(Ray ray)
		{
			List<Intersectable[]> result = new List<Intersectable[]>();

			GetObjects(ray, _root, result);

			return CreateEnumerable(result);
		}

		public long GetPotencialObjectsCount(Ray ray)
		{
			List<Intersectable[]> result = new List<Intersectable[]>();

			GetObjects(ray, _root, result);

//			return result.Sum(range => range.LongLength);
			return result.Count;
		}

		private void GetObjects(Ray ray, BvhNode node, List<Intersectable[]> result)
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
						result.Add(leftNode.Objects);
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
						result.Add(rightNode.Objects);
					}
					else
					{
						GetObjects(ray, rightNode, result);
					}
				}
			}
		}

		private IEnumerable<Geomertry> CreateEnumerable(List<Intersectable[]> ranges)
		{
			foreach (Intersectable[] range in ranges)
			{
				for (long i = 0; i < range.LongLength; i++)
				{
					yield return range[i].Geomertry;
				}
			}
		}
	}
}