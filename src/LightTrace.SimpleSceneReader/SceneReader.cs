using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightTrace.Domain;
using LightTrace.Domain.GeomertryPrimitives;
using LightTrace.Domain.Nodes;
using LightTrace.Domain.Shading;
using Microsoft.Xna.Framework;

namespace LightTrace.SimpleSceneReader
{
	public class SceneReader
	{
		private Scene _scene;
		private List<Vector3> _vertices;
		private List<Vector3> _verticesNormals;
		private Stack<Matrix> _transforms;
		private Material _material;
		private float _attenuationConst = 1;
		private float _attenuationLinear;
		private float _attenuationQuadratic;

		public Scene LoadScene(string fileName)
		{
			_scene = new SimpleScene();
			_scene.Camera = new TargetedCamera();

			_material = new Material();

			_transforms = new Stack<Matrix>();
			_transforms.Push(Matrix.Identity);

			StreamReader reader = new StreamReader(fileName);

			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine().Trim();

				if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
				{
					continue;
				}

				ProcessLine(line.Split(' ').Where(t => !string.IsNullOrEmpty(t)).ToArray());
			}

			return _scene;
		}

		private void ProcessLine(string[] tokens)
		{
			string command = tokens[0];

			TargetedCamera camera = (TargetedCamera) _scene.Camera;
			switch (command)
			{
				case "size":
					{
						camera.Width = tokens.ToInt(1);
						camera.Height = tokens.ToInt(2);
						break;
					}
				case "maxdepth ":
					{
						_scene.MaxDepth = tokens.ToInt(1);
						break;
					}
				case "output":
					{
						_scene.OutputFileName = tokens[1];
						break;
					}
				case "camera":
					{
						camera.Translation = Matrix.CreateTranslation(tokens.ToVec3(1));
						camera.Target = tokens.ToVec3(4);
						camera.Up = tokens.ToVec3(7);

						float yFov = tokens.ToFloat(10);
						camera.Fov = yFov;
						camera.UseXFov = false;
						break;
					}
				case "sphere":
					{
						Sphere sphere = new Sphere(tokens.ToVec3(1), tokens.ToFloat(4))
						                	{
						                		Material = (Material) _material.Clone(),
						                		Transform = _transforms.Peek()
						                	};

						_scene.Geomertries.Add(sphere);
						break;
					}
				case "maxverts":
					{
						_vertices = new List<Vector3>(tokens.ToInt(1));
						break;
					}
				case "maxvertnorms":
					{
						_vertices = new List<Vector3>(tokens.ToInt(1));
						_verticesNormals = new List<Vector3>(tokens.ToInt(1));
						break;
					}
				case "vertex":
					{
						_vertices.Add(tokens.ToVec3(1));
						break;
					}
				case "vertexnormal":
					{
						_vertices.Add(tokens.ToVec3(1));
						_verticesNormals.Add(tokens.ToVec3(4));
						break;
					}
				case "tri":
					{
//						Triangle triangle = new Triangle(_vertices[tokens.ToInt(1)], _vertices[tokens.ToInt(2)], _vertices[tokens.ToInt(3)])
//						                    	{
//						                    		Material = (Material) _material.Clone(),
//						                    		Transform = _transforms.Peek()
//						                    	};
						CalculatedTriangle triangle = new CalculatedTriangle(_vertices[tokens.ToInt(1)], _vertices[tokens.ToInt(2)], _vertices[tokens.ToInt(3)],_transforms.Peek())
						                    	{
						                    		Material = (Material) _material.Clone()
						                    	};

						_scene.Geomertries.Add(triangle);
						break;
					}
				case "trinormal":
					{
						Triangle triangle = new Triangle(_vertices[tokens.ToInt(1)], _vertices[tokens.ToInt(2)], _vertices[tokens.ToInt(3)])
						                    	{
						                    		Na = _verticesNormals[tokens.ToInt(1)],
						                    		Nb = _verticesNormals[tokens.ToInt(2)],
						                    		Nc = _verticesNormals[tokens.ToInt(3)],
						                    		Material = (Material) _material.Clone(),
						                    		Transform = _transforms.Peek()
						                    	};

						_scene.Geomertries.Add(triangle);
						break;
					}
				case "translate":
					{
						Matrix matrix = Matrix.CreateTranslation(tokens.ToVec3(1));
						RightMultiply(matrix);
						break;
					}
				case "rotate":
					{
						Matrix matrix = Transform.Rotate(tokens.ToFloat(4), tokens.ToVec3(1));
						RightMultiply(matrix);
						break;
					}
				case "scale":
					{
						Matrix matrix = Matrix.CreateScale(tokens.ToVec3(1));
						RightMultiply(matrix);
						break;
					}
				case "pushTransform":
					{
						Matrix matrix = _transforms.Peek();
						_transforms.Push(matrix);
						break;
					}
				case "popTransform":
					{
						_transforms.Pop();
						break;
					}
				case "directional":
					{
						DirectionalLight light = new DirectionalLight
						                         	{
						                         		Color = tokens.ToVec3(4)
						                         	};

						Matrix rotation = Matrix.Identity;
						rotation.Backward = tokens.ToVec3(1);
						light.Rotation = rotation;

						_scene.Lights.Add(light);
						break;
					}
				case "point":
					{
						PointLight light = new PointLight
						                   	{
						                   		Color = tokens.ToVec3(4),
						                   		Translation = Matrix.CreateTranslation(tokens.ToVec3(1)),
						                   		AttenuationConst = _attenuationConst,
						                   		AttenuationLinear = _attenuationLinear,
						                   		AttenuationQuadratic = _attenuationQuadratic
						                   	};

						_scene.Lights.Add(light);
						break;
					}
				case "attenuation":
					{
						_attenuationConst = tokens.ToFloat(1);
						_attenuationLinear = tokens.ToFloat(2);
						_attenuationQuadratic = tokens.ToFloat(3);
						break;
					}
				case "ambient":
					{
						_material.AmbientColor = tokens.ToVec3(1);
						break;
					}
				case "diffuse":
					{
						_material.DiffuseColor = new PlaneColorSampler(tokens.ToVec3(1));
						break;
					}
				case "specular":
					{
						_material.SpecularColor = tokens.ToVec3(1);
						_material.ReflectiveColor = tokens.ToVec3(1);
						_material.Reflectivity = 1.0f;
						break;
					}
				case "shininess":
					{
						_material.Shininess = tokens.ToFloat(1);
						break;
					}
				case "emission":
					{
						_material.EmissionColor = tokens.ToVec3(1);
						break;
					}
			}
		}

		private void RightMultiply(Matrix matrix)
		{
			Matrix top = _transforms.Pop();
			top = matrix*top;
			_transforms.Push(top);
		}
	}
}