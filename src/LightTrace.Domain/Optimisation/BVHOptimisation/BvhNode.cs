using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhNode
	{
		public BoundingBox BoundingBox;
		public readonly int LeftIndex;
		public readonly int RightIndex;
		public bool IsLeaf;
		
		public BvhNode Left;
		public BvhNode Right;

		public BvhNode()
		{
		}

		public BvhNode(BoundingBox boundingBox, int leftIndex, int rightIndex)
		{
			BoundingBox = boundingBox;
			LeftIndex = leftIndex;
			RightIndex = rightIndex;
		}
	}
}