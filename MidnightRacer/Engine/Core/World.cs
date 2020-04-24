using MidnightRacer.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MidnightRacer.Engine
{
    static class World
    {
        private static List<GameObject> GameObjectsPool = new List<GameObject>();
        private static int rendersElapsed = 0;
        private static DateTime lastRender = DateTime.Now;
        private static DateTime creationTime = DateTime.Now;

        private static Random rand = new Random();

        public static bool Renders = false;

        static Queue<GameObject> entraceQueue = new Queue<GameObject>();

        public static void InitWorld()
        {
            GameObjectsPool.Clear();
            Stats.CanEatten = 0;
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
                new Vector(View.Width+10, View.Height),
                new Vector(View.Width+10, 0));

            GameObjectsPool.Add(topWall);
            GameObjectsPool.Add(bottomWall);
            GameObjectsPool.Add(leftWall);
            GameObjectsPool.Add(RightWall);

            var car = new Car();
            var cone = new PetrolCan();
            cone.Translate(new Vector(200, 200));
            car.Translate(new Vector(50, 100));
            GameObjectsPool.Add(car);
            GameObjectsPool.Add(cone);
        }

        private static void SpawnCar()
        {
            var car = new Car();
            car.Translate(new Vector(50, 50));
            GameObjectsPool.Add(car);
        }

        private static bool CheckIntersections(GameObject gm)
        {
            foreach (var opGo in GameObjectsPool)
                if (gm != opGo)
                    if (gm.CheckIntersections(opGo))
                    {
                        return true;
                    }

            return false;
        }

        public static void RenderWorld(Graphics g)
        {
            Renders = true;

            Debug.logBuffer = "";

            {
                Debug.Write("To spawn car", "S");
                if (Keyboard.Pressed[Keys.S])
                {
                    SpawnCar();
                    Keyboard.Pressed[Keys.S] = false;
                }
            }

            Engine.View.currentGraphics = g;

            Engine.View.DrawDebugInfo = Keyboard.Pressed[Keys.Space];

            var now = DateTime.Now;
            var elapsed = (float) (now - lastRender).TotalSeconds;
            lastRender = now;

            Debug.Write("FPS", 1 / elapsed);

            {
                var destroyQueue = new Queue<GameObject>();

                foreach (var go in GameObjectsPool)
                {
                    if (go.ReadyForDestroy)
                    {
                        destroyQueue.Enqueue(go);

                        continue;
                    }

                    foreach (var opGo in GameObjectsPool)
                        if (go != opGo)
                            go.CheckIntersections(opGo);

                    go.Update(elapsed);
                    go.Render();
                }

                foreach (var go in destroyQueue)
                {
                    GameObjectsPool.Remove(go);

                    if (go is Car)
                    {
                        InitWorld();
                    }
                }

                destroyQueue.Clear();

                foreach (var go in entraceQueue)
                {
                    GameObjectsPool.Add(go);
                }

                entraceQueue.Clear();
            }


            rendersElapsed++;

            // draw debug info
            var font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular);
            g.DrawString(Debug.logBuffer, font, Brushes.Gray,
                new PointF(View.Width - 200, 0));

            g.DrawString(Stats.CanEatten.ToString(),
                new Font(FontFamily.GenericMonospace, 70, FontStyle.Regular),
                Brushes.Teal, new PointF(20, 10));

            g.DrawString(
                Math.Round((DateTime.Now - creationTime).TotalSeconds).ToString(),
                new Font(FontFamily.GenericMonospace, 20, FontStyle.Regular),
                Brushes.Teal, new PointF(10, 10));
            Renders = false;
        }


        public static void HandleKeyDown(Keys key) { Keyboard.Pressed[key] = true; }
        public static void HandleKeyUp(Keys key) { Keyboard.Pressed[key] = false; }

        public static void AddInEmptySpace<T>() where T : GameObject, new()
        {
            var intersects = false;
            GameObject gm;
            do
            {
                gm = new T();
                gm.Translate(new Vector(rand.Next(50, (int) (View.Width - 70)),
                    rand.Next(50, (int) (View.Height - 70))));
                intersects = CheckIntersections(gm);
            } while (intersects);

            entraceQueue.Enqueue(gm);
        }

        public static void HandleConeTimerTick() { AddInEmptySpace<RoadCone>(); }
    }
}