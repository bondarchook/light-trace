using System.Globalization;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain
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

		public static Vector3 ToVec3(this float[] array, int offset)
		{
			return new Vector3(array[offset*3], array[offset*3 + 1], array[offset*3 + 2]);
		}

		public static Vector2 ToVec2(this float[] array, int offset)
		{
			return new Vector2(array[offset*2], array[offset*2 + 1]);
		}

		public static Vector3 SwapAxis(this Vector3 vector)
		{
			return new Vector3(vector.X, vector.Z, vector.Y);
		}
	}
}