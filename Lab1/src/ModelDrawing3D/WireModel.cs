using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Lab1.ObjReader;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public class WireModel
    {
        private const int WHITE = 255;
        private const int WIDTH = 400;
        private const int HEIGHT = 400;
        
        private readonly ObjReader.ObjReader _objReader;
        private readonly int[] _rgbValues;
        private readonly float[] _zBuffer;
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
            _zBuffer = new float[width * height];
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
            for (var i = 0; i < _rgbValues.Length; i++)
            {
                _rgbValues[i] = Color.FromArgb(WHITE, WHITE, WHITE).ToArgb();
            }
            
            for (var i = 0; i < _zBuffer.Length; i++)
            {
                _zBuffer[i] = 1f;
            }

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
            var resultIntensity = triangle.Vertexes
                .Select(t => (_lightPosition - t.RemoveEndValue()).Normalize(1))
                .Select((lightVertex, i) => lightVertex * triangle.NormalVertexes[i])
                .Sum(intensity => intensity > 0 ? intensity * WHITE : 0);

            return  (int) (resultIntensity / 3);
        }

        private void DrawTriangle(IList<float> v0, IList<float> v1, IList<float> v2, int color)
        {
            //if(v0[2] < 0 || v0[2] > 1 || v1[2] < 0 || v1[2] > 1 || v2[2] < 0 || v2[2] > 1)
            //    return;
            
            if(v0[1].Equals(v1[1]) && v0[1].Equals(v2[1]))
                return;

            var totalHeightF = v2[1] - v0[1];
            var totalHeight = (int)Math.Round(totalHeightF, MidpointRounding.AwayFromZero);

            var yFirstPartFloat = v1[1] - v0[1];
            var yFirstPart = (int)Math.Round(yFirstPartFloat, MidpointRounding.AwayFromZero);

            var ySecondPartFloat = v2[1] - v1[1];
            var ySecondPart = (int)Math.Round(ySecondPartFloat, MidpointRounding.AwayFromZero);
            
            for (var i = 0; i < totalHeight; i++)
            {
                var isSecondHalf = i > yFirstPart || v1[1].Equals(v0[1]);
                var segmentHeight = isSecondHalf ? ySecondPartFloat : yFirstPartFloat;

                //if (segmentHeight == 0 || totalHeight == 0) 
                //    continue;
                
                var alpha = i / totalHeightF;
                var beta = (i - (isSecondHalf ? yFirstPartFloat : 0)) / segmentHeight;

                var x1 = v0[0] + (v2[0] - v0[0]) * alpha;
                var x2 = isSecondHalf ? v1[0] + (v2[0] - v1[0]) * beta : v0[0] + (v1[0] - v0[0]) * beta;
                
                var z1 = v0[2] + (v2[2] - v0[2]) * alpha;
                var z2 = isSecondHalf ? v1[2] + (v2[2] - v1[2]) * beta : v0[2] + (v1[2] - v0[2]) * beta;

                if (x1 > x2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref z1, ref z2);
                }

                x1 = CheckValue((int) Math.Round(x1, MidpointRounding.AwayFromZero), _width);
                x2 = CheckValue((int) Math.Round(x2, MidpointRounding.AwayFromZero), _width);
                
                if (x1.Equals(x2) && x1 < float.Epsilon || x1.Equals(x2) && x1.Equals(_width - 1)) 
                    continue;
                
                var y = CheckValue((int) Math.Round(v0[1], MidpointRounding.AwayFromZero) + i, _height);

                DrawHorizontalLine( y, (int) x1, (int) x2, z1, z2, color);
            }
        }

        private void DrawHorizontalLine(int y, int x1, int x2, float z1, float z2, int color)
        {
            var yMulWidth = y * _width;
            var zInc = (z2 - z1) / (x2 - x1 + 1);

            for (var x = x1; x != x2; x++) 
            {
                if (_zBuffer[yMulWidth + x] > z1) 
                {
                    _zBuffer[yMulWidth + x] = z1;
                    _rgbValues[yMulWidth + x] = color;    
                }
                
                z1 += zInc;
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

        private static void Swap(ref float first, ref float second)
        {
            var temp = first;
            first = second;
            second = temp;
        }
    }
}