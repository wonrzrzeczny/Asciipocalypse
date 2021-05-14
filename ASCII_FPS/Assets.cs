using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OBJContentPipelineExtension;

namespace ASCII_FPS
{
    public static class Assets
    {
        public static SpriteFont font;

        public static AsciiTexture texture1, texture2;
        public static AsciiTexture exitTexture;
        public static AsciiTexture barrelRedTexture, barrelGreenTexture, barrelBlueTexture;
        public static AsciiTexture projectileTexture, projectile2Texture;
        public static AsciiTexture monsterTexture, shotgunDudeTexture, spinnyBoiTexture, spooperTexture;

        public static OBJFile barrelModel, exitModel, spooperModel;

        public static SoundEffect tsch, oof, ouch, theme, btsch, beep;
    }
}
