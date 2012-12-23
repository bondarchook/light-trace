using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class Node
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Matrix Transform { get; set; }
		public Matrix Translation { get; set; }
		public Matrix Ratation { get; set; }
		public Matrix Scale { get; set; }
	}
}