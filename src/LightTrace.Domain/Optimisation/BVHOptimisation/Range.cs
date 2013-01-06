namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public struct Range
	{
		public int Axis;
		public long Begin;
		public long End;

		public Range(long begin, long end, int axis)
		{
			Begin = begin;
			End = end;
			Axis = axis;
		}
	}
}