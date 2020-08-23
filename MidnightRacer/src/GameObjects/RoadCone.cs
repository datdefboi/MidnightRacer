using MidnightRacer.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MidnightRacer.Engine.MathF;

namespace MidnightRacer.GameObjects
{
    class RoadCone : GameObject, IIntersectable
    {
        protected override VectorGroup Bounds { get; set; } = VectorGroup.FromRect(new Size(30, 30));
        protected override Vector Origin { get; set; } = new Vector(0, 0);
        protected override float BoundsRadius { get; set; } =
            Sqrt(Square(40) + Square(40));

        public void OnIntersection(GameObject opposite)
        {
            switch (opposite)
            {
                case Car c:
                    Destroy();
                    break;
                case RoadCone c:
                    Destroy();
                    break;
            }
        }

        public override void Render()
        {
            View.FillPolygon(Bounds.Move(Position-Origin), Color.Orange, true);
            View.FillCircle(Position, 10, Color.White);
            View.FillCircle(Position, 8, Color.Orange);
            View.FillCircle(Position, 6, Color.White);
            View.FillCircle(Position, 4, Color.Orange);
        }

        public override void Update(float elapsed)
        {

        }
    }
}
