using System.Drawing;
using MidnightRacer.Engine;

namespace MidnightRacer.GameObjects
{
    public class Wall : GameObject, IIntersectable
    {
        protected override Vector Origin { get; set; }
        protected override VectorGroup Bounds { get; set; }
        protected override float BoundsRadius { get; set; }

        public Wall(Vector from, Vector to)
        {
            var center = (from + to) * 0.5f;
            Bounds = VectorGroup.FromRect(
                new Size((int) MathF.Abs(from.X - to.X),
                    (int) MathF.Abs(from.Y - to.Y)));
            Position = center;
            BoundsRadius = from.DistaceTo(to);
        }

        public override void Render() { }
        public override void Update(float elapsed) { }
        public void OnIntersection(GameObject opposite) {  }
    }
}