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
        public static Color[,] texture;

        protected override void Initialize()
        {
            random = new Random();
            console = new Console(160, 90);
            rasterizer = new Rasterizer(console);
            scene = new Scene();
            scene.AddWall(-5, -20, -5, 20, 4, Vector3.One);
            scene.AddWall(5, -20, 5, 20, 4, Vector3.One);
            scene.AddWall(-20, 30, 20, 30, 4, Vector3.One);
            scene.AddWall(-15, -30, -15, 30, 4, Vector3.One);
            scene.AddWall(15, -30, 15, 30, 4, Vector3.One);
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

            Texture2D t = Content.Load<Texture2D>("textures/bricks01");
            Color[] color1d = new Color[t.Width * t.Height];
            t.GetData(color1d);
            texture = new Color[t.Width, t.Height];
            for (int i = 0; i < t.Width; i++)
            {
                for (int j = 0; j < t.Height; j++)
                {
                    texture[i, j] = color1d[i + j * t.Width];
                }
            }
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
