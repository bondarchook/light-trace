﻿using RayTracer.UI;

namespace RayTracer
{
	public class Context
	{
		public ILoger Loger { get; set; }

		private static Context _context;

		public static Context Instance
		{
			get
			{
				if (_context == null)
				{
					_context = new Context();
				}

				return _context;
			}
		}
	}
}