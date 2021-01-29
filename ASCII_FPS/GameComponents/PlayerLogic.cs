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
        private Scene scene;

        public PlayerLogic(Scene scene)
        {
            this.scene = scene;
        }


        public void Update(float deltaTime, KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            PlayerStats playerStats = ASCII_FPS.playerStats;
            if (playerStats.dead)
                return;
            
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
                    ASCII_FPS.theme.Play();
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
    }
}
