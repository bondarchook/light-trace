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
		private Triangle[] _triangles;
		private Ray[] _rays;
		private BoundingBox[] _boundingBoxs;

		public void Generate(int count)
		{
			_random = new Random();

			_triangles = Enumerable.Range(0, count).Select(i => new Triangle(RandomVector3(), RandomVector3(), RandomVector3())).ToArray();
			_rays = Enumerable.Range(0, count).Select(i => new Ray(RandomVector3(), RandomVector3())).ToArray();
			_boundingBoxs = Enumerable.Range(0, count).Select(i => new BoundingBox(RandomVector3(), RandomVector3())).ToArray();

			_triangles.ToList().ForEach(t => t.Prepare());
		}

		public float TriangleIntersect()
		{

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < _triangles.Length; i++)
			{
				_triangles[i].Intersect(_rays[i]);
			}

			stopwatch.Stop();

			return stopwatch.ElapsedTicks / (float)_triangles.Length;
		}

		public float BoundingBoxIntersect()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < _boundingBoxs.Length; i++)
			{
				_boundingBoxs[i].Intersects(_rays[i]);
			}

			stopwatch.Stop();

			return stopwatch.ElapsedTicks / (float)_boundingBoxs.Length;
		}

		private Vector3 RandomVector3()
		{
			return new Vector3((float) _random.NextDouble(), (float) _random.NextDouble(), (float) _random.NextDouble());
		}
	}
}