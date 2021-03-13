using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASCII_FPS.GameComponents
{
    public static class Achievements
    {
        public class Entry
        {
            public Entry(string description)
            {
                ID = count;
                count++;
                Description = description;
            }

            private static int count = 0;

            public string Description { get; }
            public int Progress { get; set; }
            public int ID { get; }
        }


        private static Dictionary<string, Entry> progress;


        static Achievements()
        {
            progress = new Dictionary<string, Entry>
            {
                ["Level 2"] = new Entry("Reach floor 2 of the dungeon"),
                ["Level 3"] = new Entry("Reach floor 3 of the dungeon"),
                ["Level 4"] = new Entry("Reach floor 4 of the dungeon"),
                ["Level 5"] = new Entry("Reach floor 5 of the dungeon"),
                ["Level 11"] = new Entry("Reach floor 11 of the dungeon"),
                ["Level 16"] = new Entry("Reach floor 16 of the dungeon"),
                ["Level 21"] = new Entry("Reach floor 21 of the dungeon"),

                ["100% 5"] = new Entry("100% clear floors 1 - 5 (all monsters + all pickups)"),
                ["100% 10"] = new Entry("100% clear floors 1 - 10 (all monsters + all pickups)"),
                ["100% 15"] = new Entry("100% clear floors 1 - 15 (all monsters + all pickups)"),
                ["100% 20"] = new Entry("100% clear floors 1 - 20 (all monsters + all pickups)"),

                ["Barrel 10"] = new Entry("Pick up 10 bonuses during a single run"),
                ["Barrel 25"] = new Entry("Pick up 25 bonuses during a single run"),
                ["Barrel 50"] = new Entry("Pick up 50 bonuses during a single run"),
                ["Barrel 100"] = new Entry("Pick up 100 bonuses during a single run"),

                ["Monster 50"] = new Entry("Defeat 50 monsters during a sinle run"),
                ["Monster 100"] = new Entry("Defeat 100 monsters during a sinle run"),
                ["Monster 250"] = new Entry("Defeat 250 monsters during a sinle run"),
                ["Monster 500"] = new Entry("Defeat 500 monsters during a sinle run"),
                ["Monster 1000"] = new Entry("Defeat 1000 monsters during a sinle run"),
                ["Monster 2000"] = new Entry("Defeat 2000 monsters during a sinle run"),

                ["HP 2"] = new Entry("Have your max HP increased to 140"),
                ["HP 5"] = new Entry("Have your max HP increased to 200"),
                ["HP 10"] = new Entry("Have your max HP increased to 300"),
                ["HP 20"] = new Entry("Have your max HP increased to 500"),

                ["Armor 2"] = new Entry("Have your max armor increased to 140"),
                ["Armor 5"] = new Entry("Have your max armor increased to 200"),
                ["Armor 10"] = new Entry("Have your max armor increased to 300"),
                ["Armor 20"] = new Entry("Have your max armor increased to 500"),

                ["AP 2"] = new Entry("Upgrade your armor protection to level 2"),
                ["AP 5"] = new Entry("Upgrade your armor protection to level 5"),
                ["AP 10"] = new Entry("Upgrade your armor protection to level 10"),
                ["AP 25"] = new Entry("Upgrade your armor protection to level 25"),
                ["AP 35"] = new Entry("Max out your armor protection"),

                ["Speed 2"] = new Entry("Upgrade your shooting speed to level 2"),
                ["Speed 5"] = new Entry("Upgrade your shooting speed to level 5"),
                ["Speed 10"] = new Entry("Upgrade your shooting speed to level 10"),
                ["Speed 25"] = new Entry("Upgrade your shooting speed to level 25"),
            };

            Read();
        }


        public static void Unlock(string key, HUD hud)
        {
            if (progress[key].Progress == 0)
            {
                hud.AddNotification("You've unlocked an achievement!");
                progress[key].Progress = 1;
                Write();
            }
        }

        public static void UnlockLeveled(string key, int level, HUD hud)
        {
            string fullKey = key + " " + level;
            if (progress.ContainsKey(fullKey))
            {
                Unlock(fullKey, hud);
            }
        }

        public static List<Entry> Entries { get { return progress.Values.ToList(); } }


        private static void Read()
        {
            if (!File.Exists("./achievements.dat"))
                return;

            using (StreamReader reader = new StreamReader(File.Open("./achievements.dat", FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split('=');
                    if (line.Length != 2)
                        continue;

                    string key = line[0];
                    string data = line[1];
                    
                    if (progress.ContainsKey(key))
                    {
                        progress[key].Progress = int.Parse(data);
                    }
                }
            }
        }

        private static void Write()
        {
            using (StreamWriter writer = new StreamWriter(File.Open("./achievements.dat", FileMode.Create)))
            {
                foreach (string ID in progress.Keys)
                {
                    writer.WriteLine(ID + "=" + progress[ID].Progress);
                }
            }
        }
    }
}
