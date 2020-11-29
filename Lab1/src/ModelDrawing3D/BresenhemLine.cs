using System;
using System.Drawing;

namespace Lab1.ModelDrawing3D
{
    public struct BresenhemLine
    {
        public BresenhemLine(float x0, float y0, float x1, float y1)
        {
            FromPoint = new Point<float>(x0, y0);
            ToPoint = new Point<float>(x1, y1);
        }
        
        public Point<float> FromPoint { get; }
        public Point<float> ToPoint { get; }
        
        public static void Draw(Graphics g, Brush brush, float x0, float y0, 
            float x1, float y1)
        {
            var isYSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); 
            
            if (isYSteep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            var dx = x1 - x0;
            var dy = Math.Abs(y1 - y0);
            
            var error = dx / 2;
            var yStep = y0 < y1 ? 1 : -1;
            var y = y0;
            for (var x = x0; x <= x1; x++)
            {
                DrawPoint(g, brush, isYSteep ? y : x, isYSteep ? x : y);
                error -= dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
        
        private static void Swap(ref float x0, ref float x1)
        {
            var t = x0;
            x0 = x1;
            x1 = t;
        }
        
        private static void DrawPoint(Graphics g, Brush brush, float x, float y)
        {
            g.FillRectangle(brush, x, y, 1, 1);
        }
        
        public readonly struct Point<T> where T: struct
        {
            public Point(T x, T y)
            {
                X = x;
                Y = y;
            }
        
            public T X { get; }
            public T Y { get; }
        }
    }
}