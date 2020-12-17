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
        private const int WIDTH = 520;
        private const int HEIGHT = 800;
        
        private readonly ObjReader.ObjReader _objReader;
        private readonly int[] _rgbValues;
        private readonly float[] _zBuffer;
        private readonly Camera _camera;
        private readonly int _height;
        private readonly int _width;
        private readonly Vector<float> _lightPosition;

        private Matrix<float> _viewModelProjection;
        private Matrix<float> _viewModelMatrix;
        private Matrix<float> _modelMatrix;
        private Matrix<float> _viewMatrix;
        private Matrix<float> _projectionMatrix;
        private readonly Matrix<float> _viewPortMatrix;

        public WireModel(string path, int width, int height)
        {
            _width = width;
            _height = height;
            
            _objReader = ReadDataFromObjFile(path);

            _camera = new Camera();
            _viewMatrix = _camera.GetViewMatrix();
            _modelMatrix = MathNetExtension.GetModelMatrix();
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            _viewPortMatrix = MathNetExtension.GetViewPortMatrix(WIDTH, HEIGHT);

            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            _lightPosition = Vector<float>.Build.Dense(new[] {0f, 0f, 2.5f});
            _rgbValues = new int[width * height];
            _zBuffer = new float[width * height];
        }

        private static Matrix<float> GetResultMatrix(Matrix<float> viewModelMatrix, Matrix<float> projectionMatrix)
        {
            var pvm = projectionMatrix * viewModelMatrix;

            return pvm;
        }

        private static Matrix<float> GetViewModelMatrix(Matrix<float> viewMatrix, Matrix<float> modelMatrix)
        {
            return viewMatrix * modelMatrix;
        }
        
        public Bitmap ScaleAndDraw(float scaleValue)
        {
            _modelMatrix = _modelMatrix.Scale(scaleValue);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        public Bitmap XRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.XRotate(angel);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        public Bitmap YRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.YRotate(angel);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        public Bitmap ZRotationAndDraw(float angel)
        {
            _modelMatrix = _modelMatrix.ZRotate(angel);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        public Bitmap MoveAndDraw(float x = default, float y = default, float z = default)
        {
            _modelMatrix = _modelMatrix.Move(x, y, z);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }
        
        public Bitmap CameraMovement(CameraMovement direction)
        {
            _camera.ProcessKeyboard(direction);
            _viewMatrix = _camera.GetViewMatrix();
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);
            
            return DrawTriangles();
        }

        public Bitmap ProcessMouseMovement(float xOffset, float yOffset)
        {
            _camera.ProcessMouseMovement(xOffset, yOffset);
            _viewMatrix = _camera.GetViewMatrix();
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);
            
            return DrawTriangles();
        }

        public Bitmap ProcessMouseScroll(float yOffset)
        {
            _camera.ProcessMouseScroll(yOffset);
            _projectionMatrix = _camera.GetProjectionMatrix(WIDTH, HEIGHT);
            _viewModelMatrix = GetViewModelMatrix(_viewMatrix, _modelMatrix);
            _viewModelProjection = GetResultMatrix(_viewModelMatrix, _projectionMatrix);

            return DrawTriangles();
        }

        private void Clear()
        {
            for (var i = 0; i < _rgbValues.Length; i++)
            {
                _rgbValues[i] = Color.FromArgb(WHITE, WHITE, WHITE).ToArgb();
            }
            
            for (var i = 0; i < _zBuffer.Length; i++)
            {
                _zBuffer[i] = 1f;
            }   
        }
        
        private Bitmap DrawTriangles()
        {
            Clear();
            
            foreach (var triangle in _objReader.GetTriangles())
            {
                var v0 = _viewModelProjection * triangle.V0;
                var v1 = _viewModelProjection * triangle.V1;
                var v2 = _viewModelProjection * triangle.V2;

                v0 /= v0.Last();
                v1 /= v1.Last();
                v2 /= v2.Last();
                
                var colorInInt = GetColorForTriangle(triangle);
                
                v0 = _viewPortMatrix * v0;
                v1 = _viewPortMatrix * v1;
                v2 = _viewPortMatrix * v2;

                var vertices = new List<Vector<float>> {v0, v1, v2};

                if (IsBackFaceCulling(vertices))
                {
                    continue;
                }
                
                vertices.Sort((first, second) => first[1].CompareTo(second[1]));

                var color = Color.FromArgb(colorInInt, colorInInt, colorInInt).ToArgb();

                DrawTriangle(vertices[0], vertices[1], vertices[2], color);
            }

            var bitmap = GetBitmapFromList(_rgbValues);
            
            return bitmap;
        }

        private bool IsBackFaceCulling(IReadOnlyList<Vector<float>> vertices)
        {
            var m = Matrix<float>.Build.Dense(3, 3);
            
            m.SetRow(0, new[] {vertices[1][0] - vertices[0][0], vertices[2][0] - vertices[0][0], vertices[0][0]});
            m.SetRow(1, new[] {vertices[1][1] - vertices[0][1], vertices[2][1] - vertices[0][1], vertices[0][1]});
            m.SetRow(2, new[] {0, 0f, 1f});

            return m.Determinant() > 0;
        }


        private int GetColorForTriangle(Triangle triangle)
        {
            var resultIntensity = 0f;
            
            for (var i = 0; i < triangle.Vertexes.Count; i++)
            {
                triangle.Vertexes[i] = _viewModelMatrix * triangle.Vertexes[i];
                
                var lightVertex = (_lightPosition - triangle.Vertexes[i].RemoveEndValue()).Normalize(1);
                var intensity = lightVertex * triangle.NormalVertexes[i];

                resultIntensity += intensity > 0 ? intensity * WHITE : 0;
            }
            
            return (int) (resultIntensity / 3);
        }

        private void DrawTriangle(IList<float> v0, IList<float> v1, IList<float> v2, int color)
        {
            if(v0[2] < 0 || v0[2] > 1 || v1[2] < 0 || v1[2] > 1 || v2[2] < 0 || v2[2] > 1)
                return;
            
            if(v0[1].Equals(v1[1]) && v0[1].Equals(v2[1]))
                return;

            var vI0 = new List<int>();
            var vI1 = new List<int>();
            var vI2 = new List<int>();
            
            for (var i = 0; i < 2; i++)
            {
                vI0.Add((int) Math.Round(v0[i]));
                vI1.Add((int) Math.Round(v1[i]));
                vI2.Add((int) Math.Round(v2[i]));
            }

            var totalHeightF = vI2[1] - vI0[1];

            var yFirstPartFloat = vI1[1] - vI0[1];

            var ySecondPartFloat = vI2[1] - vI1[1];

            for (var i = 0; i < totalHeightF; i++) // F
            {
                var isSecondHalf = i > yFirstPartFloat || vI1[1].Equals(vI0[1]); // F
                var segmentHeight = isSecondHalf ? ySecondPartFloat : yFirstPartFloat;

                if (segmentHeight == 0 || totalHeightF == 0)  // F
                    continue;
                
                var alpha = (float)i / totalHeightF;
                var beta = (float) (i - (isSecondHalf ? yFirstPartFloat : 0)) / segmentHeight;

                var x1 = vI0[0] + (vI2[0] - vI0[0]) * alpha;
                var x2 = isSecondHalf ? vI1[0] + (vI2[0] - vI1[0]) * beta : vI0[0] + (vI1[0] - vI0[0]) * beta;
                
                var z1 = v0[2] + (v2[2] - v0[2]) * alpha;
                var z2 = isSecondHalf ? v1[2] + (v2[2] - v1[2]) * beta : v0[2] + (v1[2] - v0[2]) * beta;

                if (x1 > x2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref z1, ref z2);
                }

                var x0I = CheckValue((int)Math.Round(x1), _width);
                var x1I = CheckValue((int)Math.Round(x2), _width);
                
                if (x0I == x1I && x0I == 0 || x0I == x1I && x0I == _width - 1) 
                    continue;
                
                var y = CheckValue(vI0[1] + i, _height);

                DrawHorizontalLine(y, x0I, x1I, z1, z2, color);
            }
        }

        private void DrawHorizontalLine(int y, int x1, int x2, float z1, float z2, int color)
        {
            var yMulWidth = y * _width;
            var deltaX = x2 - x1;
            
            var zInc = (z2 - z1) / (deltaX == 0 ? 1 : deltaX);

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