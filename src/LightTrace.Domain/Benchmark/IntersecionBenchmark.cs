using System;
using System.Diagnostics;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Benchmark
{
	public class IntersecionBenchmark
	{
		private Random _random;

		public float TriangleIntersect(int count)
		{
			_random = new Random();

			var triangles = Enumerable.Range(0, count).Select(i => new Triangle(RandomVector3(), RandomVector3(), RandomVector3())).ToArray();
			var rays = Enumerable.Range(0, count).Select(i => new Ray(RandomVector3(), RandomVector3())).ToArray();

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < count; i++)
			{
				triangles[i].Intersect(rays[i]);
			}

			stopwatch.Stop();

			return stopwatch.ElapsedTicks/(float) count;
		}

		public float BoundingBoxIntersect(int count)
		{
			_random = new Random();

			var boundingBoxs = Enumerable.Range(0, count).Select(i => new BoundingBox(RandomVector3(), RandomVector3())).ToArray();
			var rays = Enumerable.Range(0, count).Select(i => new Ray(RandomVector3(), RandomVector3())).ToArray();

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < count; i++)
			{
				boundingBoxs[i].Intersects(rays[i]);
			}

			stopwatch.Stop();

			return stopwatch.ElapsedTicks/(float) count;
		}

		private Vector3 RandomVector3()
		{
			return new Vector3((float) _random.NextDouble(), (float) _random.NextDouble(), (float) _random.NextDouble());
		}
	}
}