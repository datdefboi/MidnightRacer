using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MidnightRacer.GameObjects;

namespace MidnightRacer.Engine
{
    public static partial class World
    {
        private static List<GameObject> GameObjectsPool = new List<GameObject>();
        private static DateTime lastRender = DateTime.Now;

        public static bool InGame = true;

        private static Queue<GameObject> entraceQueue = new Queue<GameObject>();

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

        private static void DoWorldWork(float elapsed)
        {
            var destroyQueue = new Queue<GameObject>();
            
            for (var i = 0; i < GameObjectsPool.Count; i++)
            {
                var go = GameObjectsPool[i];
                if (go.ReadyForDestroy)
                {
                    destroyQueue.Enqueue(go);

                    continue;
                }

                if (InGame)
                {
                    for (var j = i + 1; j < GameObjectsPool.Count; j++)
                        go.CheckIntersections(GameObjectsPool[j]);

                    go.Update(elapsed);
                }

                try
                {
                    go.Render();
                }
                catch (Exception ex
                ) { } // это ужасно, я знаю, но GDI багует. То есть - !!!!!НЕ УБИРАТЬ ОБРАБОТЧИК!!!!
            }

            foreach (var go in destroyQueue)
            {
                GameObjectsPool.Remove(go);

                if (go is Car)
                {
                    StartNewGame();
                }
            }

            destroyQueue.Clear();

            foreach (var go in entraceQueue)
            {
                GameObjectsPool.Add(go);
            }

            entraceQueue.Clear();
        }

        public static void DoWorldTick(Graphics g)
        {
            Debug.logBuffer = "";

            Engine.View.currentGraphics = g;

            Engine.View.DrawDebugInfo = Keyboard.Pressed[Keys.Space];

            var now = DateTime.Now;
            var elapsed = (float) (now - lastRender).TotalSeconds;
            lastRender = now;

            Debug.Write("FPS", 1 / elapsed);
            
            g.DrawImage(View.slips, Point.Empty);

            HandleKeys();
            DoWorldWork(elapsed);

            OnTick();

            // draw debug info
            var font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular);
            g.DrawString(Debug.logBuffer, font, Brushes.Gray,
                new PointF(View.Width - 300, 0));

            g.DrawString(Stats.CansEatten.ToString(),
                new Font(FontFamily.GenericMonospace, 70, FontStyle.Regular),
                Brushes.Teal, new PointF(20, 10));

            g.DrawString(
                Math.Round((DateTime.Now - creationTime).TotalSeconds).ToString(),
                new Font(FontFamily.GenericMonospace, 20, FontStyle.Regular),
                Brushes.Teal, new PointF(10, 10));
        }

        public static void HandleKeyDown(Keys key)
        {
            if (InGame) Keyboard.Pressed[key] = true;
        }

        public static void HandleKeyUp(Keys key)
        {
            if (InGame) Keyboard.Pressed[key] = false;
        }

        public static void AddInEmptySpace<T>() where T : GameObject, new()
        {
            var intersects = false;
            GameObject gm;
            do
            {
                gm = new T();
                gm.Translate(new Vector(rand.Next(50, (int) (View.Width - 70)),
                    rand.Next(50, (int) (View.Height - 70))));
                intersects = CheckIntersections(gm)
                             ||
                             MathF.Abs((currentCar.Rotation -
                                        (gm.Position - currentCar.Position).Angle)) < 45
                             ||
                             currentCar.Position.DistaceTo(gm.Position) < 100;
                // in viewport
            } while (intersects);

            entraceQueue.Enqueue(gm);
        }

        public static void HandleConeTimerTick()
        {
            if (InGame) AddInEmptySpace<RoadCone>();
        }
    }
}