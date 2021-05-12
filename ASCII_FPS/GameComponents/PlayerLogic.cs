using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.GameComponents
{
    public class PlayerLogic
    {
        private readonly ASCII_FPS game;

        public PlayerLogic(ASCII_FPS game)
        {
            this.game = game;
        }


        public bool Update(float deltaTime, KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            PlayerStats playerStats = game.PlayerStats;
            if (playerStats.dead)
                return false;

            Scene scene = game.Scene;

            float sprintMultiplier = keyboard.IsKeyDown(Keybinds.sprint) ? 2.5f : 1f;

            Vector3 shift = Vector3.Zero;
            if (keyboard.IsKeyDown(Keybinds.forward))
                shift += 20f * deltaTime * scene.Camera.Forward * sprintMultiplier;
            if (keyboard.IsKeyDown(Keybinds.backwards))
                shift -= 20f * deltaTime * scene.Camera.Forward;
            if (keyboard.IsKeyDown(Keybinds.strafeRight))
                shift += 15f * deltaTime * scene.Camera.Right;
            if (keyboard.IsKeyDown(Keybinds.strafeLeft))
                shift -= 15f * deltaTime * scene.Camera.Right;

            float rotation = 0f;
            if (keyboard.IsKeyDown(Keybinds.turnLeft))
                rotation -= 0.5f * (float)Math.PI * deltaTime * sprintMultiplier;
            if (keyboard.IsKeyDown(Keybinds.turnRight))
                rotation += 0.5f * (float)Math.PI * deltaTime * sprintMultiplier;

            Vector3 realShift = scene.SmoothMovement(scene.Camera.CameraPos, shift, PlayerStats.thickness);
            scene.Camera.CameraPos += realShift;
            scene.Camera.Rotation += rotation;
            
            int playerRoomX = (int)(scene.Camera.CameraPos.X / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            int playerRoomY = (int)(scene.Camera.CameraPos.Z / SceneGenerator.tileSize + SceneGenerator.size / 2f);
            scene.Visited[playerRoomX, playerRoomY] = true;

            if (playerStats.tempHealth > 0f)
            {
                playerStats.tempHealth -= deltaTime * 0.2f;
                if (playerStats.tempHealth < 0f)
                {
                    playerStats.tempHealth = 0f;
                }
            }

            if (playerStats.shootTime > 0f)
            {
                playerStats.shootTime -= deltaTime;
            }
            
            if (playerStats.hit)
            {
                playerStats.hitTime -= deltaTime;
                if (playerStats.hitTime < 0f)
                {
                    playerStats.hitTime = 0f;
                    playerStats.hit = false;
                }
            }


            if (keyboard.IsKeyDown(Keybinds.fire))
            {
                if (playerStats.shootTime <= 0f)
                {
                    playerStats.shootTime = 1f / (3f + playerStats.skillShootingSpeed * 0.5f);
                    ASCII_FPS.tsch.Play();

                    MeshObject projectileMesh = PrimitiveMeshes.Octahedron(scene.Camera.CameraPos + Vector3.Down, 0.4f, ASCII_FPS.projectileTexture);
                    scene.AddGameObject(new Projectile(projectileMesh, scene.Camera.Forward, 75f, 2f));
                }
            }

            if (keyboard.IsKeyDown(Keybinds.action) && keyboardPrev.IsKeyUp(Keybinds.action))
            {
                if (Vector3.Distance(scene.Camera.CameraPos, new Vector3(playerStats.exitPosition.X, 0f, playerStats.exitPosition.Y)) < 7f
                    && 2 * playerStats.monsters >= playerStats.totalMonsters)
                {
                    if (playerStats.monsters < playerStats.totalMonsters)
                    {
                        playerStats.fullClear = false;
                    }
                    foreach (Collectible.Type? collectible in scene.Collectibles)
                    {
                        if (collectible != null)
                        {
                            playerStats.fullClear = false;
                        }
                    }

                    playerStats.floor++;
                    Achievements.UnlockLeveled("Level", playerStats.floor, game.HUD);

                    if (playerStats.fullClear)
                    {
                        Achievements.UnlockLeveled("100%", playerStats.floor - 1, game.HUD);
                    }

                    return true;
                }
                else
                {
                    foreach (GameObject gameObject in scene.gameObjects)
                    {
                        if (gameObject is Collectible collectible && Vector3.Distance(scene.Camera.CameraPos, collectible.Position) < 7f)
                        {
                            collectible.PickUp(playerStats);
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
                    Achievements.UnlockLeveled("HP", playerStats.skillMaxHealth, game.HUD);
                }
                else if (keyboard.IsKeyDown(Keys.D2) && !keyboardPrev.IsKeyDown(Keys.D2))
                {
                    playerStats.skillPoints--;
                    playerStats.skillMaxArmor++;
                    playerStats.maxArmor += 20f;
                    playerStats.AddArmor(20f);
                    Achievements.UnlockLeveled("Armor", playerStats.skillMaxArmor, game.HUD);
                }
                else if (keyboard.IsKeyDown(Keys.D3) && !keyboardPrev.IsKeyDown(Keys.D3) && playerStats.skillArmorProtection < 35)
                {
                    playerStats.skillPoints--;
                    playerStats.skillArmorProtection++;
                    playerStats.armorProtection += 0.02f;
                    Achievements.UnlockLeveled("AP", playerStats.skillArmorProtection, game.HUD);
                }
                else if (keyboard.IsKeyDown(Keys.D4) && !keyboardPrev.IsKeyDown(Keys.D4))
                {
                    playerStats.skillPoints--;
                    playerStats.skillShootingSpeed++;
                    Achievements.UnlockLeveled("Speed", playerStats.skillShootingSpeed, game.HUD);
                }
            }

            return false;
        }
    }
}
