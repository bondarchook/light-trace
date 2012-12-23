namespace LightTrace.Domain.Nodes
{
	public class PointLight : Ligth
	{
		public float AttenuationConst { get; set; }
		public float AttenuationLinear { get; set; }
		public float AttenuationQuadratic { get; set; }
	}
}