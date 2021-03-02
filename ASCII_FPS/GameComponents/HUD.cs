using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public class HUD
    {
        private readonly Console console;
        private readonly ASCII_FPS game;

        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorLightGray = Mathg.ColorTo8Bit(Color.LightGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());
        private readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());

        public HUD(ASCII_FPS game, Console console)
        {
            this.console = console;
            this.game = game;
            Scene = null;
        }

        public Scene Scene { private get; set; }

        public static bool skillPointMenu;


        private void LineHorizontal(int y, int left, int right, byte color, char data)
        {
            if (left < 0) left += console.Width;
            if (right < 0) right += console.Width;
            if (y < 0) y += console.Height;

            for (int i = left; i <= right; i++)
            {
                console.Data[i, y] = data;
                console.Color[i, y] = color;
            }
        }

        private void LineVertical(int x, int top, int bottom, byte color, char data)
        {
            if (top < 0) top += console.Height;
            if (bottom < 0) bottom += console.Height;
            if (x < 0) x += console.Width;

            for (int i = top; i <= bottom; i++)
            {
                console.Data[x, i] = data;
                console.Color[x, i] = color;
            }
        }

        private void Border(int left, int top, int right, int bottom, byte color, char data)
        {
            LineHorizontal(top, left, right, color, data);
            LineHorizontal(bottom, left, right, color, data);
            LineVertical(left, top, bottom, color, data);
            LineVertical(right, top, bottom, color, data);
        }

        private void Rectangle(int left, int top, int right, int bottom, byte color, char data)
        {
            if (left < 0) left += console.Width;
            if (right < 0) right += console.Width;
            if (top < 0) top += console.Height;
            if (bottom < 0) bottom += console.Height;

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    console.Data[x, y] = data;
                    console.Color[x, y] = color;
                }
            }
        }

        private void Text(int x, int y, string text, byte color)
        {
            if (x < 0) x += console.Width;
            if (y < 0) y += console.Height;

            int start = x - text.Length / 2;
            for (int xx = start; xx < start + text.Length; xx++)
            {
                if (xx >= 0 && xx < console.Width)
                {
                    console.Data[xx, y] = text[xx - start];
                    console.Color[xx, y] = color;
                }
            }
        }


        public void Draw()
        {
            const int barX = 16;
            const int barY = 10;

            // HP bar
            Rectangle(0, -barY - 8, barX + 1, -1, colorBlack, ' ');
            Border(0, -barY - 8, barX + 1, -1, colorGray, '@');
            Text(1 + barX / 2, -barY - 6, "Health", colorWhite);
            Text(1 + barX / 2, -barY - 4, (int)Math.Ceiling(game.PlayerStats.health) 
                + " / " + (int)Math.Ceiling(game.PlayerStats.maxHealth), colorWhite);
            LineHorizontal(-barY - 2, 0, barX + 1, colorGray, '@');
            int hpDots = (int)(barX * barY * game.PlayerStats.health / game.PlayerStats.maxHealth);
            if (hpDots >= barX) Rectangle(1, -1 - hpDots / barX, barX, -2, colorRed, '%');
            if (hpDots % barX > 0) Rectangle(1, -2 - hpDots / barX, 1 + hpDots % barX, -2 - hpDots / barX, colorRed, '%');

            // Armor bar
            Rectangle(-2 - barX, -barY - 8, -1, -1, colorBlack, ' ');
            Border(-2 - barX, -barY - 8, -1, -1, colorGray, '@');
            Text(-1 - barX / 2, -barY - 6, "Armor", colorWhite);
            Text(-1 - barX / 2, -barY - 4, (int)Math.Ceiling(game.PlayerStats.armor)
                + " / " + (int)Math.Ceiling(game.PlayerStats.maxArmor), colorWhite);
            LineHorizontal(-barY - 2, -2 - barX, -1, colorGray, '@');
            int armorDots = (int)(barX * barY * game.PlayerStats.armor / game.PlayerStats.maxArmor);
            if (armorDots >= barX) Rectangle(-barX - 1, -1 - armorDots / barX, -2, -2, colorForestGreen, '#');
            if (armorDots % barX > 0) Rectangle(-(1 + armorDots % barX), -2 - armorDots / barX, -2, -2 - armorDots / barX, colorForestGreen, '#');

            // Floor + killed monsters + skill points
            int offset = game.PlayerStats.skillPoints == 0 ? 0 : skillPointMenu ? 12 : 2;
            Rectangle(console.Width / 2 - 15, -7 - offset, console.Width / 2 + 14, -1, colorBlack, ' ');
            Border(console.Width / 2 - 15, -7 - offset, console.Width / 2 + 14, -1, colorGray, '@');
            Text(console.Width / 2, -5 - offset, "Floor " + game.PlayerStats.floor, colorWhite);
            Text(console.Width / 2, -3 - offset,
                 "Monsters: " + game.PlayerStats.monsters + " / " + game.PlayerStats.totalMonsters, colorWhite);
            if (game.PlayerStats.skillPoints > 0)
            {
                Text(console.Width / 2, -1 - offset, "(" + Keybinds.skills + ") Skill points left: " + game.PlayerStats.skillPoints, colorWhite);
                if (skillPointMenu)
                {
                    Rectangle(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorBlack, ' ');
                    Border(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorGray, '@');
                    Text(console.Width / 2, -9, "(1) Max health lvl. " + game.PlayerStats.skillMaxHealth, colorWhite);
                    Text(console.Width / 2, -7, "(2) Max armor lvl. " + game.PlayerStats.skillMaxArmor, colorWhite);
                    Text(console.Width / 2, -5, "(3) Armor protection lvl. " + game.PlayerStats.skillArmorProtection, colorWhite);
                    Text(console.Width / 2, -3, "(4) Shooting speed lvl. " + game.PlayerStats.skillShootingSpeed, colorWhite);
                }
            }
        
            // Minimap
            Rectangle(-13, 0, -1, 12, colorLightGray, ' ');
            Border(-13, 0, -1, 12, colorGray, '@');

            int playerRoomX = (int)(Scene.Camera.CameraPos.X / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            int playerRoomY = (int)(Scene.Camera.CameraPos.Z / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    int xx = console.Width - 11 + x;
                    int yy = 10 - y;

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        if (Scene.Visited[x / 2, y / 2])
                        {
                            if (playerRoomX == x / 2 && playerRoomY == y / 2)
                            {
                                console.Color[xx, yy] = colorLightBlue;
                                float rotation = Scene.Camera.Rotation * 4f / (float)Math.PI;
                                int direction = (int)Math.Floor(rotation) % 8;
                                if (direction < 0) direction += 8;
                                console.Data[xx, yy] = "^>>vv<<^"[direction];
                            }
                            else if (Scene.ExitRoom.X == x / 2 && Scene.ExitRoom.Y == y / 2)
                            {
                                console.Data[xx, yy] = 'E';
                            }
                            else if (Scene.Collectibles[x / 2, y / 2] != null)
                            {
                                console.Data[xx, yy] = 'x';
                                switch (Scene.Collectibles[x / 2, y / 2])
                                {
                                    case Collectible.Type.Health:
                                        console.Color[xx, yy] = colorRed;
                                        break;
                                    case Collectible.Type.Armor:
                                        console.Color[xx, yy] = colorForestGreen;
                                        break;
                                    case Collectible.Type.Skill:
                                        console.Color[xx, yy] = colorLightBlue;
                                        break;
                                }
                            }
                            else
                            {
                                console.Data[xx, yy] = 'o';
                            }
                        }
                    }
                    else if (x % 2 == 0 && Scene.CorridorLayout[x / 2, y / 2, 1]
                        && (Scene.Visited[x / 2, y / 2] || Scene.Visited[x / 2, y / 2 + 1]))
                    {
                        console.Data[xx, yy] = '|';
                    }
                    else if (y % 2 == 0 && Scene.CorridorLayout[x / 2, y / 2, 0]
                        && (Scene.Visited[x / 2, y / 2] || Scene.Visited[x / 2 + 1, y / 2]))
                    {
                        console.Data[xx, yy] = '-';
                    }
                }
            }

            // Game Over
            if (game.PlayerStats.dead)
            {
                Rectangle(console.Width / 2 - 16, console.Height / 2 - 9, console.Width / 2 + 15, console.Height / 2 + 8, colorBlack, ' ');
                Border(console.Width / 2 - 16, console.Height / 2 - 9, console.Width / 2 + 15, console.Height / 2 + 8, colorWhite, '@');
                Text(console.Width / 2, console.Height / 2 - 6, "GAME OVER", colorWhite);
                Text(console.Width / 2, console.Height / 2 - 2, "Floor reached: " + game.PlayerStats.floor, colorWhite);
                Text(console.Width / 2, console.Height / 2, "Monsters killed: " + game.PlayerStats.totalMonstersKilled, colorWhite);
                Text(console.Width / 2, console.Height / 2 + 4, "Press Esc to return", colorWhite);
                Text(console.Width / 2, console.Height / 2 + 5, "to the main menu", colorWhite);
            }
        }


        public int option = 0;
        public int submenu = 0;
        public int selected = 0;
        public void MainMenu()
        {
            Rectangle(0, 0, -1, -1, colorBlack, ' ');
            Text(console.Width / 2,  6, @"_______                                        ___                    ", colorWhite);
            Text(console.Width / 2,  7, @"|     |______________________________________  | |__ _________________", colorWhite);
            Text(console.Width / 2,  8, @"| ___ |   |   ||_||_||     |     |   ||     |  | | | | |     |   |   |", colorWhite);
            Text(console.Width / 2,  9, @"| |_| | __| __|______| ___ | ___ | __|| ___ |  | | | | | ___ | __| __|", colorWhite);
            Text(console.Width / 2, 10, @"|     |   | |  | || || | | | | | | |  | | | |  | | | | | | | |   |  | ", colorWhite);
            Text(console.Width / 2, 11, @"| ___ |__ | |__| || || |_| | |_| | |__| |_| |__| | |_| | |_| |__ | _|_", colorWhite);
            Text(console.Width / 2, 12, @"| | | |   |   || || ||     |     |   ||       || |     |     |   |   |", colorWhite);
            Text(console.Width / 2, 13, @"|_| |_|___|___||_||_|| ____|_____|___||_______||_|____ | ____|___|___|", colorWhite);
            Text(console.Width / 2, 14, @"                     | |                             | | |            ", colorWhite);
            Text(console.Width / 2, 15, @"                     | |                             | | |            ", colorWhite);
            Text(console.Width / 2, 16, @"                     |_|                             |_|_|            ", colorWhite);

            if (!game.SaveExists)
            {
                Text(console.Width / 2, 30, "New game", option == 1 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 32, "Options", option == 2 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 34, "Exit", option == 3 ? colorLightBlue : colorGray);
            }
            else
            {
                Text(console.Width / 2, 30, "Continue", option == 0 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 32, "New game", option == 1 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 34, "Options", option == 2 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 36, "Exit", option == 3 ? colorLightBlue : colorGray);
            }

            Text(4, -2, "v1.0.1", colorWhite);
            Text(-9, -2, "by wonrzrzeczny", colorWhite);
        }

        public void Tutorial()
        {
            Rectangle(0, 0, -1, -1, colorBlack, ' ');
            Text(console.Width / 2, 12, "Controls", colorWhite);

            Text(console.Width / 2, 16, "Walk forward / backwards - " + Keybinds.forward + " / " + Keybinds.backwards, colorWhite);
            Text(console.Width / 2, 18, "Turn left / right - " + Keybinds.turnLeft + " / " + Keybinds.turnRight, colorWhite);
            Text(console.Width / 2, 20, "Strafe left / right - " + Keybinds.strafeLeft + " / " + Keybinds.strafeRight, colorWhite);
            Text(console.Width / 2, 22, "Hold " + Keybinds.sprint + " - faster movement", colorWhite);
            Text(console.Width / 2, 24, "Hold " + Keybinds.fire + " - shoot", colorWhite);
            Text(console.Width / 2, 26, Keybinds.action + " - use barrel / ladder", colorWhite);
            Text(console.Width / 2, 28, Keybinds.skills + " - skill menu", colorWhite);
            Text(console.Width / 2, 30, "1/2/3/4 - upgrade skill", colorWhite);
            Text(console.Width / 2, 32, "Escape - pause game", colorWhite);

            Text(console.Width / 2, 36, "To progress you must kill at least half of the monsters on the floor", colorWhite);

            Text(console.Width / 2, 40, "Press enter to start the game", colorWhite);

        }
        
        public void Options()
        {
            Rectangle(0, 0, -1, -1, colorBlack, ' ');

            if (submenu == 0)
            {
                Text(console.Width / 2, 12, "Back to main menu", option == 0 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 16, "Keybinds", option == 1 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 18, "Fullscreen", option == 2 ? colorLightBlue : colorGray);
                Text(console.Width / 2, 22, "Resolution", colorWhite);

                int maxLines = (console.Height - 26) / 2;
                int startLine = option < 2 ? 0 : Math.Max(0, option - maxLines / 2 - 1);
                int endLine = Math.Min(startLine + maxLines, ASCII_FPS.resolutions.Length);

                for (int i = startLine; i < endLine; i++)
                {
                    int resX = ASCII_FPS.resolutions[i].Width;
                    int resY = ASCII_FPS.resolutions[i].Height;
                    string line = resX + " x " + resY;
                    if (resX == 1920 && resY == 1080) line += " (recommended)";
                    Text(console.Width / 2, 24 + 2 * (i - startLine), line, i + 3 == option ? colorLightBlue : colorGray);
                }
            }
            else
            {
                Text(console.Width / 2, 12, "Back", option == 0 ? colorLightBlue : colorGray);

                string[] descriptions = new string[]
                {
                    "Walk forward", "Walk backwards", "Turn left", "Turn right",
                    "Strafe left", "Strafe right", "Sprint", "Fire",
                    "Action", "Skill menu"
                };
                string[] currentBinds = new string[]
                {
                    Keybinds.forward.ToString(), Keybinds.backwards.ToString(), Keybinds.turnLeft.ToString(), Keybinds.turnRight.ToString(),
                    Keybinds.strafeLeft.ToString(), Keybinds.strafeRight.ToString(), Keybinds.sprint.ToString(), Keybinds.fire.ToString(),
                    Keybinds.action.ToString(), Keybinds.skills.ToString()
                };

                for (int i = 0; i < descriptions.Length; i++)
                {
                    string text = descriptions[i] + " - " + (selected == i + 1 ? "< Press key >" : currentBinds[i]);
                    byte color = option == i + 1 ? colorLightBlue : colorGray;
                    Text(console.Width / 2, 16 + 2 * i, text, color);
                }
            }
        }
    }
}
