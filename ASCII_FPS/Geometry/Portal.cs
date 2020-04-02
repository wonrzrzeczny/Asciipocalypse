using Microsoft.Xna.Framework;

namespace ASCII_FPS
{
	public struct Portal
	{
		public Zone Zone { get; }
		public Vector2 Start { get; }
		public Vector2 End { get; }

		public Portal(Zone zone, Vector2 start, Vector2 end)
		{
			Zone = zone;
			Start = start;
			End = end;
		}
	}
}
