using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class DirectionalLight : Light
	{
		public override void Calculate(Vector3 point, out Vector3 direction, out float distance, out float attenuation)
		{
			attenuation = 1.0f;
			distance = 0;
			direction = Vector3.Normalize(Rotation.Forward);
		}
	}
}