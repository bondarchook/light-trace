using System;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class Camera : Node
	{
		public float XFov;
		public double AspectRatio;
		public int Width;
		public int Height;

		protected double _fovXRadians;
		protected double _halfWidth;
		protected double _halfHeight;
		protected Vector3 _uVector;
		protected Vector3 _vVector;
		protected Vector3 _wVector;

		public Camera()
		{
			Width = 700;
			Height = 450;
		}

		public virtual void PrepareCamera()
		{
			AspectRatio = Height/(double) Width;
			_halfWidth = Width/2.0;
			_halfHeight = Height/2.0;
			_fovXRadians = XFov*Math.PI/180.0;
			InitCameraCoordinateSystem();
		}

		protected virtual void InitCameraCoordinateSystem()
		{
			_uVector = Rotation.Right;
			_vVector = Rotation.Up;
			_wVector = Rotation.Backward;
		}

		public Ray CreatePrimaryRay(int x, int y)
		{
			double alfa = Math.Tan(_fovXRadians / 2.0) * ((x + 0.5f - _halfWidth) / _halfWidth);
			double beta = Math.Tan(_fovXRadians / 2.0) * ((_halfHeight - y - 0.5f) / _halfHeight) * (AspectRatio);

			Vector3 rayDir = (float) alfa*_uVector + (float) beta*_vVector - _wVector;

			rayDir.Normalize();

			return new Ray(Translation.Translation, rayDir);
		}
	}
}