using ASCII_FPS.GameComponents;
using ASCII_FPS.GameComponents.Enemies;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Generator = ASCII_FPS.Scenes.SceneStructures.Generator;

namespace ASCII_FPS.Scenes.Generators
{
    public class SceneGeneratorJungle : SceneGenerator
    {
        private readonly float monsterHP;
        private readonly float monsterDamage;
        private readonly int monstersPerRoom;
        private readonly float[] monsterChances;

        protected override AsciiTexture WallTexture => Assets.jungleWallVinesTexture;
        protected override AsciiTexture FloorTexture => Assets.floorTexture;

        public SceneGeneratorJungle(ASCII_FPS game, int floor) : base(game)
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
        }

        protected override Collectible.Type?[,] DistributeCollectibles()
        {
            return SceneGenUtils.DistributeCollectibles(rand, size, exitRoom, accessible);
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
            float[] roomCorridors = Enumerable.Range(0, 4).Select(t => corridorWidths[x, y, t]).ToArray();

            if ((x != exitRoom.X || y != exitRoom.Y) && (x != size / 2 || y != size / 2))
            {
                int monsterCount = rand.Next(2, monstersPerRoom + 1);
                game.PlayerStats.totalMonsters += monsterCount;
                List<Vector3> spawnPoints = new List<Vector3>();
                new List<(int, int)> { (-1, -1), (-1, 1), (1, -1), (1, 1) }
                    .ForEach(((int, int) p) => {
                        int xx = p.Item1;
                        int zz = p.Item2;
                        spawnPoints.Add(new Vector3(xx * 30f, -1f, zz * 30f));
                        spawnPoints.Add(new Vector3(xx * 22f, -1f, zz * 30f));
                        spawnPoints.Add(new Vector3(xx * 15f, -1f, zz * 30f));
                        spawnPoints.Add(new Vector3(xx * 30f, -1f, zz * 22f));
                        spawnPoints.Add(new Vector3(xx * 30f, -1f, zz * 15f));
                    });
                Mathg.Shuffle(rand, spawnPoints);

                for (int i = 0; i < Math.Min(monsterCount, spawnPoints.Count); i++)
                {
                    Vector3 position = spawnPoints[i] + roomCenter;
                    Monster monster = Mathg.DiscreteChoiceFn(rand, new Func<Monster>[]
                    {
                        () => new BasicMonster(position, monsterHP, monsterDamage),
                        () => new ShotgunDude(position, monsterHP, monsterDamage),
                        () => new SpinnyBoi(position, monsterHP * 2, monsterDamage),
                        () => new Spooper(position, monsterHP * 1.5f, monsterDamage)
                    }, monsterChances);

                    scene.AddGameObject(monster);
                }


                List<Generator> generators = new List<Generator>
                {
                    SceneStructures.Pillars4Inner(WallTexture),
                    SceneStructures.JungleBushes()
                };

                if (flags.ClearCenter)
                {
                    generators.Add(SceneStructures.PillarBig(WallTexture));
                    generators.Add(SceneStructures.PillarSmall(WallTexture));
                }

                Mathg.DiscreteChoice(rand, generators).Invoke(scene, zone, roomCenter);
                SceneStructures.JungleWalls(Assets.jungleWallTexture).Invoke(scene, zone, roomCenter);
            }

            return results;
        }
    }
}
