using System;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain
{
    public static class Transform
    {
        public static Matrix LookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 w = (eye - center);
            w.Normalize();

            Vector3 u = Vector3.Cross(up, w);
            u.Normalize();

            Vector3 v = Vector3.Cross(w, u);

            float xx = Vector3.Dot(u, eye);
            float yy = Vector3.Dot(v, eye);
            float zz = Vector3.Dot(w, eye);

            Matrix ret = new Matrix(u.X, u.Y, u.Z, -xx,
                                    v.X, v.Y, v.Z, -yy,
                                    w.X, w.Y, w.Z, -zz,
                                    0, 0, 0, 1
                );

            return Matrix.Transpose(ret);
        }

        public static Vector3[] LookAtVectors(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 w = (eye - center);
            w.Normalize();

            Vector3 u = Vector3.Cross(up, w);
            u.Normalize();

            Vector3 v = Vector3.Cross(w, u);

            return new[] {u, v, w};
        }

        public static Matrix Rotate(float degrees, Vector3 axis)
        {
            Vector3 ax = Vector3.Normalize(axis);
            float a = (float) -(degrees*Math.PI/180.0);

            Matrix id = Matrix.Identity;

            Matrix m2 = new Matrix(
                ax.X*ax.X, ax.X*ax.Y, ax.X*ax.Z, 0.0f,
                ax.Y*ax.X, ax.Y*ax.Y, ax.Y*ax.Z, 0.0f,
                ax.Z*ax.X, ax.Z*ax.Y, ax.Z*ax.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
                );

            Matrix m3 = new Matrix(
                0, -ax.Z, ax.Y, 0.0f,
                ax.Z, 0, -ax.X, 0.0f,
                -ax.Y, ax.X, 0, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
                );


            Matrix ret = ((float) Math.Cos(a))*id + ((float) (1 - Math.Cos(a)))*m2 + ((float) Math.Sin(a))*m3;
            ret.M44 = 1;

            return ret;
        }

        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            //R = 2*(V dot N)*N - V

            return (-2*Vector3.Dot(vector, normal)*normal) + vector;
        }
    }
}