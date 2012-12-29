using System;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class Camera : Node
	{
		public float Fov;
		public double XRatio;
		public double YRatio;
		public int Width;
		public int Height;
		public bool UseXFov;

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
			if (UseXFov)
			{
				XRatio = 1;
				YRatio = Height/(double) Width;
			}
			else
			{
				XRatio = Width/(double) Height;
				YRatio = 1;
			}

			_halfWidth = Width/2.0;
			_halfHeight = Height/2.0;
			_fovXRadians = Fov*Math.PI/180.0;
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
			double alfa = Math.Tan(_fovXRadians/2.0)*((x + 0.5f - _halfWidth)/_halfWidth)*XRatio;
			double beta = Math.Tan(_fovXRadians/2.0)*((_halfHeight - y - 0.5f)/_halfHeight)*YRatio;

			Vector3 rayDir = (float) alfa*_uVector + (float) beta*_vVector - _wVector;

			rayDir.Normalize();

			return new Ray(Translation.Translation, rayDir);
		}
	}
}