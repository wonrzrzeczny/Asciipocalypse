using Microsoft.Xna.Framework;

namespace ASCII_FPS.Scenes
{
    public struct Obstacle
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }
        public ObstacleLayer Layer { get; }


        public Obstacle(Vector2 start, Vector2 end, ObstacleLayer layer)
        {
            Start = start;
            End = end;
            Layer = layer;
        }
    }
}
