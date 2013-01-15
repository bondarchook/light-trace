using System;
using System.Collections.Generic;
using System.Text;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Optimisation.OctTreeOptimisation;
using Microsoft.Xna.Framework;
using System.Linq;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhTree
	{
		private BvhNode _root;

        private int _minCount = int.MaxValue;
        private int _maxCount = int.MinValue;
        private int _totalCount;
        private int _avrCount;
        private int _leafCount;

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

        public string GetStatistics()
        {
            CalculateStatistic(_root);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Min count in leaf = {0}", _minCount));
            builder.AppendLine(string.Format("Max count in leaf = {0}", _maxCount));
            builder.AppendLine(string.Format("Avr count in leaf = {0}", _avrCount));
            builder.AppendLine(string.Format("Total count in leaf = {0}", _totalCount));
            builder.AppendLine(string.Format("Leaf count = {0}", _leafCount));

            return builder.ToString();
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
			return from range in ranges from intersectable in range select intersectable.Geomertry;

			//			foreach (Intersectable[] range in ranges)
//			{
//				for (long i = 0; i < range.LongLength; i++)
//				{
//					yield return range[i].Geomertry;
//				}
//			}
		}

		public void CalculateStatistic(BvhNode node)
	    {
	        if (node.IsLeaf)
	        {
	            _minCount = Math.Min(_minCount, node.Objects.Length);
	            _maxCount = Math.Max(_maxCount, node.Objects.Length);
	            _totalCount += node.Objects.Length;
	            _leafCount++;
	        }
	        else
	        {
	            if (node.Left != null)
	                CalculateStatistic(node.Left);

	            if (node.Right != null)
	                CalculateStatistic(node.Right);
	        }
			if (_leafCount != 0)
			{
				_avrCount = _totalCount / _leafCount;
			}
	    }
	}
}