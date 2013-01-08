namespace RayTracer.UI
{
	public interface ILoger
	{
		void Log(string message);
		void Log(string format, params object[] args);
		void Log(Level level, string format, params object[] args);
		void Log(Level level, string message);
		bool Paused { get; set; }
	}
}