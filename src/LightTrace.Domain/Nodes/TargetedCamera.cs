using Microsoft.Xna.Framework;

namespace LightTrace.Domain.Nodes
{
	public class TargetedCamera : Camera
	{
		public Vector3 Target;
		public Vector3 Up;

		protected override void InitCameraCoordinateSystem()
		{
			Vector3[] ss = Transform.LookAtVectors(Translation.Translation, Target, Up);

			_uVector = ss[0];
			_vVector = ss[1];
			_wVector = ss[2];
		}
	}
}