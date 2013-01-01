using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Shading
{
	public class PlaneColorSampler:ColorSampler
	{
		public Vector3 Color;

		public PlaneColorSampler(Vector3 color)
		{
			Color = color;
		}

		public override Vector3 GetColor(Vector2 texCoodinates)
		{
			return Color;
		}
	}
}