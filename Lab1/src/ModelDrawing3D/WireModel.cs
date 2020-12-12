using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Lab1.ObjReader;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public class WireModel
    {
        private const int WHITE = 255;
        private const int WIDTH = 600;
        private const int HEIGHT = 600;
        
        private readonly ObjReader.ObjReader _objReader;
        private readonly int[] _rgbValues;
        private readonly Camera _camera;
        private readonly int _height;
        private readonly int _width;
        private readonly Vector<float> _lightPosition;

        private Matrix<float> _transformMatrix;
        private Matrix<float> _modelMatrix;
        private Matrix<float> _viewMatrix;
        private Matrix<float> _projectionMatrix;

        public WireModel(string path, int width, int height)
        {
            _width = width;
            _height = height;
            
            _objReader = ReadDataFromObjFile(path);

            _camera = new Camera();
            _viewMatrix = _camera.GetViewMatrix();
            _modelMatrix = MathNetExtension.GetModelMatrix();
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            _lightPosition = Vector<float>.Build.Dense(new[] {0f, 0f, 2.5f});
            _rgbValues = new int[width * height];
        }


        public Bitmap ScaleAndDraw(float scaleValue)
        {
            _modelMatrix = _modelMatrix.Scale(scaleValue);

            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return DrawTriangles();
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

            return DrawTriangles();
        }

        public Bitmap YRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.YRotate(angel);
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return DrawTriangles();
        }
        
        
        public Bitmap ZRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.ZRotate(angel);
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        public Bitmap MoveAndDraw(float x = default, float y = default, float z = default)
        {
            _modelMatrix = _modelMatrix.Move(x, y, z);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return DrawTriangles();
        }
        
        public Bitmap CameraMovement(CameraMovement direction)
        {
            _camera.ProcessKeyboard(direction);
            _viewMatrix = _camera.GetViewMatrix();

            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);
            
            return DrawTriangles();
        }

        public Bitmap ProcessMouseMovement(float xOffset, float yOffset)
        {
            _camera.ProcessMouseMovement(xOffset, yOffset);
            _viewMatrix = _camera.GetViewMatrix();
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);
            
            return DrawTriangles();
        }

        public Bitmap ProcessMouseScroll(float yOffset)
        {
            _camera.ProcessMouseScroll(yOffset);
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            
            _transformMatrix = GetResultMatrix(_viewMatrix, _modelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        private Bitmap DrawTriangles()
        {
            Array.Clear(_rgbValues, 0, _rgbValues.Length);

            foreach (var triangle in _objReader.GetTriangles())
            {
                var v0 = _transformMatrix.Scale(0.5f) * triangle.V0;
                var v1 = _transformMatrix.Scale(0.5f) * triangle.V1;
                var v2 = _transformMatrix.Scale(0.5f) * triangle.V2;
                
                var vertices = new List<Vector<float>> {v0, v1, v2};
                vertices.Sort((first, second) => first[1].CompareTo(second[1]));

                var colorInInt = GetColorForTriangle(triangle);
                var color = Color.FromArgb(colorInInt, colorInInt, colorInInt).ToArgb();

                DrawTriangle(vertices[0], vertices[1], vertices[2], color);
            }

            var bitmap = GetBitmapFromList(_rgbValues);
            
            return bitmap;
        }

        private int GetColorForTriangle(Triangle triangle)
        {
            var resultIntensity = 0f;

            for (var i = 0; i < triangle.Vertexes.Count; i++)
            {
                var lightVertex0 = (_lightPosition - triangle.Vertexes[i].RemoveEndValue()).Normalize(1);
                var intensity = lightVertex0 * triangle.NormalVertexes[i];

                // White is RGB setting in 255
                resultIntensity += intensity > 0 ? intensity * WHITE : 0;
            }
            
            return  (int) (resultIntensity / 3);
        }

        private void DrawTriangle(Vector<float> v0, Vector<float> v1, Vector<float> v2, int color)
        {
            if(Math.Abs(v0[1] - v1[1]) < float.Epsilon && Math.Abs(v0[1] - v2[1]) < float.Epsilon)
                return;

            var totalHeight = (int)(v2[1] - v0[1]);
            
            for (var i = 0; i < totalHeight; i++)
            {
                var isSecondHalf = i > v1[1] - v0[1] || Math.Abs(v1[1] - v0[1]) < float.Epsilon;
                var segmentHeight = isSecondHalf ? v2[1] - v1[1] : v1[1] - v0[1];

                var alpha = (float) i / totalHeight;
                var beta = (i - (isSecondHalf ? v1[1] - v0[1] : 0)) / segmentHeight;
                
                var vertexA = v0 + (v2 - v0) * alpha;
                var vertexB = isSecondHalf ? v1 + (v2 - v1) * beta : v0 + (v1 - v0) * beta;

                var minX = (int)Math.Min(vertexA[0], vertexB[0]);
                var length = (int)Math.Abs(vertexA[0] - vertexB[0]) + minX;

                for (var j = minX; j <= length; j++)
                {
                    var tempX = CheckValue(j, _width);
                    var tempY = CheckValue((int)(v0[1] + i), _height);
                    
                    _rgbValues[tempY * _width + tempX] = color;
                }
            }
        }

        private Bitmap GetBitmapFromList(int[] rgbValues)
        {
            var bmp = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            
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
        
        private static ObjReader.ObjReader ReadDataFromObjFile(string path)
        {
            var objReader = new ObjReader.ObjReader();

            path = Directory.GetCurrentDirectory() + path;
            
            objReader.ReadObjFile(path);

            return objReader;
        }
    }
}