using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightTrace.Domain;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace RayTracer
{
    public class SceneReader
    {
        private Scene _scene;
        private List<Vector3> _vertices;
        private List<Vector3> _verticesNormals;
        private Stack<Matrix> _transforms;
        private Material _material;

        public Scene LoadScene(string fileName)
        {
            _scene = new Scene();

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

            switch (command)
            {
                case "size":
                    {
                        _scene.Width = tokens.ToInt(1);
                        _scene.Height = tokens.ToInt(2);
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
                        _scene.CameraPos = tokens.ToVec3(1);
                        _scene.CameraTarget = tokens.ToVec3(4);
                        _scene.CameraUp = tokens.ToVec3(7);
                        _scene.Fov = tokens.ToFloat(10);
                        break;
                    }
                case "sphere":
                    {
                        Sphere sphere = new Sphere(tokens.ToVec3(1), tokens.ToFloat(4))
                                            {
                                                Material = (Material) _material.Clone(),
                                                Transform = _transforms.Peek()
                                            };

                        _scene.Objects.Add(sphere);
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
                        Triangle triangle = new Triangle(_vertices[tokens.ToInt(1)], _vertices[tokens.ToInt(2)], _vertices[tokens.ToInt(3)])
                                                {
                                                    Material = (Material) _material.Clone(),
                                                    Transform = _transforms.Peek()
                                                };

                        _scene.Objects.Add(triangle);
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

                        _scene.Objects.Add(triangle);
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
                        Light light = new Light
                                          {
                                              Type = LightType.Directional,
                                              Position = tokens.ToVec3(1),
                                              Color = tokens.ToVec3(4)
                                          };
                        _scene.Lights.Add(light);
                        break;
                    }
                case "point":
                    {
                        Light light = new Light
                                          {
                                              Type = LightType.Point,
                                              Position = tokens.ToVec3(1),
                                              Color = tokens.ToVec3(4)
                                          };
                        _scene.Lights.Add(light);
                        break;
                    }
                case "attenuation":
                    {
                        _scene.AttenuationConst = tokens.ToFloat(1);
                        _scene.AttenuationLinear = tokens.ToFloat(2);
                        _scene.AttenuationQuadratic = tokens.ToFloat(3);
                        break;
                    }
                case "ambient":
                    {
                        _material.AmbientColor = tokens.ToVec3(1);
                        break;
                    }
                case "diffuse":
                    {
                        _material.DiffuseColor = tokens.ToVec3(1);
                        break;
                    }
                case "specular":
                    {
                        _material.SpecularColor = tokens.ToVec3(1);
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