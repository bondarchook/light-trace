using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightTrace.ColladaReader;
using LightTrace.Domain;
using LightTrace.SimpleSceneReader;

namespace RayTracer.UI
{
	public partial class MainForm : Form
	{
		private Renderer _renderer;

		public MainForm()
		{
			InitializeComponent();
		}

		private void GoButton_Click(object sender, EventArgs e)
		{
			Task task = new Task(() => _renderer.Render());
			task.Start();
		}

		private void ExitButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Context.Instance.Loger = new ListBoxLog(LogListBox);


			SceneReader reader = new SceneReader();
//			Scene scene = reader.LoadScene(@"g:\X-Files\Dropbox\Dropbox\3D_course\hw3-submissionscenes.a13d1de57b76\hw3-submissionscenes\test.test");
//			LightTrace.Domain.Scene scene = reader.LoadScene(@"g:\X-Files\Dropbox\Dropbox\3D_course\hw3-submissionscenes.a13d1de57b76\hw3-submissionscenes\scene4-ambient.test");
//			LightTrace.Domain.Scene scene = reader.LoadScene(@"g:\X-Files\Dropbox\Dropbox\3D_course\hw3-submissionscenes.a13d1de57b76\hw3-submissionscenes\scene4-specular.test");
//			Scene scene = reader.LoadScene(@"g:\X-Files\Dropbox\Dropbox\3D_course\hw3-submissionscenes.a13d1de57b76\hw3-submissionscenes\scene7.test");
//            Scene scene = reader.LoadScene(@"d:\tmp\scene7.test");

//			scene.Width = 160;
//			scene.Height = 120;
//			scene.Fov = 55;
//			scene.MaxDepth = 0;

			ColladaSceneReader reader2 = new ColladaSceneReader();
			Scene scene = reader2.Load(@"g:\X-Files\Art\3D\blender\test\RayTracerTests\test3.dae");

			_renderer = new Renderer(pictureBox, scene);
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			string fileName = Path.GetFileNameWithoutExtension(_renderer.Scene.OutputFileName);
//            string path = @"G:\X-Files\Dropbox\Dropbox\3D_course\hw3-submissionscenes.a13d1de57b76\hw3-submissionscenes\out\";
			string path = @"d:\tmp\";
			pictureBox.Image.Save(Path.Combine(path, fileName + ".png"), ImageFormat.Png);
		}
	}
}