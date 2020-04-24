using System.Drawing;

namespace MidnightRacer.Engine
{
    public abstract class GameObject
    {
        protected Vector Position { get; set; } = Vector.Zero;
        protected abstract Vector Origin { get; set; }
        protected float Rotation { get; set; } = 0f;
        protected abstract VectorGroup Bounds { get; set; }
        protected abstract float BoundsRadius { get; set; }

        public bool ReadyForDestroy { get; private set; }

        public abstract void Render();
        public abstract void Update(float elapsed);

        public void Translate(Vector movement)
        {
            Position += movement;
        }

        public void Rotate(float angle)
        {
            Rotation += angle;
        }

        public void Destroy()
        {
            ReadyForDestroy = true;
        }

        public bool CheckIntersections(GameObject go)
        {
            var selfBounds = Bounds
                .Move(Origin)
                .Rotate(Vector.Zero, Rotation)
                .Move(Position);

            var opposBounds =
                go
                 .Bounds
                 .Move(go.Origin)
                 .Rotate(Vector.Zero, go.Rotation)
                 .Move(go.Position);

            if (View.DrawDebugInfo)
            {
                View.DrawPolygon(selfBounds, Color.Red);
                View.DrawPolygon(opposBounds, Color.Red);
            }

            if (Position.DistaceTo(go.Position) < BoundsRadius + go.BoundsRadius)
            {
                var isInters = selfBounds.IsIntersectsByBounding(opposBounds);

                if (isInters)
                {
                    if (go is IIntersectable)
                        ((IIntersectable) go).OnIntersection(this);

                    if (this is IIntersectable)
                        ((IIntersectable) this).OnIntersection(go);

                    return true;
                }
            }

            return false;
        }
    }
}
