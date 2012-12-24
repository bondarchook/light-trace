using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public abstract class Light : Node
	{
		public Vector3 Color { get; set; }
		public abstract void Calculate(Vector3 point, out Vector3 direction, out float distance, out float attenuation);
	}
}