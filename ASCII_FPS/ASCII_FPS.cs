using ASCII_FPS.GameComponents;
using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using OBJContentPipelineExtension;
using System;
using System.Linq;
using System.IO;

namespace ASCII_FPS
{
    public class ASCII_FPS : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public static DisplayMode[] resolutions;

        private Console console;
        private Rasterizer rasterizer;
        private HUD hud;

        public bool SaveExists { get; private set; } = false;

        public Scene Scene { get; private set; }
        public PlayerStats PlayerStats { get; private set; }
        private PlayerLogic playerLogic;

        private enum GameState { MainMenu, Tutorial, Options, Game }
        private GameState gameState = GameState.MainMenu;

        // Content
        public static SpriteFont font;
        public static AsciiTexture texture1, texture2, barrelRedTexture, barrelGreenTexture, barrelBlueTexture, projectileTexture, projectile2Texture, exitTexture;
        public static AsciiTexture monsterTexture, shotgunDudeTexture, spinnyBoiTexture, spooperTexture;
        public static OBJFile barrelModel, exitModel, spooperModel;
        public static SoundEffect tsch, oof, ouch, theme;

        // DEBUG
        public static bool enableDebug = false;
        public static int triangleCount = 0;
        public static int triangleCountClipped = 0;
        public static int zonesRendered = 0;
        public static int frames = 0;
        public static float timeElapsed = 0f;
        public static float fps = 0f;
        public static string additionalDebug = "";


        public ASCII_FPS()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            Window.Title = "Asciipocalypse";

            resolutions = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                .Where((DisplayMode dm) => dm.Width >= 1000 && dm.Height >= 400).ToArray();

