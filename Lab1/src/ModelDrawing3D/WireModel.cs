using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public class WireModel
    {
        private const byte WHITE = 255;
        private const int WIDTH = 600;
        private const int HEIGHT = 600;
        
        private readonly ObjReader.ObjReader _objReader;
        private readonly int _height;
        private readonly int _width;
        private readonly byte[] _rgbValues;
        
        private Matrix<float> _transformMatrix;
        private Matrix<float> _modelMatrix;
        private Matrix<float> _viewMatrix;
        private Matrix<float> _projectionMatrix;
        private Camera _camera;

        public WireModel(string path, int width, int height)
        {
            _width = width;
            _height = height;
            
            _objReader = ReadDataFromObjFile(path);
            _modelMatrix = MathNetExtension.GetModelMatrix();
            
            _camera = new Camera();
            _viewMatrix = _camera.GetViewMatrix();
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);
            
            _rgbValues = new byte[width * height];
        }


        public Bitmap ScaleAndDraw(float scaleValue)
        {
            _modelMatrix = _modelMatrix.Scale(scaleValue);

            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }

        private static Matrix<float> GetResultMatrix(Matrix<float> viewMatrix, Matrix<float> modelMatrix, Matrix<float> projectionMatrix)
        {
            return MathNetExtension.GetViewPortMatrix(WIDTH / 2f, HEIGHT / 2f)
                   * projectionMatrix
                   * viewMatrix
                   * modelMatrix;
        }

        public Bitmap XRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.XRotate(angel);
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }

        public Bitmap YRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.YRotate(angel);
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }
        
        
        public Bitmap ZRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.ZRotate(angel);
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }

        public Bitmap MoveAndDraw(float x = default, float y = default, float z = default)
        {
            _modelMatrix = _modelMatrix.Move(x, y, z);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }
        
        public Bitmap CameraMovement(CameraMovement direction)
        {
            _camera.ProcessKeyboard(direction);
            _viewMatrix = _camera.GetViewMatrix();

            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);
            
            return Draw();
        }

        public Bitmap ProcessMouseMovement(float xOffset, float yOffset)
        {
            _camera.ProcessMouseMovement(xOffset, yOffset);
            _viewMatrix = _camera.GetViewMatrix();
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);
            
            return Draw();
        }

        public Bitmap ProcessMouseScroll(float yOffset)
        {
            _camera.ProcessMouseScroll(yOffset);
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return Draw();
        }

        private Bitmap Draw()
        {
            Array.Clear(_rgbValues, 0, _rgbValues.Length);

            foreach (var line in _objReader.GetVertices())
            {
                var startVector = _transformMatrix.Scale(0.5f) * line.StartVector;
                var endVector = _transformMatrix.Scale(0.5f) * line.EndVector;

                Draw(startVector[0], startVector[1], endVector[0],
                    endVector[1], _rgbValues);
            }

            var bitmap = GetBitmapFromList(_rgbValues);
            
            return bitmap;
        }

        private static ObjReader.ObjReader ReadDataFromObjFile(string path)
        {
            var objReader = new ObjReader.ObjReader();

            path = Directory.GetCurrentDirectory() + path;
            
            objReader.ReadObjFile(path);

            return objReader;
        }
        
        private void Draw(float x0, float y0, float x1, float y1, IList<byte> rgbValues)
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

                tempX = CheckValue(tempX, _width);
                tempY = CheckValue(tempY, _height);
                
                rgbValues[tempY * _width + tempX] = WHITE;

                error -= dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
        
        private Bitmap GetBitmapFromList(byte[] rgbValues)
        {
            var bmp = new Bitmap(_width, _height, PixelFormat.Format8bppIndexed);
            
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            Marshal.Copy(rgbValues, 0, bitmapData.Scan0, rgbValues.Length);

            bmp.UnlockBits(bitmapData);

            return bmp;
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