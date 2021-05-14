using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.Scenes.Generators
{
    public class SceneGeneratorJungle : SceneGeneratorDefault
    {
        protected override AsciiTexture WallTexture => Assets.jungleWallVinesTexture;

        public SceneGeneratorJungle(ASCII_FPS game, int floor)
            : base(game, floor) { }
    }
}
