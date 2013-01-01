using System;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;

namespace LightTrace.Domain.Shading
{
	public class Texture2DSampler : ColorSampler
	{
		public Texture Texture { get; set; }
		private object lockObj = new object();


		public Texture2DSampler(Texture texture)
		{
			Texture = texture;
		}

		public override Vector3 GetColor(Vector2 texCoodinates)
		{
			Color color;

			lock (lockObj)
			{
				double u = texCoodinates.X - Math.Floor(texCoodinates.X);
				double v = texCoodinates.Y - Math.Floor(texCoodinates.Y);

				int x = (int) (u*Texture.Image.Width);
				int y = (int) (v*Texture.Image.Height);
				color = Texture.Image.GetPixel(x, y);
			}
			return new Vector3(color.R/(float) 255, color.G/(float) 255, color.B/(float) 255);
		}
	}
}