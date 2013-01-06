using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public class BvhNode
	{
		public BoundingBox BoundingBox;
		public bool IsLeaf;
		public Intersectable[] Objects;
		public Vector3 Color;
		
		public BvhNode Left;
		public BvhNode Right;

		public BvhNode(BoundingBox boundingBox)
		{
			BoundingBox = boundingBox;
		}
	}
}