using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Shading
{
	public abstract class ColorSampler
	{
		public abstract Vector3 GetColor(Vector2 texCoodinates);
	}
}