using MidnightRacer.Engine;
using System.Collections.Generic;
using System.Drawing;
using static MidnightRacer.Engine.MathF;
using Keys = System.Windows.Forms.Keys;

namespace MidnightRacer.GameObjects
{
    class Car : GameObject, IIntersectable
    {
        protected override VectorGroup Bounds { get; set; } =
            VectorGroup.FromRect(new Size(70, 30));

        protected override float BoundsRadius { get; set; } =
            Sqrt(Square(70) + Square(30));

        protected override Vector Origin { get; set; } = new Vector(22, 0);

        private float speed = 0f;
        private float maxSpeed = 300f;
        private float a = 90f;

        private float steerAngle = 0f;
        private float steerSpeed = 45f;
        private float maxSteerAngle = 45f;

        private float fuel = 100;
        private float maxFuel = 100;
        private float fuelConsuptionPerSec = 10;
        private Color headColor = Color.Green;

        private static VectorGroup headVGroup =
            VectorGroup.FromRect(new Size(34, 30 - 8)).Move(new Vector(16, 0));

        private static VectorGroup bodyVGroup = VectorGroup.
            FromRect(new Size(70 - 15, 30 - 14)).Move(new Vector(-((15) / 2), 0));

        private static VectorGroup wheelVGroup = VectorGroup.FromRect(new Size(12, 6));


        private static IEnumerable<(IEnumerable<Vector>, float)> wheesMeta =
            new (IEnumerable<Vector>, float)[]
            {
                (new Vector[] {new Vector(-20, 16), new Vector(-20, -16)}, 0f),
                (new Vector[] {new Vector(-20, 9), new Vector(-20, -9)}, 0f),


                (new Vector[] {new Vector(8, 14), new Vector(8, -14)}, 0.5f),
                (new Vector[] {new Vector(26, 14), new Vector(26, -14)}, 1f)
            };


        public override void Render()
        {
            foreach (var (points, turnRatio) in wheesMeta)
            foreach (var p in points)
            {
                var wheel = wheelVGroup.Rotate(Vector.Zero, steerAngle * turnRatio).
                    Move(Origin).Move(p).Rotate(Vector.Zero, Rotation).Move(Position);

                View.FillPolygon(wheel, Color.Black, true);
            }

            VectorGroup Prepare(VectorGroup vg) =>
                vg.Move(Origin).Rotate(Vector.Zero, Rotation).Move(Position);

            var headPoints = Prepare(headVGroup);

            var bodyPoints = Prepare(bodyVGroup);

            View.FillPolygon(bodyPoints, Color.White, false);
            View.DrawPolygon(bodyPoints, Color.Black, false);

            var fuelRatio = fuel / maxFuel;

            View.FillPolygon(headPoints, Color.FromArgb(
                    (int) (255 - ((255 - headColor.R) * fuelRatio)),
                    (int) (255 - ((255 - headColor.G) * fuelRatio)),
                    (int) (255 - ((255 - headColor.B) * fuelRatio))),
                true);

            View.DrawPolygon(headPoints, Color.Black, true);
        }

        public override void Update(float d)
        {
            if (Keyboard.Pressed[Keys.Up] && speed < maxSpeed)
                speed += a * d;
            if (Keyboard.Pressed[Keys.Down] && speed > 0)
                speed -= a * d;
            Debug.Write("speed", speed);

            if (Keyboard.Pressed[Keys.Left])
                steerAngle += steerSpeed * d;
            else if (Keyboard.Pressed[Keys.Right])
                steerAngle -= steerSpeed * d;
            else if (Abs(steerAngle) > 0.1f)
                steerAngle -= Sign(steerAngle) * 40f * d;

            var availableSteer = 1600 / (speed/2 + 22);/*picked up formula for this car*/
            steerAngle = Sign(steerAngle) * (Min(Abs(steerAngle), availableSteer));

            Debug.Write("steer angle", steerAngle);

            var turnRadius = 38 /*расстояние между осями*/ / Sin(steerAngle);
            var currentRotation = ToDeg(speed / turnRadius /*в угловых минутах*/);
            
            Debug.Write("cur rot", currentRotation);
            Rotate(currentRotation * d);
            //speed -= steerAngle * angularDrag * d;

            fuel -= fuelConsuptionPerSec * d;

            if (fuel < 0)
            {
                fuel = 0;
                Destroy();
            }

            Position += Vector.FromAngle(Rotation) * speed * d;
        }

        public void OnIntersection(GameObject opposite)
        {
            switch (opposite)
            {
                case RoadCone cone:
                    Destroy();

                    break;
                case PetrolCan can:
                    fuel = maxFuel;
                    Stats.CanEatten++;
                    World.AddInEmptySpace<PetrolCan>();
                    headColor = can.color;

                    break;
                case Wall w:
                    Destroy();

                    break;
            }
        }
    }
}