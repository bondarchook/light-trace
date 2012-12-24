using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class Node
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Matrix Translation { get; set; }
		public Matrix Rotation { get; set; }
		public Matrix Scale { get; set; }
	}
}