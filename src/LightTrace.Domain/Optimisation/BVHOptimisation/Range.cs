namespace LightTrace.Domain.Optimisation.BVHOptimisation
{
	public struct Range
	{
		public int Begin;
		public int End;

		public Range(int begin, int end)
		{
			Begin = begin;
			End = end;
		}
	}
}