﻿using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public class HUD
    {
        private readonly Console console;

        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorLightGray = Mathg.ColorTo8Bit(Color.LightGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());
        private readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());

        public HUD(Console console)
        {
            this.console = console;
        }

        public static Scene scene;
        public static bool[,,] corridorLayout;
        public static bool[,] visited;
        public static Point exitRoom;

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
            // HP bar
            Rectangle(0, -22, 21, -1, colorBlack, ' ');
            Border(0, -22, 21, -1, colorGray, '@');
            Text(11, -20, "Health", colorWhite);
            Text(11, -18, (int)Math.Ceiling(ASCII_FPS.playerStats.health) 
                + " / " + (int)Math.Ceiling(ASCII_FPS.playerStats.maxHealth), colorWhite);
            LineHorizontal(-16, 0, 21, colorGray, '@');
            int hpDots = (int)(20 * 14 * ASCII_FPS.playerStats.health / ASCII_FPS.playerStats.maxHealth);
            if (hpDots >= 20) Rectangle(1, -1 - hpDots / 20, 20, -2, colorRed, '%');
            if (hpDots % 20 > 0) Rectangle(1, -2 - hpDots / 20, 1 + hpDots % 20, -2 - hpDots / 20, colorRed, '%');

            // Armor bar
            Rectangle(-22, -22, -1, -1, colorBlack, ' ');
            Border(-22, -22, -1, -1, colorGray, '@');
            Text(-11, -20, "Armor", colorWhite);
            Text(-11, -18, (int)Math.Ceiling(ASCII_FPS.playerStats.armor)
                + " / " + (int)Math.Ceiling(ASCII_FPS.playerStats.maxArmor), colorWhite);
            LineHorizontal(-16, -22, -1, colorGray, '@');
            int armorDots = (int)(20 * 14 * ASCII_FPS.playerStats.armor / ASCII_FPS.playerStats.maxArmor);
            if (armorDots >= 20) Rectangle(-21, -1 - armorDots / 20, -2, -2, colorForestGreen, '#');
            if (armorDots % 20 > 0) Rectangle(-(1 + armorDots % 20), -2 - armorDots / 20, -2, -2 - armorDots / 20, colorForestGreen, '#');

            // Floor + killed monsters + skill points
            int offset = ASCII_FPS.playerStats.skillPoints == 0 ? 0 : skillPointMenu ? 12 : 2;
            Rectangle(console.Width / 2 - 15, -7 - offset, console.Width / 2 + 14, -1, colorBlack, ' ');
            Border(console.Width / 2 - 15, -7 - offset, console.Width / 2 + 14, -1, colorGray, '@');
            Text(console.Width / 2, -5 - offset, "Floor " + ASCII_FPS.playerStats.floor, colorWhite);
            Text(console.Width / 2, -3 - offset,
                 "Monsters: " + ASCII_FPS.playerStats.monsters + " / " + ASCII_FPS.playerStats.totalMonsters, colorWhite);
            if (ASCII_FPS.playerStats.skillPoints > 0)
            {
                Text(console.Width / 2, -1 - offset, "(P) Skill points left: " + ASCII_FPS.playerStats.skillPoints, colorWhite);
                if (skillPointMenu)
                {
                    Rectangle(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorBlack, ' ');
                    Border(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorGray, '@');
                    Text(console.Width / 2, -9, "(1) Max health lvl. " + ASCII_FPS.playerStats.skillMaxHealth, colorWhite);
                    Text(console.Width / 2, -7, "(2) Max armor lvl. " + ASCII_FPS.playerStats.skillMaxArmor, colorWhite);
                    Text(console.Width / 2, -5, "(3) Armor protection lvl. " + ASCII_FPS.playerStats.skillArmorProtection, colorWhite);
                    Text(console.Width / 2, -3, "(4) Shooting speed lvl. " + ASCII_FPS.playerStats.skillShootingSpeed, colorWhite);
                }
            }

            // Minimap
            Rectangle(-13, 0, -1, 12, colorLightGray, ' ');
            Border(-13, 0, -1, 12, colorGray, '@');

            int playerRoomX = (int)(scene.Camera.CameraPos.X / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            int playerRoomY = (int)(scene.Camera.CameraPos.Z / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    int xx = console.Width - 11 + x;
                    int yy = 10 - y;

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        if (visited[x / 2, y / 2])
                        {
                            if (playerRoomX == x / 2 && playerRoomY == y / 2)
                            {
                                console.Color[xx, yy] = colorLightBlue;
                                float rotation = scene.Camera.Rotation * 4f / (float)Math.PI;
                                int direction = (int)Math.Floor(rotation) % 8;
                                if (direction < 0) direction += 8;
                                console.Data[xx, yy] = "^>>vv<<^"[direction];
                            }
                            else if (exitRoom.X == x / 2 && exitRoom.Y == y / 2)
                            {
                                console.Data[xx, yy] = 'E';
                            }
                            else
                            {
                                console.Data[xx, yy] = 'o';
                            }
                        }
                    }
                    else if (x % 2 == 0 && corridorLayout[x / 2, y / 2, 1] && (visited[x / 2, y / 2] || visited[x / 2, y / 2 + 1]))
                        console.Data[xx, yy] = '|';
                    else if (y % 2 == 0 && corridorLayout[x / 2, y / 2, 0] && (visited[x / 2, y / 2] || visited[x / 2 + 1, y / 2]))
                        console.Data[xx, yy] = '-';
                }
            }
        }


        public int option = 0;
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

            Text(console.Width / 2, 30, "Play game", option == 0 ? colorLightBlue : colorGray);
            Text(console.Width / 2, 32, "Options", option == 1 ? colorLightBlue : colorGray);
            Text(console.Width / 2, 34, "Exit (your progress won't be saved)", option == 2 ? colorLightBlue : colorGray);
        }

        public void Tutorial()
        {
            Rectangle(0, 0, -1, -1, colorBlack, ' ');
            Text(console.Width / 2, 12, "Controls", colorWhite);

            Text(console.Width / 2, 16, "Walk forward / backwards - up arrow / down arrow", colorWhite);
            Text(console.Width / 2, 18, "Turn left / right - left arrow / right arrow", colorWhite);
            Text(console.Width / 2, 20, "Strafe left / right - Z / X", colorWhite);
            Text(console.Width / 2, 22, "Hold shift - faster movement", colorWhite);
            Text(console.Width / 2, 24, "Hold space - shoot", colorWhite);
            Text(console.Width / 2, 26, "Enter - use barrel / ladder", colorWhite);
            Text(console.Width / 2, 28, "P - skill menu", colorWhite);
            Text(console.Width / 2, 30, "1/2/3/4 - upgrade skill", colorWhite);
            Text(console.Width / 2, 32, "Escape - pause game", colorWhite);

            Text(console.Width / 2, 36, "To progress you must kill at least half of the monsters on the floor", colorWhite);

            Text(console.Width / 2, 40, "Press enter to start the game", colorWhite);

        }
        
        public void Options()
        {
            Rectangle(0, 0, -1, -1, colorBlack, ' ');
            Text(console.Width / 2, 12, "Back to main menu", option == 0 ? colorLightBlue : colorGray);
            Text(console.Width / 2, 16, "Fullscreen", option == 1 ? colorLightBlue : colorGray);
            Text(console.Width / 2, 20, "Resolution", colorWhite);

            int maxLines = (console.Height - 24) / 2;
            int startLine = option < 2 ? 0 : Math.Max(0, option - maxLines / 2 - 1);
            int endLine = Math.Min(startLine + maxLines, ASCII_FPS.resolutions.Length);

            for (int i = startLine; i < endLine; i++)
            {
                int resX = ASCII_FPS.resolutions[i].Width;
                int resY = ASCII_FPS.resolutions[i].Height;
                string line = resX + " x " + resY;
                if (resX == 1920 && resY == 1080) line += " (recommended)";
                Text(console.Width / 2, 22 + 2 * (i - startLine), line, i + 2 == option ? colorLightBlue : colorGray);
            }
        }
    }
}