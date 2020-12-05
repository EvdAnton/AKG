using System;
using System.Drawing;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public class WireModel
    {
        private readonly ObjReader.ObjReader _objReader;
        private Matrix<float> _transformMatrix;

        public WireModel(string path, int width, int height)
        {
            _objReader = ReadDataFromObjFile(path);
            
            _transformMatrix = MathNetExtension.GetResultMatrix(width, height);
        }

        public Bitmap ScaleAndDraw(float scaleValue, Bitmap image)
        {
            _transformMatrix = _transformMatrix.ScaleModelMatrix(scaleValue);

            return Draw(image);
        }
        
        public Bitmap Draw(Bitmap image)
        {
            foreach (var line in _objReader.GetVertices())
            {
                var startVector = _transformMatrix.ScaleModelMatrix(0.5f) * line.StartVector;
                var endVector = _transformMatrix.ScaleModelMatrix(0.5f) * line.EndVector;

                Draw(startVector[0], startVector[1], endVector[0],
                    endVector[1], image);
            }

            return image;
        }
        
        private static ObjReader.ObjReader ReadDataFromObjFile(string path)
        {
            var objReader = new ObjReader.ObjReader();

            path = Directory.GetCurrentDirectory() + path;
            
            objReader.ReadObjFile(path);

            return objReader;
        }
        
        private void Draw(float x0, float y0, float x1, float y1, Bitmap image)
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
                var tempX = (int)(isYSteep ? y : x);
                var tempY = (int)(isYSteep ? x : y);

                tempX = CheckValue(tempX, image.Width);
                tempY = CheckValue(tempY, image.Height);
                
                image.SetPixel(tempX, tempY, Color.Black);
                error -= dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }

        private static int CheckValue(int value, int maxValue)
        {
            if (value >= maxValue)
            {
                value = maxValue - 1;
            }

            if (value < 0)
            {
                value = 0;
            }

            return value;
        }
        
        private static void Swap(ref float x0, ref float x1)
        {
            var t = x0;
            x0 = x1;
            x1 = t;
        }
    }
}