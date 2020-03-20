using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ASCII_FPS : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public ASCII_FPS()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        SpriteFont font;
        Random random;
        Console console;
        Rasterizer rasterizer;
        Scene scene;
        Camera camera;


        public static string debug = "";
        public static AsciiTexture texture1, texture2;

        protected override void Initialize()
        {
            random = new Random();
            console = new Console(160, 90);
            rasterizer = new Rasterizer(console);
            camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            
            texture1 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks01"));
            texture2 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks02"));

            scene = Scenes.Scenes.Level1();
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }
        
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            rasterizer.Raster(scene, camera);

            Vector3 shift = Vector3.Zero;
            if (keyboard.IsKeyDown(Keys.Up))
                shift += 0.02f * gameTime.ElapsedGameTime.Milliseconds * camera.Forward;
            if (keyboard.IsKeyDown(Keys.Down))
                shift -= 0.02f * gameTime.ElapsedGameTime.Milliseconds * camera.Forward;

            float rotation = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
                rotation -= 0.0005f * (float)Math.PI * gameTime.ElapsedGameTime.Milliseconds;
            if (keyboard.IsKeyDown(Keys.Right))
                rotation += 0.0005f * (float)Math.PI * gameTime.ElapsedGameTime.Milliseconds;

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                shift *= 2.5f;
                rotation *= 2.5f;
            }

            if (scene.CheckMovement(camera.CameraPos, shift, 0.65f))
                camera.CameraPos += shift;
            camera.Rotation += rotation;

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            debug = gameTime.ElapsedGameTime.Milliseconds + " ms";

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    int color = console.Color[i, j];
                    int r = ((color & 0b111) * 0b1001001) >> 1;
                    int g = (((color >> 3) & 0b111) * 0b1001001) >> 1;
                    int b = ((color >> 6) & 0b11) * 0b1010101;
                    spriteBatch.DrawString(font, console.Data[i, j].ToString(), new Vector2(i, j) * Console.FONT_SIZE, new Color(r, g, b));
                }
            }
            spriteBatch.DrawString(font, debug, Vector2.Zero, Color.White);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
