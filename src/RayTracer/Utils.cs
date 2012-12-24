using System;
using System.Windows.Forms;

namespace RayTracer
{
	public static class Utils
	{
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