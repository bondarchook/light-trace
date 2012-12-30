using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightTrace.Domain;
using Microsoft.Xna.Framework;
using RayTracer.UI;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace RayTracer
{
	public class Renderer
	{
		private readonly Scene _scene;
		private readonly Bitmap _bitmap;
		private readonly Tracer.RayTracer _rayTracer;

		private readonly object _lockObj = new object();
		private readonly ILoger _loger;
		private readonly PictureBox _pictureBox;

		public Scene Scene
		{
			get { return _scene; }
		}

		public Renderer(PictureBox pictureBox, Scene scene)
		{
			_loger = Context.Instance.Loger;

			_scene = scene;
			_scene.PrepareScene();

			//_scene.CalculateOctTree();

			//_loger.Log(Level.Verbose, _scene.Tree.GetStatistics());

			_pictureBox = pictureBox;

			int width = _scene.Camera.Width;
			int height = _scene.Camera.Height;

			pictureBox.Width = width;
			pictureBox.Height = height;
			_bitmap = new Bitmap(width, height);
			pictureBox.Image = _bitmap;

			_rayTracer = new Tracer.RayTracer(_scene);
		}

		public void Render()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			var partitions = Partitioner.Create(0, _scene.Camera.Width, 80);

			Parallel.ForEach(partitions, RenderTile);

			stopwatch.Stop();

			_loger.Log(Level.Info, string.Format("Render time: {0} ms", stopwatch.ElapsedMilliseconds));
		}

		private void RenderTile(Tuple<int, int> range, ParallelLoopState state)
		{
			_loger.Log(Level.Verbose, string.Format("Started tile {0} {1}", range.Item1, range.Item2));

			int height = _scene.Camera.Height;

			Bitmap buffer = new Bitmap(range.Item2 - range.Item1, height);

			for (int x = range.Item1; x < range.Item2; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var finalColor = _rayTracer.CalculatePixelColor(x, y);
					SetPixelColor(x - range.Item1, y, finalColor, buffer);
				}
			}

			CopyBitmap(buffer, _bitmap, range.Item1);

			_pictureBox.Manipulate(p => p.Refresh());

			_loger.Log(Level.Verbose, string.Format("Finished {0} {1}", range.Item1, range.Item2));
		}

		private void SetPixelColor(int x, int y, Vector3 finalColor, Bitmap buffer)
		{
			var red = (int) (finalColor.X*255);
			var green = (int) (finalColor.Y*255);
			var blue = (int) (finalColor.Z*255);

			if (red < 0 || green < 0 || blue < 0)
			{
				buffer.SetPixel(x, y, Color.Fuchsia);
			}
			else
			{
				buffer.SetPixel(x, y, Color.FromArgb(Math.Min(red, 255), Math.Min(green, 255), Math.Min(blue, 255)));
			}
		}

		private void CopyBitmap(Bitmap srcBitmap, Bitmap destBitmap, int offset)
		{
			lock (_lockObj)
			{
				Rectangle srcArea = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
				BitmapData srcData = srcBitmap.LockBits(srcArea, ImageLockMode.ReadOnly, destBitmap.PixelFormat);
				Rectangle destArea = new Rectangle(offset, 0, srcBitmap.Width, srcBitmap.Height);
				BitmapData destData = destBitmap.LockBits(destArea, ImageLockMode.WriteOnly, destBitmap.PixelFormat);

				IntPtr srcPtr = srcData.Scan0;
				IntPtr destPtr = destData.Scan0;
				byte[] buffer = new byte[srcData.Stride];
				for (int i = 0; i < srcData.Height; ++i)
				{
					Marshal.Copy(srcPtr, buffer, 0, buffer.Length);
					Marshal.Copy(buffer, 0, destPtr, buffer.Length);

					srcPtr += srcData.Stride;
					destPtr += destData.Stride;
				}

				srcBitmap.UnlockBits(srcData);
				destBitmap.UnlockBits(destData);
			}
		}
	}
}