using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace LightTrace.ColladaReader
{
	public static class Utils
	{
		public static int ToInt(this string[] tokens, int offset)
		{
			return int.Parse(tokens[offset]);
		}

		public static int ToInt(this string str, int offset)
		{
			string[] tokens = str.Split(' ');
			return ToInt(tokens, offset);
		}

		public static int ToInt(this string str)
		{
			return ToInt(str, 0);
		}

		public static float ToFloat(this string str, int offset)
		{
			string[] tokens = str.Split(' ');
			return ToFloat(tokens, offset);
		}

		public static float ToFloat(this string[] tokens, int offset)
		{
			string s = tokens[offset];
			return float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		public static float ToFloat(this string str)
		{
			return ToFloat(str, 0);
		}

		public static Vector3 ToVec3(this string str)
		{
			return ToVec3(str, 0);
		}

		public static Vector3 ToVec3(this string str, int offset)
		{
			string[] tokens = str.Split(' ');
			return new Vector3(tokens.ToFloat(offset), tokens.ToFloat(offset + 1), tokens.ToFloat(offset + 2));
		}

		public static Vector3 SwapAxis(this Vector3 vector)
		{
			return new Vector3(vector.X, vector.Z, vector.Y);
		}

		public static int[] ToIntArray(this string str)
		{
			string[] tokens = str.Split(' ').Where(t => !string.IsNullOrEmpty(t)).ToArray();
			return tokens.Select(t => int.Parse(t)).ToArray();
		}

		public static float[] ToFloatArray(this string str)
		{
			string[] tokens = str.Split(' ').Where(t => !string.IsNullOrEmpty(t)).ToArray();
			return tokens.Select(t => float.Parse(t, NumberStyles.Float, CultureInfo.InvariantCulture)).ToArray();
		}

		public static string GetAttributeValue(this XElement element, string attributeName, bool mondatory)
		{
			XAttribute attribute = element.Attribute(attributeName);

			if (attribute != null)
			{
				return attribute.Value;
			}
			else
			{
				if (mondatory)
					throw new Exception("Mandatory attribute {0} not found");
				else
				{
					return null;
				}
			}
		}

		public static int? GetAttributeIntValue(this XElement element, string attributeName, bool mondatory)
		{
			string value = GetAttributeValue(element, attributeName, mondatory);
			return value != null ? int.Parse(value) : (int?) null;
		}

		public static int GetMandatoryAttributeIntValue(this XElement element, string attributeName)
		{
			return (int) GetAttributeIntValue(element, attributeName, true);
		}
	}
}