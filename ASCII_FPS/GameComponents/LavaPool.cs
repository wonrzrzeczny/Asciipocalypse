using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class LavaPool : GameObject
    {
        private LavaPool(MeshObject meshObject) : base(meshObject) { }

        private RectangleF bounds;
        private float soundTimer = 0.25f;

        public static LavaPool Create(Vector2 v0, Vector2 v1)
        {
            float left = Math.Min(v0.X, v1.X);
            float right = Math.Max(v0.X, v1.X);
            float bottom = Math.Min(v0.Y, v1.Y);
            float top = Math.Max(v0.Y, v1.Y);
            MeshObject meshObject = SceneGenUtils.MakeFloor(left, right, bottom, top, -3.9f, Assets.lavaTexture, true, 50f);

            return new LavaPool(meshObject)
            {
                bounds = new RectangleF(left, bottom, right - left, top - bottom)
            };
        }


        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.LavaPoolLoader).FullName);
            GameSave.WriteVector2(writer, new Vector2(bounds.X, bounds.Y));
            GameSave.WriteVector2(writer, new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));
        }

        public override void Update(float deltaTime)
        {
            Vector2 camPos = new Vector2(Camera.CameraPos.X, Camera.CameraPos.Z);
            if (soundTimer >= 0f)
            {
                soundTimer -= deltaTime;
            }
                
            if (bounds.TestPoint(camPos))
            {
                if (soundTimer < 0f)
                {
                    soundTimer = 0.25f;
                    Assets.burn.Play();
                }

                Game.PlayerStats.DealDamage(10f * deltaTime, false);
                Game.PlayerStats.onFire = true;
            }
        }
    }
}
