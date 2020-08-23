using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using MidnightRacer.Engine.Persistence;
using MidnightRacer.GameObjects;

namespace MidnightRacer.Engine
{
    public static partial class World
    {
        private static void SaveStats()
        {
            File.WriteAllText("stats.json", JsonSerializer.Serialize(users));
        }

        private static void LoadStats()
        {
            try
            {
                users = JsonSerializer.Deserialize<List<UserRecord>>
                    (File.ReadAllText("stats.json"));
            }
            catch (Exception e)
            {
                File.WriteAllText("stats.json", "[]");
            }
        }

        private static void SaveMap()
        {
            var save = new GameSave();
            View.slips.Save("slips.bmp");

            save.cansEatten = (int) Stats.CansEatten;
            save.creation = creationTime;

            foreach (var go in GameObjectsPool)
            {
                switch (go)
                {
                    case Car c:
                        save.cars.Add(c);

                        break;
                    case PetrolCan c:
                        save.cans.Add(c);

                        break;
                    case RoadCone c:
                        save.cones.Add(c);

                        break;
                }
            }

            File.WriteAllText("save.json", JsonSerializer.Serialize(save));
        }

        private static void LoadSave()
        {
            var save =
                JsonSerializer.Deserialize<GameSave>(File.ReadAllText("save.json"));
          
            StartNewGame();
            GameObjectsPool.Clear();
            GameObjectsPool.AddRange(save.cans);
            GameObjectsPool.AddRange(save.cars);
            GameObjectsPool.AddRange(save.cones);

            Stats.CansEatten = save.cansEatten;
            creationTime = save.creation;

            View.InitSlips(new Bitmap(Image.FromFile("slips.bmp")));
        }
    }
}