using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MidnightRacer.Engine.Persistence;
using MidnightRacer.GameObjects;

namespace MidnightRacer.Engine
{
    public static partial class World
    {
        private static Car currentCar;

        private static Random rand = new Random();

        private static DateTime creationTime = DateTime.Now;

        private static string userName = "player";

        static RenameDialog renameDialog = new RenameDialog();

        private static List<UserRecord> users =
            new List<UserRecord>();

        private static UserRecord currentUser => users.Find(u => u.Name == userName);

        public static void ResumeGame() => InGame = true;
        public static void PauseGame() => InGame = false;

        public static void StartNewGame()
        {
            Stats.maxCanEaten = (int) Math.Max(Stats.CansEatten, Stats.maxCanEaten);
            InitWorld();
        }

        public static void InitWorld()
        {
            GameObjectsPool.Clear();
            Stats.CansEatten = 0;
            creationTime = DateTime.Now;

            var topWall = new Wall(
                new Vector(0, View.Height + 10),
                new Vector(View.Width, View.Height + 10));

            var bottomWall = new Wall(
                new Vector(0, -10),
                new Vector(View.Width, -10));

            var leftWall = new Wall(
                new Vector(-10, View.Height),
                new Vector(-10, 0));

            var RightWall = new Wall(
                new Vector(View.Width + 10, View.Height),
                new Vector(View.Width + 10, 0));

            GameObjectsPool.Add(topWall);
            GameObjectsPool.Add(bottomWall);
            GameObjectsPool.Add(leftWall);
            GameObjectsPool.Add(RightWall);

            var can = new PetrolCan();
            can.Translate(new Vector(200, 200));
            GameObjectsPool.Add(can);
            SpawnCar();
        }

        private static void SpawnCar()
        {
            var car = new Car();
            car.Translate(new Vector(100, 100));
            GameObjectsPool.Add(car);
            currentCar = car;
        }

        private static void OnTick()
        {
            ConsumeHighscore();
            Debug.Write("High score", currentUser.Highscore);
            
            Debug.Write("----------", "----------");
            Debug.Write("Leader board", "");
            foreach (var u in users)
            {
                Debug.Write(u.Name, u.Highscore);
            }
        }

        private static void FetchUserExists()
        {
            if (users.FirstOrDefault(p => p.Name == userName) == default)
                users.Add(new UserRecord
                {
                    Name = userName
                });
        }

        public static void OnCreation()
        {
            LoadStats();
            FetchUserExists();
        }

        private static void ConsumeHighscore()
        {
            if (Stats.CansEatten > currentUser.Highscore)
            {
                currentUser.Highscore = Stats.CansEatten;
                SaveStats();
            }
        }

        private static void HandleKeys()
        {
            Debug.Write("New game", "N");
            Debug.Write("Save game", "S");
            Debug.Write("Load game", "L");
            Debug.Write("Nickname", userName);
            Debug.Write("Change nick", "C");

            if (Keyboard.Pressed[Keys.N])
            {
                Keyboard.Pressed[Keys.N] = false;
                StartNewGame();
            }

            if (Keyboard.Pressed[Keys.S])
            {
                Keyboard.Pressed[Keys.S] = false;
                SaveMap();
            }

            if (Keyboard.Pressed[Keys.L])
            {
                Keyboard.Pressed[Keys.L] = false;
                LoadSave();
            }

            if (Keyboard.Pressed[Keys.C])
            {
                Keyboard.Pressed[Keys.C] = false;
                PauseGame();
                renameDialog.UserName = userName;
                renameDialog.ShowDialog();
                userName = renameDialog.UserName;
                FetchUserExists();
                StartNewGame();
                ResumeGame();
            }
        }
    }
}