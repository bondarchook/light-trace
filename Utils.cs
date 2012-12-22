using System;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace RayTracer
{
    public static class Utils
    {
        public static Vector4 ToV4(this Vector3 vector)
        {
            return new Vector4(vector, 1);
        }

        public static Vector4 ToV4(this Vector3 vector, float w)
        {
            return new Vector4(vector, w);
        }

        public static Vector3 ToV3(this Vector4 vector)
        {
            return new Vector3(vector.X/vector.W, vector.Y/vector.W, vector.Z/vector.W);
        }

        public static int ToInt(this string[] tokens, int offset)
        {
            return int.Parse(tokens[offset]);
        }

        public static float ToFloat(this string[] tokens, int offset)
        {
            string s = tokens[offset];
            return float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public static Vector3 ToVec3(this string[] tokens, int offset)
        {
            return new Vector3(tokens.ToFloat(offset), tokens.ToFloat(offset + 1), tokens.ToFloat(offset + 2));
        }

        public static void Manipulate<T>(this T control, Action<T> action) where T : Control
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<T, Action<T>>(Manipulate),
                            new object[] { control, action });
            }
            else
            { action(control); }
        }

    }
}