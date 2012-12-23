using System;
using System.Windows.Forms;
using LightTrace.ColladaReader;
using RayTracer.UI;

namespace RayTracer
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
			ColladaSceneReader reader = new ColladaSceneReader();
			reader.Load(@"d:\tmp\s1.dae");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}