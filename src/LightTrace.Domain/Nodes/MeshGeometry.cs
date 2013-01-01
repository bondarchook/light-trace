using System;
using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Shading;

namespace LightTrace.Domain.Nodes
{
	public class MeshGeometry : Node
	{
		public IList<Triangle> Triangles;
		public Material Material;

		public void BuildMesh(int[] vertexCounts, int[] poligonIndexes, float[] vertices, float[] normals, float[] texcoord, Material material)
		{
			//Only tiangles supported for now
			if (!vertexCounts.All(c => c == 3))
				throw new Exception("Only tiangles supported for now");

			int poligonCount = vertexCounts.Count();
			Triangles = new List<Triangle>(poligonCount);

			int poligonComponentsCount = texcoord != null ? 3 : 2;

			for (int i = 0; i < poligonCount; i++)
			{
				Triangle triangle = new Triangle();
				triangle.Material = material;

				int polygonOffset = i*vertexCounts[i]*poligonComponentsCount;

				int vertexBOffset = 1*poligonComponentsCount;
				int vertexCOffset = 2*poligonComponentsCount;

				triangle.A = vertices.ToVec3(poligonIndexes[polygonOffset]);
				triangle.B = vertices.ToVec3(poligonIndexes[polygonOffset + vertexBOffset]);
				triangle.C = vertices.ToVec3(poligonIndexes[polygonOffset + vertexCOffset]);

				int normalOffset = polygonOffset + 1;

				triangle.Na = normals.ToVec3(poligonIndexes[normalOffset]);
				triangle.Nb = normals.ToVec3(poligonIndexes[normalOffset + vertexBOffset]);
				triangle.Nc = normals.ToVec3(poligonIndexes[normalOffset + vertexCOffset]);

				if (texcoord != null)
				{
					int texcoordOffset = polygonOffset + 2;

					triangle.Ta = texcoord.ToVec2(poligonIndexes[texcoordOffset]);
					triangle.Tb = texcoord.ToVec2(poligonIndexes[texcoordOffset + vertexBOffset]);
					triangle.Tc = texcoord.ToVec2(poligonIndexes[texcoordOffset + vertexCOffset]);
				}

				Triangles.Add(triangle);
			}
		}
	}
}