using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public static class MathNetExtension
    {
        private static readonly float _tanFov = (float) Math.Tan(45f / 2f);

        private static readonly Vector<float> _up = Vector<float>.Build.Dense(new[] {0f, 1f, 0f});

        private static Matrix<float> ModelMatrix { get; set; }
        private static Matrix<float> ViewMatrix { get; set; }
        private static Matrix<float> ProjectionMatrix { get; set; }
        private static Matrix<float> ViewportMatrix { get; set; }

        public static Matrix<float> GetResultMatrix(int width, int height)
        {
            var eye = Vector<float>.Build.Dense(new[] {0f, 0f, 2.5f});
            var center = Vector<float>.Build.Dense(new[] {0f, 0f, 1.5f});

            SetModelMatrix();
            SetViewMatrix(eye, center);
            SetProjectionMatrix(width, height, 0.1f, 1000f);
            SetViewPortMatrix(width / 2f, height / 2f);
            
            return ViewportMatrix
                   * ProjectionMatrix
                   * ViewMatrix
                   * ModelMatrix;
        }

        public static void MoveModelMatrix(float x, float y, float z)
        {
            var displacementMatrix = GetIdentityMatrix(4, 4);

            displacementMatrix[0, 3] = x;
            displacementMatrix[1, 3] = y;
            displacementMatrix[2, 3] = z;

            ModelMatrix *= displacementMatrix;
        }

        public static void ScaleModelMatrix(float scale)
        {
            var scaleMatrix = GetIdentityMatrix(4, 4);

            scaleMatrix[0, 0] = scale;
            scaleMatrix[1, 1] = scale;
            scaleMatrix[2, 2] = scale;

            ModelMatrix *= scaleMatrix;
        }
        
        public static void XRotateModelMatrix(double angel)
        {
            var xRotationMatrix = GetIdentityMatrix(4, 4);

            var angelInRad = Math.PI * angel / 180.0;
            var sin = (float)Math.Sin(angelInRad);
            var cos = (float)Math.Cos(angelInRad);
            
            xRotationMatrix[1, 1] = cos;
            xRotationMatrix[1, 2] = -sin;
            
            xRotationMatrix[2, 1] = sin;
            xRotationMatrix[2, 2] = cos;
            
            ModelMatrix *= xRotationMatrix;
        }
        
        public static void YRotateModelMatrix(double angel)
        {
            var yRotationMatrix = GetIdentityMatrix(4, 4);

            var angelInRad = Math.PI * angel / 180.0;
            var sin = (float)Math.Sin(angelInRad);
            var cos = (float)Math.Cos(angelInRad);
            
            yRotationMatrix[0, 0] = cos;
            yRotationMatrix[0, 2] = sin;
            
            yRotationMatrix[2, 0] = -sin;
            yRotationMatrix[2, 2] = cos;
            
            ModelMatrix *= yRotationMatrix;
        }
        
        public static void ZRotateModelMatrix(double angel)
        {
            var zRotationMatrix = GetIdentityMatrix(4, 4);

            var angelInRad = Math.PI * angel / 180.0;
            var sin = (float)Math.Sin(angelInRad);
            var cos = (float)Math.Cos(angelInRad);
            
            zRotationMatrix[0, 0] = cos;
            zRotationMatrix[0, 1] = -sin;
            
            zRotationMatrix[1, 0] = sin;
            zRotationMatrix[1, 1] = cos;
            
            ModelMatrix *= zRotationMatrix;
        }

        private static Matrix<float> GetIdentityMatrix(int rows, int columns)
        {
            return Matrix<float>.Build.DenseIdentity(rows, columns);
        }
        
        private static void SetModelMatrix()
        {
            ModelMatrix = GetIdentityMatrix(4, 4);
        }
        

        private static void SetViewMatrix(Vector<float> eye, Vector<float> center)
        {
            var targetDirection = (eye - center)
                .Normalize(1);

            var cameraRight = _up.CrossProduct(targetDirection)
                .Normalize(1);

            var cameraUp = targetDirection.CrossProduct(cameraRight)
                .Normalize(1);

            var lookAt = Matrix<float>.Build.Dense(4, 4);
            lookAt.SetRow(0, cameraRight.AddValueToEnd(0));
            lookAt.SetRow(1, cameraUp.AddValueToEnd(0));
            lookAt.SetRow(2, targetDirection.AddValueToEnd(0));
            lookAt.SetRow(3, new[] {0f, 0f, 0f, 1f});

            var identityMatrix = GetIdentityMatrix(4, 4);
            identityMatrix.SetColumn(3, -eye.AddValueToEnd(1));

            ViewMatrix = lookAt * identityMatrix;
        }

        private static void SetProjectionMatrix(int width, int height, float zNear, float zFar)
        {
            var deltaZ = zNear - zFar;
            var aspect = width / height;

            var projection = Matrix<float>.Build.Dense(4, 4);

            projection[0, 0] = 1f / (aspect * _tanFov);
            projection[1, 1] = 1f / _tanFov;
            projection[2, 2] = zFar / deltaZ;
            projection[3, 2] = zNear * zFar / deltaZ;
            projection[3, 3] = -1f;

            ProjectionMatrix = projection;
        }

        private static void SetViewPortMatrix(float halfOfWidth, float halfOfHeight)
        {
            var viewportMatrix = GetIdentityMatrix(4, 4);

            viewportMatrix[0, 0] = halfOfWidth;
            viewportMatrix[1, 1] = -halfOfHeight;
            viewportMatrix[0, 3] = halfOfWidth;
            viewportMatrix[1, 3] = halfOfHeight;

            ViewportMatrix = viewportMatrix;
        }
        
        private static Vector<float> CrossProduct(this IList<float> left, IList<float> right)
        {
            if (left.Count != 3 || right.Count != 3)
                throw new ArgumentException();

            var result = Vector<float>.Build.Dense(3);

            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];

            return result;
        }

        private static Vector<float> AddValueToEnd(this IEnumerable<float> vector, float value)
        {
            var result = vector.AsEnumerable().Append(value);

            return Vector<float>.Build.DenseOfEnumerable(result);
        }
    }
}