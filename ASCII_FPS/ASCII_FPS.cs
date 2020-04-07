using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OBJContentPipelineExtension;
using System;
using System.Collections.Generic;

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

        
        public static int triangleCount = 0;
        public static int triangleCountClipped = 0;
        public static int zonesRendered = 0;
        public static int frames = 0;
        public static float timeElapsed = 0f;
        public static float fps = 0f;

        public static AsciiTexture texture1, texture2, barrelTexture, monsterTexture;
        public static OBJFile barrelModel;

        private List<GameObject> gameObjects;

        protected override void Initialize()
        {
            random = new Random();
            console = new Console(160, 90);
            rasterizer = new Rasterizer(console);
            camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
            gameObjects = new List<GameObject>();

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
            barrelTexture = new AsciiTexture(Content.Load<Texture2D>("textures/barrel"));
            monsterTexture = new AsciiTexture(Content.Load<Texture2D>("textures/monster"));
            barrelModel = Content.Load<OBJFile>("models/barrel");

            scene = Scenes.Scenes.Level1();
            MeshObject monster = PrimitiveMeshes.Tetrahedron(new Vector3(0f, -1f, 0f), 3f, monsterTexture);
            gameObjects.Add(new Monster(scene, monster));
            scene.AddDynamicMesh(monster);
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }


        KeyboardState keyboardPrev;
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float seconds = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            Vector3 shift = Vector3.Zero;
            if (keyboard.IsKeyDown(Keys.Up))
                shift += 20f * seconds * camera.Forward;
            if (keyboard.IsKeyDown(Keys.Down))
                shift -= 20f * seconds * camera.Forward;
            if (keyboard.IsKeyDown(Keys.X))
                shift += 10f * seconds * camera.Right;
            if (keyboard.IsKeyDown(Keys.Z))
                shift -= 10f * seconds * camera.Right;

            float rotation = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
                rotation -= 0.5f * (float)Math.PI * seconds;
            if (keyboard.IsKeyDown(Keys.Right))
                rotation += 0.5f * (float)Math.PI * seconds;

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                shift *= 2.5f;
                rotation *= 2.5f;
            }

            Vector3 realShift = scene.SmoothMovement(camera.CameraPos, shift, 0.65f);
            camera.CameraPos += realShift;

            camera.Rotation += rotation;

            if (keyboard.IsKeyDown(Keys.Space) && keyboardPrev.IsKeyUp(Keys.Space))
            {
                MeshObject projectileMesh = PrimitiveMeshes.Octahedron(camera.CameraPos + Vector3.Down, 0.4f, texture1);
                gameObjects.Add(new Projectile(scene, camera.Forward, 75f, projectileMesh));
                scene.AddDynamicMesh(projectileMesh);
            }

            List<GameObject> newGameObjects = new List<GameObject>();
            float deltaTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(deltaTime);
                if (!gameObject.Destroy)
                    newGameObjects.Add(gameObject);
            }
            gameObjects = newGameObjects;

            keyboardPrev = keyboard;



            rasterizer.Raster(scene, camera);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            frames += 1;
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            if (timeElapsed > 1f)
            {
                fps = frames / timeElapsed;
                timeElapsed = 0f;
                frames = 0;
            }
            string debug = fps + " FPS\nTotal number of triangles: " + scene.TotalTriangles +
                                 "\nNumber of rendered triangles: " + triangleCount +
                                 "\nNumber of triangles after clipping: " + triangleCountClipped +
                                 "\nNumber of zones rendered: " + zonesRendered +
                                 "\nPosition: " + camera.CameraPos;

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
