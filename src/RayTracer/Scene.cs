using System.Collections.Generic;
using System.Linq;
using LightTrace.Domain;
using Microsoft.Xna.Framework;
using RayTracer.GeomertryPrimitives;
using RayTracer.OctTreeOptimisation;

namespace RayTracer
{
    public class Scene
    {
        public Vector3 CameraPos { get; set; }
        public Vector3 CameraTarget { get; set; }
        public Vector3 CameraUp { get; set; }
        public float Fov { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int MaxDepth { get; set; }

        public IList<Geomertry> Objects { get; set; }
        public IList<Light> Lights { get; set; }

        public float AttenuationConst { get; set; }
        public float AttenuationLinear { get; set; }
        public float AttenuationQuadratic { get; set; }

        public string OutputFileName { get; set; }

        public bool UseOctTree { get; set; }

        public OctTree Tree { get; set; }

        public Scene()
        {
            Objects = new List<Geomertry>();
            Lights = new List<Light>();

            AttenuationConst = 1.0f;
            AttenuationLinear = 0.0f;
            AttenuationQuadratic = 0.0f;

            MaxDepth = 5;
            UseOctTree = true;
        }

        public void CalculateOctTree()
        {
            OctTreeBuilder builder = new OctTreeBuilder();
            if (Objects.Any())
            {
                Tree = builder.Build(Objects, 3, 50);
            }
        }

        public IList<Geomertry> GetObjects(Ray ray)
        {
            if (UseOctTree)
            {
                IEnumerable<Geomertry> triangles = Tree.GetPotencialObjects(ray);
                return triangles.ToList();
            }
            else
            {
                return Objects;
            }
        }

        public void InitDefaultScene()
        {
            CameraPos = new Vector3(0, 0, -5);
            CameraTarget = new Vector3(0, 0, 0);
            CameraUp = new Vector3(0, 1, 0);
            Width = 200;
            Height = 200;
            Fov = 45;
            MaxDepth = 5;

            AttenuationConst = 1.0f;
            AttenuationLinear = 0.0f;
            AttenuationQuadratic = 0.0f;

            Lights.Add(new Light
                           {
                               Type = LightType.Directional,
                               Color = new Vector3(0.6f, 0.3f, 0f),
                               Position = new Vector3(1, 1, 0)
                           });

            Lights.Add(new Light
                           {
                               Type = LightType.Point,
                               Color = new Vector3(0.0f, 0.3f, 0.6f),
                               Position = new Vector3(-1, 1, 5f)
                           });

            Objects.Add(new Sphere(0, 0, 0, 0.5f)
                            {
                                Transform = Matrix.CreateTranslation(0.7f, 0.7f, 0),
                                Material = new Material
                                               {
                                                   DiffuseColor = new Vector3(0.7f),
                                                   SpecularColor = new Vector3(0.6f),
                                                   EmissionColor = new Vector3(0.0f, 0, 0),
                                                   Shininess = 2000
                                               }
                            });

            Objects.Add(new Sphere(0, 0, 0, 0.5f)
                            {
                                Transform = Matrix.CreateTranslation(-0.7f, 0.7f, 0),
                                Material = new Material
                                               {
                                                   DiffuseColor = new Vector3(0.0f),
                                                   SpecularColor = new Vector3(0.3f, 1.0f, 0.3f),
                                                   Shininess = 2000
                                               }
                            });
            Objects.Add(new Sphere(0, 0, 0, 0.5f)
                            {
                                Transform = Matrix.CreateTranslation(0.7f, -0.7f, 0),
                                Material = new Material
                                               {
                                                   DiffuseColor = new Vector3(0.3f),
                                                   SpecularColor = new Vector3(0.3f, 0.3f, 1.0f),
                                                   Shininess = 2000
                                               }
                            });

            Objects.Add(new Sphere(0, 0, 0, 0.5f)
                            {
                                Transform = Matrix.CreateTranslation(-0.7f, -0.7f, 0),
                                Material = new Material
                                               {
                                                   DiffuseColor = new Vector3(0.0f),
                                                   SpecularColor = new Vector3(0.9f),
                                                   Shininess = 2000
                                               }
                            });

            Objects.Add(new Triangle(new Vector3(0f, 2f, 1f), new Vector3(-1f, -1f, 1f), new Vector3(1f, -1f, 1f))
                            {
                                Na = Vector3.Normalize(new Vector3(0.0f, 0.7f, 0.1f)),
                                Nb = Vector3.Normalize(new Vector3(-0.7f, -0.7f, 0.1f)),
                                Nc = Vector3.Normalize(new Vector3(0.7f, -0.7f, 0.1f)),
                                Transform = Matrix.CreateTranslation(0, 0, 0),
                                Material = new Material
                                               {
                                                   DiffuseColor = new Vector3(0.7f),
                                                   SpecularColor = new Vector3(0.7f),
                                                   Shininess = 10000
                                               }
                            });
        }
    }
}