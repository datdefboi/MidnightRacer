using System;
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

        public float Speed { get; set; } = 0f;
        private const float MaxSpeed = 300f;
        private const float Acceleration = 90f;
        private float overload = 0;
        
        private const float AxisDistance = 36;

        public float SteerAngle { get; set; } = 0f;
        private const float SteerSpeed = 45f;
        private const float MaxSteerAngle = 65f;

        public float Fuel { get; set; } = 100;
        private const float MaxFuel = 100;
        private const float FuelConsumptionPerSec = 10;

        public Color headColor = Color.Green;

        private static VectorGroup headVGroup =
            VectorGroup.FromRect(new Size(34, 30 - 8)).Move(new Vector(16, 0));

        private static VectorGroup bodyVGroup = VectorGroup.
            FromRect(new Size(70 - 15, 30 - 14)).Move(new Vector(-((15) / 2), 0));

        private static VectorGroup wheelVGroup = VectorGroup.FromRect(new Size(12, 6));


        private static IEnumerable<(IEnumerable<Vector>, float)> wheesMeta =
            new (IEnumerable<Vector>, float)[]
            {
                (new[] {new Vector(-20, 16), new Vector(-20, -16)}, 0f),
                (new[] {new Vector(-20, 9), new Vector(-20, -9)}, 0f),


                (new[] {new Vector(8, 14), new Vector(8, -14)}, 0.5f),
                (new[] {new Vector(26, 14), new Vector(26, -14)}, 1f)
            };


        public override void Render()
        {
            foreach (var (points, turnRatio) in wheesMeta)
            foreach (var p in points)
            {
                var wheel = wheelVGroup.Rotate(Vector.Zero, SteerAngle * turnRatio).
                    Move(Origin).Move(p).Rotate(Vector.Zero, Rotation).Move(Position);
                
                if (Abs(overload) > 0.95f)
                {
                    View.MarkSlip(wheel);
                }
                
                View.FillPolygon(wheel, Color.Black, true);
            }

            VectorGroup Prepare(VectorGroup vg) =>
                vg.Move(Origin).Rotate(Vector.Zero, Rotation).Move(Position);

            var headPoints = Prepare(headVGroup);

            var bodyPoints = Prepare(bodyVGroup);

            View.FillPolygon(bodyPoints, Color.White, false);
            View.DrawPolygon(bodyPoints, Color.Black, false);

            var fuelRatio = Fuel / MaxFuel;

            View.FillPolygon(headPoints, Color.FromArgb(
                    (int) (255 - ((255 - headColor.R) * fuelRatio)),
                    (int) (255 - ((255 - headColor.G) * fuelRatio)),
                    (int) (255 - ((255 - headColor.B) * fuelRatio))),
                true);

            View.DrawPolygon(headPoints, Color.Black, true);
        }

        public override void Update(float d)
        {
            if (Keyboard.Pressed[Keys.Up] && Speed < MaxSpeed)
                Speed += Acceleration * d;
            if (Keyboard.Pressed[Keys.Down] && Speed > 0)
                Speed -= Acceleration * d;
            Debug.Write("DBG:Speed KM/h", (Speed / AxisDistance * 4.4f) / 3.6 * 2);

            if (Keyboard.Pressed[Keys.Left])
                SteerAngle += SteerSpeed * d;
            else if (Keyboard.Pressed[Keys.Right])
                SteerAngle -= SteerSpeed * d;
            else if (Abs(SteerAngle) > 0.1f)
                SteerAngle -= Sign(SteerAngle) * 40f * d;

            Debug.Write("DBG:steer angle", SteerAngle);

            var turnRadius = AxisDistance / Sin(SteerAngle);
            Debug.Write("DBG:radius", turnRadius);
            var currentRotation = ToDeg(Speed / turnRadius); 
            overload = Speed / turnRadius;
            if (Math.Abs(overload) > 1)
            {
                var angle = ToDeg((float) Math.Asin(
                    AxisDistance / Speed /
                    Math.Abs(overload)));
                SteerAngle = Sign(SteerAngle) * angle;
            }
            
            SteerAngle = Sign(SteerAngle) * Min(MaxSteerAngle, Abs(SteerAngle));

            Debug.Write("DBG:ovd", overload);
            Debug.Write("DBG:cur rot", currentRotation);
            Rotate(currentRotation * d);
            //speed -= steerAngle * angularDrag * d;

            Fuel -= FuelConsumptionPerSec * d;

            if (Fuel < 0)
            {
                Fuel = 0;
                Destroy();
            }

            Position += Vector.FromAngle(Rotation) * Speed * d;
        }

        public void OnIntersection(GameObject opposite)
        {
            switch (opposite)
            {
                case RoadCone cone:
                    Destroy();

                    break;
                case PetrolCan can:
                    Fuel = MaxFuel;
                    Stats.CansEatten++;
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