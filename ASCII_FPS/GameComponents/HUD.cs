using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
        private readonly byte colorLightGreen = Mathg.ColorTo8Bit(Color.Lime.ToVector3());

        private readonly Queue<string> notifications = new Queue<string>();
        private float notificationTimer = 0f;


        public string Hint { get; set; } = "";

        public HUD(ASCII_FPS game, Console console)
        {
            this.console = console;
            this.game = game;
            Scene = null;
        }

        public Scene Scene { private get; set; }

        public static bool skillPointMenu;


        public void Update(float deltaTime)
        {
            if (notifications.Count > 0)
            {
                notificationTimer -= deltaTime;
                if (notificationTimer < 0f)
                {
                    notifications.Dequeue();
                    if (notifications.Count > 0)
                    {
                        notificationTimer = 5f;
                    }
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
            Text(1 + barX / 2, -barY - 4, (int)Math.Ceiling(game.PlayerStats.health + game.PlayerStats.tempHealth) 
                + " / " + (int)Math.Ceiling(game.PlayerStats.maxHealth), colorWhite);
            LineHorizontal(-barY - 2, 0, barX + 1, colorGray, '@');
            int hpDots = (int)(barX * barY * game.PlayerStats.health / game.PlayerStats.maxHealth);
            int tempHpDots = (int)(barX * barY * (game.PlayerStats.health + game.PlayerStats.tempHealth) / game.PlayerStats.maxHealth) - hpDots;
            for (int d = 0; d < hpDots + tempHpDots; d++)
            {
                int cx = 1 + d % barX;
                int cy = console.Height - 2 - (d / barX);
                console.Data[cx, cy] = '%';
                console.Color[cx, cy] = d < hpDots ? colorRed : colorLightGreen;
            }

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

            // Floor + killed monsters + hint + skill points
            int offset = (skillPointMenu ? 10 : 0) + (Hint != "" ? 2 : 0);
            int width = Math.Max(30, Hint.Length + 4);
            Rectangle(console.Width / 2 - width / 2, -7 - offset, console.Width / 2 + width / 2, -1, colorBlack, ' ');
            Border(console.Width / 2 - width / 2, -7 - offset, console.Width / 2 + width / 2, -1, colorGray, '@');
            Text(console.Width / 2, -5 - offset, "Floor " + game.PlayerStats.floor, colorWhite);
            Text(console.Width / 2, -3 - offset,
                 "Monsters: " + game.PlayerStats.monsters + " / " + game.PlayerStats.totalMonsters, colorWhite);

            if (skillPointMenu)
            {
                Rectangle(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorBlack, ' ');
                Border(console.Width / 2 - 30, -11, console.Width / 2 + 30, -1, colorGray, '@');
                Text(console.Width / 2, -9, "(1) Max health lvl. " + game.PlayerStats.skillMaxHealth, colorWhite);
                Text(console.Width / 2, -7, "(2) Max armor lvl. " + game.PlayerStats.skillMaxArmor, colorWhite);
                Text(console.Width / 2, -5, "(3) Armor protection lvl. " + game.PlayerStats.skillArmorProtection, colorWhite);
                Text(console.Width / 2, -3, "(4) Shooting speed lvl. " + game.PlayerStats.skillShootingSpeed, colorWhite);
            }

            if (Hint != "")
            {
                Text(console.Width / 2, -1 - offset, Hint, colorWhite);
            }

            // Notifications
            if (notifications.Count > 0)
            {
                Rectangle(console.Width / 2 - 30, 3, console.Width / 2 + 30, 7, colorGray, '@');
                Rectangle(console.Width / 2 - 29, 4, console.Width / 2 + 29, 6, colorBlack, ' ');
                Text(console.Width / 2, 5, notifications.Peek(), colorWhite);
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

        public void AddNotification(string text)
        {
            if (notifications.Count == 0)
            {
                notificationTimer = 5f;
            }
            notifications.Enqueue(text);
        }


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
    }
}
