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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public ASCII_FPS()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }



        public static DisplayMode[] resolutions;

        SpriteFont font;
        Random random;
        Console console;
        Rasterizer rasterizer;
        HUD hud;
        Scene scene;

        public static bool saveExists = false;

        
        public static int triangleCount = 0;
        public static int triangleCountClipped = 0;
        public static int zonesRendered = 0;
        public static int frames = 0;
        public static float timeElapsed = 0f;
        public static float fps = 0f;
        public static string additionalDebug = "";
        public static bool enableDebug = false;

        public static PlayerStats playerStats;

        private enum GameState { MainMenu, Tutorial, Options, Game }
        private GameState gameState = GameState.MainMenu;

        public static AsciiTexture texture1, texture2, barrelRedTexture, barrelGreenTexture, barrelBlueTexture, monsterTexture, projectileTexture, exitTexture;
        public static OBJFile barrelModel, exitModel;
        public static SoundEffect tsch, oof, ouch, theme;

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

            random = new Random();
            int charsX = (int)Math.Floor((double)graphics.PreferredBackBufferWidth / Console.FONT_SIZE);
            int charsY = (int)Math.Floor((double)graphics.PreferredBackBufferHeight / Console.FONT_SIZE);
            console = new Console(charsX, charsY);
            rasterizer = new Rasterizer(console);
            hud = new HUD(console);
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
            monsterTexture = new AsciiTexture(Content.Load<Texture2D>("textures/monster"));
            projectileTexture = new AsciiTexture(Content.Load<Texture2D>("textures/projectile"));
            exitTexture = new AsciiTexture(Content.Load<Texture2D>("textures/exit"));

            barrelModel = Content.Load<OBJFile>("models/barrel");
            exitModel = Content.Load<OBJFile>("models/exit");

            tsch = Content.Load<SoundEffect>("audio/tsch");
            oof = Content.Load<SoundEffect>("audio/oof");
            ouch = Content.Load<SoundEffect>("audio/ouch");
            theme = Content.Load<SoundEffect>("audio/theme");
            theme.Play();

            ResetGame();
            playerStats.dead = true;
            saveExists = File.Exists("./scene.sav");
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
            hud = new HUD(console);
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
            playerStats = new PlayerStats
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

            scene = SceneGenerator.Generate(10f, 5f, 4);
            scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
            HUD.visited = new bool[SceneGenerator.size, SceneGenerator.size];
            HUD.visited[SceneGenerator.size / 2, SceneGenerator.size / 2] = true;
            HUD.scene = scene;
        }

        private void LoadGame()
        {
            scene = GameSave.LoadGameScene();
            GameSave.LoadGameStats();
            HUD.scene = scene;
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
                    gameState = GameState.MainMenu;
                    if (playerStats.dead)
                    {
                        File.Delete("./player.sav");
                        File.Delete("./scene.sav");
                        saveExists = false;
                    }
                    else
                    {
                        GameSave.SaveGame(scene);
                        saveExists = true;
                    }
                }

                if (!playerStats.dead)
                {
                    Vector3 shift = Vector3.Zero;
                    if (keyboard.IsKeyDown(Keybinds.forward))
                        shift += 20f * deltaTime * scene.Camera.Forward;
                    if (keyboard.IsKeyDown(Keybinds.backwards))
                        shift -= 20f * deltaTime * scene.Camera.Forward;
                    if (keyboard.IsKeyDown(Keybinds.strafeRight))
                        shift += 10f * deltaTime * scene.Camera.Right;
                    if (keyboard.IsKeyDown(Keybinds.strafeLeft))
                        shift -= 10f * deltaTime * scene.Camera.Right;

                    float rotation = 0f;
                    if (keyboard.IsKeyDown(Keybinds.turnLeft))
                        rotation -= 0.5f * (float)Math.PI * deltaTime;
                    if (keyboard.IsKeyDown(Keybinds.turnRight))
                        rotation += 0.5f * (float)Math.PI * deltaTime;

                    if (keyboard.IsKeyDown(Keybinds.sprint))
                    {
                        shift *= 2.5f;
                        rotation *= 2.5f;
                    }

                    Vector3 realShift = scene.SmoothMovement(scene.Camera.CameraPos, shift, PlayerStats.thickness);
                    scene.Camera.CameraPos += realShift;

                    scene.Camera.Rotation += rotation;

                    if (playerStats.shootTime > 0f)
                    {
                        playerStats.shootTime -= deltaTime;
                    }

                    if (keyboard.IsKeyDown(Keybinds.fire))
                    {
                        if (playerStats.shootTime <= 0f)
                        {
                            playerStats.shootTime = 1f / (3f + playerStats.skillShootingSpeed * 0.5f);
                            tsch.Play();

                            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(scene.Camera.CameraPos + Vector3.Down, 0.4f, projectileTexture);
                            scene.AddGameObject(new Projectile(projectileMesh, scene.Camera.Forward, 75f, 2f));
                        }
                    }

                    if (keyboard.IsKeyDown(Keybinds.action) && keyboardPrev.IsKeyUp(Keybinds.action))
                    {
                        if (Vector3.Distance(scene.Camera.CameraPos, new Vector3(playerStats.exitPosition.X, 0f, playerStats.exitPosition.Y)) < 7f
                            && 2 * playerStats.monsters >= playerStats.totalMonsters)
                        {
                            theme.Play();
                            playerStats.floor++;

                            float monsterHealth = 8f + playerStats.floor * 2f;
                            float monsterDamage = 4f + playerStats.floor;
                            int maxMonsters = 4 + (int)Math.Floor(playerStats.floor / 3.0);
                            scene = SceneGenerator.Generate(monsterHealth, monsterDamage, maxMonsters);
                            scene.Camera = new Camera(0.5f, 1000f, (float)Math.PI / 2.5f, 16f / 9f);
                            HUD.scene = scene;
                            HUD.visited = new bool[SceneGenerator.size, SceneGenerator.size];
                            HUD.visited[SceneGenerator.size / 2, SceneGenerator.size / 2] = true;
                            GameSave.SaveGame(scene);
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

                    if (playerStats.skillPoints > 0 && keyboard.IsKeyDown(Keybinds.skills) && !keyboardPrev.IsKeyDown(Keybinds.skills))
                    {
                        HUD.skillPointMenu = !HUD.skillPointMenu;
                    }
                    if (playerStats.skillPoints == 0)
                    {
                        HUD.skillPointMenu = false;
                    }
                    if (HUD.skillPointMenu)
                    {
                        if (keyboard.IsKeyDown(Keys.D1) && !keyboardPrev.IsKeyDown(Keys.D1))
                        {
                            playerStats.skillPoints--;
                            playerStats.skillMaxHealth++;
                            playerStats.maxHealth += 20f;
                            playerStats.AddHealth(20f);
                        }
                        else if (keyboard.IsKeyDown(Keys.D2) && !keyboardPrev.IsKeyDown(Keys.D2))
                        {
                            playerStats.skillPoints--;
                            playerStats.skillMaxArmor++;
                            playerStats.maxArmor += 20f;
                            playerStats.AddArmor(20f);
                        }
                        else if (keyboard.IsKeyDown(Keys.D3) && !keyboardPrev.IsKeyDown(Keys.D3) && playerStats.skillArmorProtection < 35)
                        {
                            playerStats.skillPoints--;
                            playerStats.skillArmorProtection++;
                            playerStats.armorProtection += 0.02f;
                        }
                        else if (keyboard.IsKeyDown(Keys.D4) && !keyboardPrev.IsKeyDown(Keys.D4))
                        {
                            playerStats.skillPoints--;
                            playerStats.skillShootingSpeed++;
                        }
                    }
                }

                scene.UpdateGameObjects(deltaTime);

                int playerRoomX = (int)(scene.Camera.CameraPos.X / SceneGenerator.tileSize + SceneGenerator.size / 2f);
                int playerRoomY = (int)(scene.Camera.CameraPos.Z / SceneGenerator.tileSize + SceneGenerator.size / 2f);
                HUD.visited[playerRoomX, playerRoomY] = true;
            }
            else if (gameState == GameState.MainMenu)
            {
                if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
                {
                    hud.option = (hud.option + 1) % 4;
                    if (!saveExists && hud.option == 1)
                        hud.option++;
                }
                if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
                {
                    hud.option = (hud.option + 3) % 4;
                    if (!saveExists && hud.option == 1)
                        hud.option--;
                }
                if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                {
                    if (hud.option == 0)
                        gameState = GameState.Tutorial;
                    else if (hud.option == 1)
                    {
                        LoadGame();
                        gameState = GameState.Game;
                    }
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
                        gameState = GameState.MainMenu;
                        hud.option = 0;
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
            if (playerStats.dead && gameState == GameState.Game)
            {
                console.Effect = Console.ColorEffect.Grayscale;
            }
            else if (playerStats.hit && gameState == GameState.Game)
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


            // Rendering
            if (gameState == GameState.Game)
            {
                rasterizer.Raster(scene, scene.Camera);
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
                string debug = fps + " FPS\nTotal number of static triangles: " + scene.TotalTriangles +
                                     "\nNumber of rendered triangles: " + triangleCount +
                                     "\nNumber of triangles after clipping: " + triangleCountClipped +
                                     "\nNumber of zones rendered: " + zonesRendered +
                                     "\nPosition: " + scene.Camera.CameraPos +
                                     "\n" + additionalDebug +
                                     "\nHealth: " + (int)playerStats.health;

                spriteBatch.DrawString(font, debug, Vector2.Zero, Color.White);
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
