using Microsoft.Xna.Framework;

namespace ASCII_FPS
{
	public struct Portal
	{
		public Zone Zone { get; }
		public Vector2 Start { get; }
		public Vector2 End { get; }
		public Vector3 Normal { get; }

		public Portal(Zone zone, Vector2 start, Vector2 end)
		{
			Zone = zone;
			Start = start;
			End = end;

			Normal = Vector3.Cross(new Vector3(end.X, 0f, end.Y) - new Vector3(start.X, 0f, start.Y), Vector3.Up);
		}
	}
}
