using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OBJContentPipelineExtension;
using System;
using System.Reflection;

namespace ASCII_FPS
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AssetPathAttribute : Attribute
    {
        public string Path { get; }

        public AssetPathAttribute(string path)
        {
            Path = path;
        }
    }

    public static class Assets
    {
        [AssetPath("font")] public static SpriteFont font;


        [AssetPath("textures/bricks01")] public static AsciiTexture wallTexture;
        [AssetPath("textures/bricks02")] public static AsciiTexture floorTexture;
        [AssetPath("textures/jungle_bricks")] public static AsciiTexture jungleWallTexture;
        [AssetPath("textures/jungle_bricks_vines")] public static AsciiTexture jungleWallVinesTexture;
        [AssetPath("textures/jungle_ground")] public static AsciiTexture jungleFloorTexture;
        [AssetPath("textures/lava_walls")] public static AsciiTexture lavaWallsTexture;
        [AssetPath("textures/lava_ground")] public static AsciiTexture lavaFloorTexture;
        [AssetPath("textures/lava")] public static AsciiTexture lavaTexture;

        [AssetPath("textures/exit")] public static AsciiTexture exitTexture;

        [AssetPath("textures/barrel_red")] public static AsciiTexture barrelRedTexture;
        [AssetPath("textures/barrel_green")] public static AsciiTexture barrelGreenTexture;
        [AssetPath("textures/barrel_blue")] public static AsciiTexture barrelBlueTexture;
        [AssetPath("textures/bush")] public static AsciiTexture bushTexture;

        [AssetPath("textures/projectile")] public static AsciiTexture projectileTexture;
        [AssetPath("textures/projectile2")] public static AsciiTexture projectile2Texture;
        [AssetPath("textures/projectile3")] public static AsciiTexture projectile3Texture;

        [AssetPath("textures/monster")] public static AsciiTexture monsterTexture;
        [AssetPath("textures/shotgun_dude")] public static AsciiTexture shotgunDudeTexture;
        [AssetPath("textures/spinny_boi")] public static AsciiTexture spinnyBoiTexture;
        [AssetPath("textures/spooper")] public static AsciiTexture spooperTexture;
        [AssetPath("textures/poison_monster")] public static AsciiTexture poisonMonsterTexture;


        [AssetPath("models/barrel")] public static OBJFile barrelModel;
        [AssetPath("models/exit")] public static OBJFile exitModel;
        [AssetPath("models/spooper")] public static OBJFile spooperModel;
        [AssetPath("models/bush")] public static OBJFile bushModel;


        [AssetPath("audio/tsch")] public static SoundEffect tsch;
        [AssetPath("audio/oof")] public static SoundEffect oof;
        [AssetPath("audio/ouch")] public static SoundEffect ouch;
        [AssetPath("audio/theme")] public static SoundEffect theme;
        [AssetPath("audio/btsch")] public static SoundEffect btsch;
        [AssetPath("audio/beep")] public static SoundEffect beep;


        // More reflection hacks -- starting to feel bad about it xD
        public static void LoadAssets(ContentManager content)
        {
            // Gather all Assets fields
            FieldInfo[] fields = typeof(Assets).GetFields();
            MethodInfo loadMethodInfo = content.GetType().GetMethod("Load"); // Content.Load method

            foreach (FieldInfo field in fields)
            {
                string path = field.GetCustomAttribute<AssetPathAttribute>().Path;
                Type type = field.FieldType;
                if (type == typeof(AsciiTexture))
                {
                    type = typeof(Texture2D);
                }

                // Execute Content.Load with appropriate generic argument
                object result = loadMethodInfo.MakeGenericMethod(type).Invoke(content, new object[] { path });
                if (field.FieldType == typeof(AsciiTexture))
                {
                    result = new AsciiTexture((Texture2D)result);
                }

                field.SetValue(path, result);
            }
        }
    }
}
