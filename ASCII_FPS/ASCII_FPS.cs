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

        protected override void Initialize()
        {
            random = new Random();
            console = new Console(160, 90);
            rasterizer = new Rasterizer(console);
            scene = new Scene();
            scene.AddTriangle(new Triangle(new Vector3(-5f, 4f, 20f), new Vector3(5f, 4f, 24f), new Vector3(1f, -5f, 18f), new Vector3(0.5f, 0.2f, 0.8f)));
            scene.AddTriangle(new Triangle(new Vector3(-10f, 6f, 22f), new Vector3(2f, -2f, 34f), new Vector3(-4f, -2f, 28f), new Vector3(0.9f, 0.3f, 0.2f)));
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
                shift += 0.02f * gameTime.ElapsedGameTime.Milliseconds * Vector3.Forward;
            if (keyboard.IsKeyDown(Keys.Down))
                shift += 0.02f * gameTime.ElapsedGameTime.Milliseconds * Vector3.Backward;

            float rotation = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
                rotation -= 0.0005f * (float)Math.PI * gameTime.ElapsedGameTime.Milliseconds;
            if (keyboard.IsKeyDown(Keys.Right))
                rotation += 0.0005f * (float)Math.PI * gameTime.ElapsedGameTime.Milliseconds;
            Matrix rotationMatrix = Mathg.RotationMatrix(-rotation);

            foreach (Triangle triangle in scene.triangles)
            {
                triangle.V0 += shift;
                triangle.V1 += shift;
                triangle.V2 += shift;
                triangle.V0 = Vector3.Transform(triangle.V0, rotationMatrix);
                triangle.V1 = Vector3.Transform(triangle.V1, rotationMatrix);
                triangle.V2 = Vector3.Transform(triangle.V2, rotationMatrix);
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    int color = console.Color[i, j];
                    int r = (color << 5) & 0xe0;
                    int g = (color << 2) & 0xe0;
                    int b = color & 0xc0;
                    spriteBatch.DrawString(font, console.Data[i, j].ToString(), new Vector2(i, j) * Console.FONT_SIZE, new Color(r, g, b));
                }
            }
            spriteBatch.DrawString(font, debug, Vector2.Zero, Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
