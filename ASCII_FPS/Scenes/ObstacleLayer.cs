namespace ASCII_FPS.Scenes
{
    public enum ObstacleLayer { Wall, Gap };


    public static class ObstacleLayerMask
    {
        public static int GetMask(params ObstacleLayer[] layers)
        {
            int mask = 0;
            foreach (ObstacleLayer layer in layers)
            {
                mask |= 1 << (int)layer;
            }

            return mask;
        }

        public static bool CheckMask(int mask, ObstacleLayer layer)
        {
            return ((1 << (int)layer) & mask) > 0;
        }

        public static int Everything
        {
            get
            {
                return 0x7fffffff;
            }
        }
    }
}
