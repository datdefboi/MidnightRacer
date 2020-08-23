using MidnightRacer.Engine;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using static MidnightRacer.Engine.MathF;

namespace MidnightRacer.GameObjects
{
    class PetrolCan : GameObject, IIntersectable
    {
        protected override VectorGroup Bounds { get; set; } = new VectorGroup(new
            []
            {
                new Vector(-20, -20), new Vector(-20, 10), new Vector(-10, 20),
                new Vector(20, 20), new Vector(20, -20)
            });

        protected override Vector Origin { get; set; } = Vector.Zero;

        protected override float BoundsRadius { get; set; } =
            Sqrt(Square(40) + Square(40));

        public Color color;
        public DateTime CreationTime = DateTime.Now;

        public override void Render()
        {
            var pts = Bounds.ToArray();
            var canPts = new VectorGroup(pts[0], pts[1], pts[4], pts[3], pts[0], pts[4],
                pts[1], pts[2],pts[3]);

            View.FillPolygon(new VectorGroup(pts).Move(Position), color, false);
            View.DrawPolygon(canPts.Move(Position), Color.Black, false, 2);
        }

        public PetrolCan()
        {
            var test = new Random();
            color = Color.FromArgb(test.Next(0, 150), test.Next(0, 150),
                test.Next(0, 150));
        }

        public override void Update(float elapsed)
        {
            if (DateTime.Now - CreationTime > TimeSpan.FromSeconds(10))
                Destroy();
        }

        public void OnIntersection(GameObject opposite)
        {
            switch (opposite)
            {
                case Car c:
                    Destroy();

                    break;
            }
        }
    }
}