using System.Collections.Generic;
using LightTrace.Domain.Nodes;

namespace LightTrace.Domain
{
	public class Scene
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public IList<Node> Nodes { get; set; }

		public Scene()
		{
			Nodes = new List<Node>();
		}
	}
}