using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OBJContentPipelineExtension;
using System;

namespace ASCII_FPS
{
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
        HUD hud;
        Scene scene;

        
        public static int triangleCount = 0;
        public static int triangleCountClipped = 0;
        public static int zonesRendered = 0;
        public static int frames = 0;
        public static float timeElapsed = 0f;
        public static float fps = 0f;
        public static string additionalDebug = "";

        public static PlayerStats playerStats;

        public static AsciiTexture texture1, texture2, barrelTexture, monsterTexture, projectileTexture, exitTexture;
        public static OBJFile barrelModel, exitModel;

        protected override void Initialize()
        {
            random = new Random();
            console = new Console(160, 90);
            rasterizer = new Rasterizer(console);
            hud = new HUD(console);

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            base.Initialize();
            
            playerStats.health = 100f;
            playerStats.maxHealth = 100f;
            playerStats.maxArmor = 100f;
            playerStats.armor = 100f;
            playerStats.armorProtection = 0.3f;
            playerStats.dead = false;
            playerStats.hit = false;
            playerStats.floor = 1;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            
            texture1 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks01"));
            texture2 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks02"));
            barrelTexture = new AsciiTexture(Content.Load<Texture2D>("textures/barrel"));
            monsterTexture = new AsciiTexture(Content.Load<Texture2D>("textures/monster"));
            projectileTexture = new AsciiTexture(Content.Load<Texture2D>("textures/projectile"));
            exitTexture = new AsciiTexture(Content.Load<Texture2D>("textures/exit"));

            barrelModel = Content.Load<OBJFile>("models/barrel");
            exitModel = Content.Load<OBJFile>("models/exit");

            scene = Scenes.Scenes.Generate(10f, 5f, 4);
            scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }


        KeyboardState keyboardPrev;
        protected override void Update(GameTime gameTime)
        {
            additionalDebug = "";
            KeyboardState keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            Vector3 shift = Vector3.Zero;
            if (keyboard.IsKeyDown(Keys.Up))
                shift += 20f * deltaTime * scene.Camera.Forward;
            if (keyboard.IsKeyDown(Keys.Down))
                shift -= 20f * deltaTime * scene.Camera.Forward;
            if (keyboard.IsKeyDown(Keys.X))
                shift += 10f * deltaTime * scene.Camera.Right;
            if (keyboard.IsKeyDown(Keys.Z))
                shift -= 10f * deltaTime * scene.Camera.Right;

            float rotation = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
                rotation -= 0.5f * (float)Math.PI * deltaTime;
            if (keyboard.IsKeyDown(Keys.Right))
                rotation += 0.5f * (float)Math.PI * deltaTime;

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                shift *= 2.5f;
                rotation *= 2.5f;
            }

            Vector3 realShift = scene.SmoothMovement(scene.Camera.CameraPos, shift, PlayerStats.thickness);
            scene.Camera.CameraPos += realShift;

            scene.Camera.Rotation += rotation;

            if (keyboard.IsKeyDown(Keys.Space) && keyboardPrev.IsKeyUp(Keys.Space))
            {
                MeshObject projectileMesh = PrimitiveMeshes.Octahedron(scene.Camera.CameraPos + Vector3.Down, 0.4f, projectileTexture);
                scene.AddGameObject(new Projectile(projectileMesh, scene.Camera.Forward, 75f, 2f));
            }

            if (keyboard.IsKeyDown(Keys.Enter) && keyboardPrev.IsKeyUp(Keys.Enter))
            {
                if (Vector3.Distance(scene.Camera.CameraPos, new Vector3(playerStats.exitPosition.X, 0f, playerStats.exitPosition.Y)) < 7f 
                    && 2 * playerStats.monsters >= playerStats.totalMonsters)
                {
                    playerStats.floor++;

                    float monsterHealth = 8f + playerStats.floor * 2f;
                    float monsterDamage = 4f + playerStats.floor;
                    int maxMonsters = 4 + (int)Math.Floor(playerStats.floor / 3.0);
                    scene = Scenes.Scenes.Generate(monsterHealth, monsterDamage, maxMonsters);
                    scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
                }
                else
                {
                    foreach (GameObject gameObject in scene.gameObjects)
                    {
                        if (gameObject is Collectible collectible && Vector3.Distance(scene.Camera.CameraPos, collectible.Position) < 7f)
                        {
                            collectible.PickUp();
                        }
                    }
                }
            }

            scene.UpdateGameObjects(deltaTime);

            keyboardPrev = keyboard;


            // Update effects
            if (playerStats.dead)
            {
                console.Effect = Console.ColorEffect.Grayscale;
            }
            else if (playerStats.hit)
            {
                console.Effect = Console.ColorEffect.Red;
                playerStats.hitTime -= deltaTime;
                if (playerStats.hitTime < 0f)
                {
                    playerStats.hitTime = 0f;
                    playerStats.hit = false;
                }
            }
            else
            {
                console.Effect = Console.ColorEffect.None;
            }

            rasterizer.Raster(scene, scene.Camera);
            hud.Draw();

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
            string debug = fps + " FPS\nTotal number of static triangles: " + scene.TotalTriangles +
                                 "\nNumber of rendered triangles: " + triangleCount +
                                 "\nNumber of triangles after clipping: " + triangleCountClipped +
                                 "\nNumber of zones rendered: " + zonesRendered +
                                 "\nPosition: " + scene.Camera.CameraPos +
                                 "\n" + additionalDebug +
                                 "\nHealth: " + (int)playerStats.health;

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            for (int i = 0; i < console.Width; i++)
            {
                for (int j = 0; j < console.Height; j++)
                {
                    int color = console.GetColor(i, j);
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
