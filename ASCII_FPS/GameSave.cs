using ASCII_FPS.Input;
using ASCII_FPS.GameComponents;
using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;

namespace ASCII_FPS
{
    public static class GameSave
    {
        public class BadVersionException : Exception
        {
            public BadVersionException(string saveVersionID)
            {
                SaveVersionID = saveVersionID;
            }

            public string SaveVersionID { get; }
        }


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


        public static void WriteRectangleF(BinaryWriter writer, RectangleF rectangleF)
        {
            writer.Write(rectangleF.X);
            writer.Write(rectangleF.Y);
            writer.Write(rectangleF.Width);
            writer.Write(rectangleF.Height);
        }

        public static RectangleF ReadRectangleF(BinaryReader reader)
        {
            return new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }



        public static void SaveGame(ASCII_FPS game)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("./scene.sav", FileMode.Create)))
            {
                writer.Write(ASCII_FPS.VERSION);
                game.Scene.Save(writer);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open("./player.sav", FileMode.Create)))
            {
                writer.Write(ASCII_FPS.VERSION);
                game.PlayerStats.Save(writer);
            }
        }

        public static void SaveOptions(GraphicsDeviceManager graphics)
        {
            using (StreamWriter writer = new StreamWriter(File.Open("./options.dat", FileMode.Create)))
            {
                writer.WriteLine(ASCII_FPS.VERSION);

                writer.WriteLine("Resolution Width=" + graphics.PreferredBackBufferWidth);
                writer.WriteLine("Resolution Height=" + graphics.PreferredBackBufferHeight);
                writer.WriteLine("Difficulty=" + ASCII_FPS.Difficulty);
                writer.WriteLine("EyeEasy=" + Rasterizer.EyeEasy);
                writer.WriteLine("Fullscreen=" + graphics.IsFullScreen);
                
                // Reflections hacks once more
                foreach (FieldInfo fieldInfo in typeof(Keybinds).GetFields())
                {
                    writer.WriteLine("Keybind " + fieldInfo.Name + "=" + ((Keybind)fieldInfo.GetValue(null)).Serialize());
                }
            }
        }


        public static Scene LoadGameScene(ASCII_FPS game)
        {
            Scene scene;
            using (BinaryReader reader = new BinaryReader(File.Open("./scene.sav", FileMode.Open)))
            {
                string saveVersionID = reader.ReadString();
                if (saveVersionID != ASCII_FPS.VERSION)
                {
                    throw new BadVersionException(saveVersionID);
                }

                scene = Scene.Load(reader, game);
            }
            return scene;
        }

        public static PlayerStats LoadGameStats()
        {
            PlayerStats playerStats = new PlayerStats();
            using (BinaryReader reader = new BinaryReader(File.Open("./player.sav", FileMode.Open)))
            {
                string saveVersionID = reader.ReadString();
                if (saveVersionID != ASCII_FPS.VERSION)
                {
                    throw new BadVersionException(saveVersionID);
                }

                playerStats.Load(reader);
            }
            return playerStats;
        }

        public static bool LoadOptions(GraphicsDeviceManager graphics)
        {
            if (!File.Exists("./options.dat"))
                return false;
            
            using (StreamReader reader = new StreamReader(File.Open("./options.dat", FileMode.Open)))
            {
                if (reader.ReadLine() != ASCII_FPS.VERSION)
                {
                    return false;
                }

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split('=');
                    if (line.Length != 2)
                        continue;

                    string key = line[0];
                    string data = line[1];

                    if (key.StartsWith("Keybind"))
                    {
                        string name = key.Substring(8);
                        if (Keybind.TryParse(data, out Keybind keybind))
                            typeof(Keybinds).GetField(name).SetValue(null, keybind);
                    }
                    else switch(key)
                        {
                            case "Resolution Width":
                                graphics.PreferredBackBufferWidth = int.Parse(data);
                                break;
                            case "Resolution Height":
                                graphics.PreferredBackBufferHeight = int.Parse(data);
                                break;
                            case "Difficulty":
                                ASCII_FPS.Difficulty = int.Parse(data);
                                break;
                            case "EyeEasy":
                                Rasterizer.EyeEasy = bool.Parse(data);
                                break;
                            case "Fullscreen":
                                graphics.IsFullScreen = bool.Parse(data);
                                break;
                        }
                }
            }

            graphics.ApplyChanges();
            return true;
        }
    }
}
