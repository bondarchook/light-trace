using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class PointLight : Light
	{
		public float AttenuationConst { get; set; }
		public float AttenuationLinear { get; set; }
		public float AttenuationQuadratic { get; set; }

		public override void Calculate(Vector3 point, out Vector3 direction, out float distance, out float attenuation)
		{
			distance = (point - Translation.Translation).Length();
			attenuation = AttenuationConst + (AttenuationLinear*distance) + (AttenuationQuadratic*distance*distance);
			direction = Vector3.Normalize((Translation.Translation - point));
		}
	}
}