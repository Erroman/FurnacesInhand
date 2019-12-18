using System;
using System.Windows;
using System.Windows.Media;

namespace RulerControls
{
 
    static class TransformWorldToScreen
    {
        private static Matrix WtoDMatrix, DtoWMatrix;
        public static void PrepareTransformations(
        double wxmin, double wxmax, double wymin, double wymax,
        double dxmin, double dxmax, double dymax, double dymin)
        {
            // Make WtoD.
            WtoDMatrix = Matrix.Identity;
            WtoDMatrix.Translate(-wxmin, -wymin);

            double xscale = (dxmax - dxmin) / (wxmax - wxmin);
            double yscale = (dymax - dymin) / (wymax - wymin);
            WtoDMatrix.Scale(xscale, yscale);

            WtoDMatrix.Translate(dxmin, dymin);

            // Make DtoW.
            DtoWMatrix = WtoDMatrix;
            DtoWMatrix.Invert();
        }

        // Transform a point from world to device coordinates.
        public static Point WtoD(Point point)
        {
            return WtoDMatrix.Transform(point);
        }

        // Transform a point from device to world coordinates.
        public static Point DtoW(Point point)
        {
            return DtoWMatrix.Transform(point);
        }

        public static double OptimalSpacing(double original)
        {
            double[] da = { 1.0, 2.0, 5.0 };
            double multiplier = Math.Pow(10, Math.Floor(Math.Log(original, 10)));
            double dmin = 100 * multiplier;
            double spacing = 0.0;
            double mn = 100;
            foreach (double d in da)
            {
                double delta = Math.Abs(original - d * multiplier);
                if (delta < dmin)
                {
                    dmin = delta;
                    spacing = d * multiplier;
                }
                if (d < mn)
                {
                    mn = d;
                }
            }
            if (Math.Abs(original - 10 * mn * multiplier) < Math.Abs(original - spacing))
                spacing = 10 * mn * multiplier;
            return spacing;
        }
    }
}