            if (!GameSave.LoadOptions(graphics))
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                graphics.ApplyChanges();
            }

            int charsX = (int)Math.Floor((double)graphics.PreferredBackBufferWidth / Console.FONT_SIZE);
            int charsY = (int)Math.Floor((double)graphics.PreferredBackBufferHeight / Console.FONT_SIZE);
            console = new Console(charsX, charsY);
            rasterizer = new Rasterizer(console);
            hud = new HUD(this, console);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            
            texture1 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks01"));
            texture2 = new AsciiTexture(Content.Load<Texture2D>("textures/bricks02"));
            barrelRedTexture = new AsciiTexture(Content.Load<Texture2D>("textures/barrel_red"));
            barrelGreenTexture = new AsciiTexture(Content.Load<Texture2D>("textures/barrel_green"));
            barrelBlueTexture = new AsciiTexture(Content.Load<Texture2D>("textures/barrel_blue"));
            projectileTexture = new AsciiTexture(Content.Load<Texture2D>("textures/projectile"));
            projectile2Texture = new AsciiTexture(Content.Load<Texture2D>("textures/projectile2"));
            exitTexture = new AsciiTexture(Content.Load<Texture2D>("textures/exit"));

            monsterTexture = new AsciiTexture(Content.Load<Texture2D>("textures/monster"));
            shotgunDudeTexture = new AsciiTexture(Content.Load<Texture2D>("textures/shotgun_dude"));
            spinnyBoiTexture = new AsciiTexture(Content.Load<Texture2D>("textures/spinny_boi"));
            spooperTexture = new AsciiTexture(Content.Load<Texture2D>("textures/spooper"));

            barrelModel = Content.Load<OBJFile>("models/barrel");
            exitModel = Content.Load<OBJFile>("models/exit");
            spooperModel = Content.Load<OBJFile>("models/spooper");

            tsch = Content.Load<SoundEffect>("audio/tsch");
            oof = Content.Load<SoundEffect>("audio/oof");
            ouch = Content.Load<SoundEffect>("audio/ouch");
            theme = Content.Load<SoundEffect>("audio/theme");
            theme.Play();

            SaveExists = File.Exists("./scene.sav");
            hud.option = SaveExists ? 0 : 1;
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }


        private void ChangeResolution(int resX, int resY)
        {
            int charsX = (int)Math.Floor((double)resX / Console.FONT_SIZE);
            int charsY = (int)Math.Floor((double)resY / Console.FONT_SIZE);
            console = new Console(charsX, charsY);
            rasterizer = new Rasterizer(console);
            int opt = hud.option;
            hud = new HUD(this, console);
            hud.option = opt;
            graphics.PreferredBackBufferWidth = resX;
            graphics.PreferredBackBufferHeight = resY;
            graphics.ApplyChanges();
        }

        private void ChangeFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
        }

        private void ResetGame()
        {
            PlayerStats = new PlayerStats
            {
                health = 100f,
                maxHealth = 100f,
                maxArmor = 100f,
                armor = 100f,
                armorProtection = 0.3f,
                dead = false,
                hit = false,
                floor = 1,
                skillPoints = 0,
                skillMaxHealth = 0,
                skillMaxArmor = 0,
                skillArmorProtection = 0,
                skillShootingSpeed = 0,
                totalMonstersKilled = 0
            };

            Scene = new SceneGenerator(this, 1).Generate();
            Scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
            Scene.Visited[SceneGenerator.size / 2, SceneGenerator.size / 2] = true;
            hud.Scene = Scene;
            playerLogic = new PlayerLogic(this);
        }

        private void LoadGame()
        {
            Scene = GameSave.LoadGameScene(this);
            PlayerStats = GameSave.LoadGameStats();
            hud.Scene = Scene;
            playerLogic = new PlayerLogic(this);
        }


        KeyboardState keyboardPrev;
        protected override void Update(GameTime gameTime)
        {
            additionalDebug = "";
            KeyboardState keyboard = Keyboard.GetState();

            float deltaTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            if (gameState == GameState.Game)
            {
                if (keyboard.IsKeyDown(Keys.OemOpenBrackets) && keyboard.IsKeyDown(Keys.OemCloseBrackets) && keyboard.IsKeyDown(Keys.LeftShift))
                    enableDebug = !enableDebug;

                if (keyboard.IsKeyDown(Keys.Escape))
                {
                    if (PlayerStats.dead)
                    {
                        File.Delete("./player.sav");
                        File.Delete("./scene.sav");
                        SaveExists = false;
                    }
                    else
                    {
                        GameSave.SaveGame(this);
                        SaveExists = true;
                    }
                    hud.option = SaveExists ? 0 : 1;
                    gameState = GameState.MainMenu;
                }

                if (playerLogic.Update(deltaTime, keyboard, keyboardPrev))
                {
                    theme.Play();
                    PlayerStats.floor++;

                    Scene = new SceneGenerator(this, PlayerStats.floor).Generate();
                    Scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
                    hud.Scene = Scene;
                    Scene.Visited[SceneGenerator.size / 2, SceneGenerator.size / 2] = true;
                    GameSave.SaveGame(this);
                    playerLogic = new PlayerLogic(this);
                }

                Scene.UpdateGameObjects(deltaTime);
            }
            else if (gameState == GameState.MainMenu)
            {
                if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
                {
                    hud.option = (hud.option + 1) % 4;
                    if (!SaveExists && hud.option == 0)
                        hud.option = 1;
                }
                if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
                {
                    hud.option = (hud.option + 3) % 4;
                    if (!SaveExists && hud.option == 0)
                        hud.option = 3;
                }
                if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                {
                    if (hud.option == 0)
                    {
                        LoadGame();
                        gameState = GameState.Game;
                    }
                    else if (hud.option == 1)
                        gameState = GameState.Tutorial;
                    else if (hud.option == 2)
                        gameState = GameState.Options;
                    else if (hud.option == 3)
                        Exit();

                    hud.option = 0;
                    hud.submenu = 0;
                }
            }
            else if (gameState == GameState.Tutorial)
            {
                if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                {
                    ResetGame();
                    
                    gameState = GameState.Game;
                }
                else if (keyboard.IsKeyDown(Keys.Escape) && !keyboardPrev.IsKeyDown(Keys.Escape))
                {
                    gameState = GameState.MainMenu;
                }
            }
            else if (gameState == GameState.Options)
            {
                if (hud.submenu == 0)
                {
                    if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
                    {
                        hud.option = (hud.option + 1) % (resolutions.Length + 3);
                    }
                    if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
                    {
                        hud.option = (hud.option + resolutions.Length + 2) % (resolutions.Length + 3);
                    }
                    if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                    {
                        if (hud.option == 0)
                        {
                            GameSave.SaveOptions(graphics);
                            gameState = GameState.MainMenu;
                            hud.option = SaveExists ? 0 : 1;
                        }
                        else if (hud.option == 1)
                        {
                            hud.submenu = 1;
                            hud.option = 0;
                        }
                        else if (hud.option == 2)
                        {
                            ChangeFullScreen();
                        }
                        else
                        {
                            ChangeResolution(resolutions[hud.option - 3].Width, resolutions[hud.option - 3].Height);
                        }
                    }
                    if (keyboard.IsKeyDown(Keys.Escape) && !keyboardPrev.IsKeyDown(Keys.Escape))
                    {
                        GameSave.SaveOptions(graphics);
                        gameState = GameState.MainMenu;
                        hud.option = SaveExists ? 0 : 1;
                    }
                }
                else
                {
                    if (hud.selected == 0)
                    {
                        if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
                        {
                            hud.option = (hud.option + 1) % 11;
                        }
                        if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
                        {
                            hud.option = (hud.option + 10) % 11;
                        }
                        if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                        {
                            if (hud.option == 0)
                            {
                                hud.submenu = 0;
                            }
                            else
                            {
                                hud.selected = hud.option;
                            }
                        }
                        if (keyboard.IsKeyDown(Keys.Escape) && !keyboardPrev.IsKeyDown(Keys.Escape))
                        {
                            hud.option = 0;
                            hud.submenu = 0;
                        }
                    }
                    else
                    {
                        Keys[] keys = keyboard.GetPressedKeys();
                        if (keys.Length > 0 && !keyboardPrev.IsKeyDown(keys[0]))
                        {
                            if (hud.selected == 1) Keybinds.forward = keys[0];
                            else if (hud.selected == 2) Keybinds.backwards = keys[0];
                            else if (hud.selected == 3) Keybinds.turnLeft = keys[0];
                            else if (hud.selected == 4) Keybinds.turnRight = keys[0];
                            else if (hud.selected == 5) Keybinds.strafeLeft = keys[0];
                            else if (hud.selected == 6) Keybinds.strafeRight = keys[0];
                            else if (hud.selected == 7) Keybinds.sprint = keys[0];
                            else if (hud.selected == 8) Keybinds.fire = keys[0];
                            else if (hud.selected == 9) Keybinds.action = keys[0];
                            else if (hud.selected == 10) Keybinds.skills = keys[0];

                            hud.selected = 0;
                        }
                    }
                }
            }

            keyboardPrev = keyboard;
        
            
            // Update effects
            if (gameState == GameState.Game && PlayerStats.dead)
            {
                console.Effect = Console.ColorEffect.Grayscale;
            }
            else if (gameState == GameState.Game && PlayerStats.hit)
            {
                console.Effect = Console.ColorEffect.Red;
            }
            else
            {
                console.Effect = Console.ColorEffect.None;
            }


            // Rendering
            if (gameState == GameState.Game)
            {
                rasterizer.Raster(Scene);
                hud.Draw();
            }
            else if (gameState == GameState.MainMenu)
            {
                hud.MainMenu();
            }
            else if (gameState == GameState.Tutorial)
            {
                hud.Tutorial();
            }
            else if (gameState == GameState.Options)
            {
                hud.Options();
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
                    int color = console.GetColor(i, j);
                    int r = ((color & 0b111) * 0b1001001) >> 1;
                    int g = (((color >> 3) & 0b111) * 0b1001001) >> 1;
                    int b = ((color >> 6) & 0b11) * 0b1010101;
                    spriteBatch.DrawString(font, console.Data[i, j].ToString(), new Vector2(i, j) * Console.FONT_SIZE, new Color(r, g, b));
                }
            }

            if (enableDebug)
            {
                frames += 1;
                timeElapsed += gameTime.ElapsedGameTime.Milliseconds * 0.001f;
                if (timeElapsed > 1f)
                {
                    fps = frames / timeElapsed;
                    timeElapsed = 0f;
                    frames = 0;
                }
                string debug = fps + " FPS\nTotal number of static triangles: " + Scene.TotalTriangles +
                                     "\nNumber of rendered triangles: " + triangleCount +
                                     "\nNumber of triangles after clipping: " + triangleCountClipped +
                                     "\nNumber of zones rendered: " + zonesRendered +
                                     "\nPosition: " + Scene.Camera.CameraPos +
                                     "\n" + additionalDebug +
                                     "\nHealth: " + (int)PlayerStats.health;

                spriteBatch.DrawString(font, debug, Vector2.Zero, Color.White);
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
