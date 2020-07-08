using ASCII_FPS.GameComponents;
using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS
{
    public static class GameSave
    {
        public static void WriteVector3(BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static Vector3 ReadVector3(BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }


        public static void WriteVector2(BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static Vector2 ReadVector2(BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }



        public static void SaveGame(Scene scene)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("./scene.sav", FileMode.Create)))
            {
                scene.Save(writer);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open("./player.sav", FileMode.Create)))
            {
                ASCII_FPS.playerStats.Save(writer);
            }
        }

        public static Scene LoadGameScene()
        {
            Scene scene;
            using (BinaryReader reader = new BinaryReader(File.Open("./scene.sav", FileMode.Open)))
            {
                scene = Scene.Load(reader);
            }
            return scene;
        }

        public static void LoadGameStats()
        {
            using (BinaryReader reader = new BinaryReader(File.Open("./player.sav", FileMode.Open)))
            {
                ASCII_FPS.playerStats.Load(reader);
            }
        }
    }
}
