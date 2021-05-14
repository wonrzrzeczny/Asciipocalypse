using ASCII_FPS.GameComponents;
using ASCII_FPS.GameComponents.Enemies;
using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.Scenes.Generators
{
    public class SceneGeneratorDefault : SceneGenerator
    {
        private readonly float monsterHP;
        private readonly float monsterDamage;
        private readonly int monstersPerRoom;
        private readonly float[] monsterChances;

        protected override AsciiTexture WallTexture => Assets.wallTexture;
        protected override AsciiTexture FloorTexture => Assets.floorTexture;


        public SceneGeneratorDefault(ASCII_FPS game, int floor) : base(game)
        {
            monsterHP = 8f + floor * 2f;
            monsterDamage = 4f + floor;
            monstersPerRoom = 4 + (int)Math.Floor(floor / 3.0);

            float monsterChanceShotgun = floor < 2 ? 0f : 0.3f * (1 - 1 / (0.7f * (floor - 1) + 1f));
            float monsterChanceSpinny = floor < 3 ? 0f : 0.1f * (1 - 1 / (0.3f * (floor - 2) + 1f));
            float monsterChanceSpooper = floor < 4 ? 0f : 0.2f * (1 - 1 / (0.4f * (floor - 3) + 1f));
            monsterChances = new float[]
            {
                1f - monsterChanceShotgun - monsterChanceSpinny - monsterChanceSpooper,
                monsterChanceShotgun,
                monsterChanceSpinny,
                monsterChanceSpooper
            };
        }


        protected override void GenerateLayout(out bool[,] accessible, out bool[,,] corridorLayout, out float[,,] corridorWidths)
        {
            corridorLayout = SceneGenUtils.GenerateCorridorLayout(size, size, out accessible);
            corridorWidths = new float[size, size, 4];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int t = 0; t < 4; t++)
                    {
                        if (corridorLayout[x, y, t])
                        {
                            corridorWidths[x, y, t] = 10f;
                        }
                    }
                }
            }

            // Select two rooms connected with a corridor to join (5 tries, max 1 joint pair)
            for (int t = 0; t < 5; t++)
            {
                int x = rand.Next(size);
                int y = rand.Next(size);
                int d = rand.Next(4);
                if (corridorLayout[x, y, d] && (x != size / 2 || y != size / 2))
                {
                    Point[] shift = new Point[4] { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
                    if (x + shift[d].X != size / 2 || y + shift[d].Y != size / 2)
                    {
                        corridorWidths[x, y, d] = 80f;
                        corridorWidths[x + shift[d].X, y + shift[d].Y, d ^ 2] = 80f;
                        break;
                    }
                }
            }
        }

        protected override Collectible.Type?[,] DistributeCollectibles()
        {
            // Distribute collectibles (3 x skill point, 2 x armor refill, 1 x hp refill)
            // 10 attempts per each collectible
            Collectible.Type?[,] collectibles = new Collectible.Type?[size, size];
            for (int b = 0; b < 6; b++)
            {
                for (int t = 0; t < 10; t++)
                {
                    int x = rand.Next(size);
                    int y = rand.Next(size);
                    if ((x != size / 2 || y != size / 2) && (x != exitRoom.X || y != exitRoom.Y) && accessible[x, y] && collectibles[x, y] == null)
                    {
                        collectibles[x, y] = b < 3 ? Collectible.Type.Skill
                            : b < 5 ? Collectible.Type.Armor : Collectible.Type.Health;
                        break;
                    }
                }
            }
            return collectibles;
        }


        protected override PopulateRoomResults PopulateRoom(Scene scene, Zone zone, int x, int y, PopulateSchemeFlags flags)
        {
            PopulateRoomResults results = new PopulateRoomResults
            {
                GenerateFloor = true
            };

            float left = x * tileSize - size * tileSize / 2;
            float right = left + tileSize;
            float bottom = y * tileSize - size * tileSize / 2;
            float top = bottom + tileSize;
            Vector3 roomCenter = new Vector3((left + right) / 2, 0f, (top + bottom) / 2);

            if ((x != exitRoom.X || y != exitRoom.Y) && (x != size / 2 || y != size / 2))
            {
                int monsterCount = rand.Next(scene.Collectibles[x, y] == null ? 1 : 2, monstersPerRoom + 1);
                game.PlayerStats.totalMonsters += monsterCount;
                Vector2 shift = monsterCount == 1 ? Vector2.Zero : new Vector2(30f, 0f);
                float angleOffset = (float)(rand.NextDouble() * Math.PI * 2f);
                for (int i = 0; i < monsterCount; i++)
                {
                    Vector2 position = new Vector2(roomCenter.X, roomCenter.Z);
                    position += Vector2.Transform(shift, Mathg.RotationMatrix2D(angleOffset + i * (float)Math.PI * 2f / monsterCount));
                    Vector3 position3 = new Vector3(position.X, -1f, position.Y);

                    Monster monster = Mathg.DiscreteChoiceFn(rand, new Func<Monster>[]
                    {
                        () => new BasicMonster(position3, monsterHP, monsterDamage),
                        () => new ShotgunDude(position3, monsterHP, monsterDamage),
                        () => new SpinnyBoi(position3, monsterHP * 2, monsterDamage),
                        () => new Spooper(position3, monsterHP * 1.5f, monsterDamage)
                    }, monsterChances);

                    scene.AddGameObject(monster);
                }

                if (monsterCount != 1)
                {
                    flags.ClearPerimeter = false;
                }
                else
                {
                    flags.ClearCenter = false;
                }
            }
            else
            {
                flags.ClearCenter = false;
                flags.ClearFloor = false;
                flags.ClearPerimeter = false;
            }

            if (rand.Next(8) == 0 || (flags.ClearPerimeter && rand.Next(2) == 0)) // special room
            {
                PopulateSpecialRoom(scene, zone, roomCenter, flags, ref results);
            }
            else
            {
                PopulateRoomCenter(scene, zone, roomCenter, flags, ref results);
                PopulateRoomWalls(scene, zone, roomCenter, flags, ref results);
            }

            return results;
        }

        protected void PopulateSpecialRoom(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rng = rand.Next(6);
            if (flags.ClearPerimeter)
            {
                if (rng == 0 || rng == 1)
                {
                    SceneStructures.Arena(WallTexture)(scene, zone, roomCenter);
                }
            }
            if (flags.ClearFloor)
            {
                if (rng == 2 || rng == 3)
                {
                    results.GenerateFloor = false;
                    SceneStructures.Pit(FloorTexture, WallTexture)(scene, zone, roomCenter);

                    if (rng == 3 && flags.ClearCenter)
                    {
                        SceneStructures.PitPillar(WallTexture)(scene, zone, roomCenter);
                    }
                }
            }
        }

        protected void PopulateRoomCenter(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rnd = rand.Next(4);
            if (flags.ClearCenter)
            {
                if (rnd == 0)
                {
                    SceneStructures.PillarSmall(WallTexture)(scene, zone, roomCenter);
                }
                else if (rnd == 1)
                {
                    SceneStructures.PillarBig(WallTexture)(scene, zone, roomCenter);
                }
            }
            else
            {
                if (rnd == 2)
                {
                    SceneStructures.Pillars4Inner(WallTexture)(scene, zone, roomCenter);
                }
            }
        }

        protected void PopulateRoomWalls(Scene scene, Zone zone, Vector3 roomCenter, PopulateSchemeFlags flags, ref PopulateRoomResults results)
        {
            int rnd = rand.Next(4);
            if (flags.NotJoint && rnd == 0)
            {
                SceneStructures.CutCorners(WallTexture)(scene, zone, roomCenter);
            }
            else if (rnd == 1)
            {
                SceneStructures.Pillars4Outer(WallTexture)(scene, zone, roomCenter);
            }
        }
    }
}
