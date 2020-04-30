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

        public static Bitmap slips;

        private static Graphics slipsG;

        public static void InitSlips(Bitmap bmp = null)
        {
            if (bmp != null)
                slips = bmp;
            else
                slips = new Bitmap((int) Width, (int) Height);
            slipsG = Graphics.FromImage(slips);
        }

        public static void MarkSlip(VectorGroup points)
        {
            var normalizedPoints = NormalizeCoords(points);

            slipsG.FillPolygon(new SolidBrush(Color.LightGray), normalizedPoints);
        }


        private static PointF[] NormalizeCoords(VectorGroup group) =>
            group.Select(p => new PointF(p.X, Height - p.Y)).ToArray();

        public static void DrawPolygon(VectorGroup points, Color color, bool isCurved =
                                           false, float stroke = 1)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                currentGraphics.DrawClosedCurve(new Pen(color, stroke), normalizedPoints);
            else
                currentGraphics.DrawPolygon(new Pen(color, stroke), normalizedPoints);
        }

        public static void FillPolygon(VectorGroup points, Color color,
                                       bool isCurved = false)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                currentGraphics.FillClosedCurve(new SolidBrush(color), normalizedPoints);
            else
                currentGraphics.FillPolygon(new SolidBrush(color), normalizedPoints);
        }

        public static void FillCircle(Vector position, float radius, Color color)
        {
            currentGraphics.FillEllipse(new SolidBrush(color), position.X - radius,
                Height - (position.Y + radius), radius * 2, radius * 2);
        }
    }
}