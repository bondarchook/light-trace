using System;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain
{
    public class Material : ICloneable
    {
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public Vector3 EmissionColor { get; set; }
        public Vector3 AmbientColor { get; set; }
        public float Shininess { get; set; }

        public Material()
        {
            AmbientColor = new Vector3(0.2f);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}