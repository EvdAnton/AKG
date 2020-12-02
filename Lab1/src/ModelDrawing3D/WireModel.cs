using System;
using System.Drawing;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    // TODO : twice buffer for image?
    
    public class WireModel
    {
        private readonly Graphics _graphics;
        private readonly ObjReader.ObjReader _objReader;
        private readonly Matrix<float> _transformMatrix;

        public WireModel(string path, Graphics graphics, int width, int height)
        {
            _graphics = graphics;
            
            _objReader = ReadDataFromObjFile(path);
            
            _transformMatrix = MathNetExtension.GetResultMatrix(width, height);
        }

        public void Draw(Brush brush)
        {
            foreach (var line in _objReader.GetVertices())
            {
                var startVector = line.StartVector * _transformMatrix;
                var endVector = line.EndVector * _transformMatrix;
                
                Draw(_graphics, brush, startVector[0], startVector[1], endVector[0],
                    endVector[1]);
            }
        }
        
        private static ObjReader.ObjReader ReadDataFromObjFile(string path)
        {
            var objReader = new ObjReader.ObjReader();

            path = Directory.GetCurrentDirectory() + path;
            
            objReader.ReadObjFile(path);

            return objReader;
        }
        
        private static void Draw(Graphics g, Brush brush, float x0, float y0, 
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
    }
}