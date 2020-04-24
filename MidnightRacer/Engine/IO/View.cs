using System.Drawing;
using System.Linq;

namespace MidnightRacer.Engine
{
    static class View
    {
        public static Graphics currentGraphics;
        public static bool DrawDebugInfo;

        public static float Height;
        public static float Width;

        private static PointF[] NormalizeCoords(VectorGroup group) => group.Select(p => new PointF(p.X, Height - p.Y)).ToArray();

        public static void DrawPolygon(VectorGroup points, Color color, bool isCurved = false)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                currentGraphics.DrawClosedCurve(new Pen(color), normalizedPoints);
            else
                currentGraphics.DrawPolygon(new Pen(color), normalizedPoints);
        }

        public static void FillPolygon(VectorGroup points, Color color, bool isCurved = false)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                currentGraphics.FillClosedCurve(new SolidBrush(color), normalizedPoints);
            else
                currentGraphics.FillPolygon(new SolidBrush(color), normalizedPoints);
        }
        public static void FillCircle(Vector position, float radius, Color color)
        {
            currentGraphics.FillEllipse(new SolidBrush(color), position.X - radius, Height - (position.Y + radius), radius*2, radius*2);
        }
    }
}
