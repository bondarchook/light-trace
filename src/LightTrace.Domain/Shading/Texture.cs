using System.Drawing;

namespace LightTrace.Domain.Shading
{
	public class Texture
	{
		public string FileName { get; set; }
		public Bitmap Image;


		public Texture(string fileName)
		{
			FileName = fileName;
		}

		public void PrepareTexture()
		{
			Image = (Bitmap)System.Drawing.Image.FromFile(FileName);
		}
	}
}