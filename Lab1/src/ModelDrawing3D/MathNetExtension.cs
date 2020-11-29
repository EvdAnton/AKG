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

        public static Matrix<float> GetResultMatrix(int width, int height)
        {
            var cameraPosition = Vector<float>.Build.Dense(new[] {0f, 0f, 3f});
            var targetPosition = Vector<float>.Build.Dense(new[] {0f, 0f, 0f});

            return GetViewPortMatrix(width / 2f, height / 2f)
                   * GetProjectionMatrix(width, height, 0.1f, 1000f)
                   * GetCameraMatrix(cameraPosition, targetPosition)
                   * GetIdentityMatrix(4, 4);
        }

        private static Matrix<float> GetIdentityMatrix(int rows, int columns)
        {
            return Matrix<float>.Build.DenseIdentity(rows, columns);
        }
        

        private static Matrix<float> GetCameraMatrix(Vector<float> cameraPosition, Vector<float> targetPosition)
        {
            var reverseTargetDirection = (cameraPosition - targetPosition)
                .Normalize(1);

            var cameraRight = _up.CrossProduct(reverseTargetDirection)
                .Normalize(1);

            var cameraUp = reverseTargetDirection.CrossProduct(cameraRight);

            var lookAt = Matrix<float>.Build.Dense(4, 4);
            lookAt.SetRow(0, cameraRight.AddValueToEnd(0));
            lookAt.SetRow(1, cameraUp.AddValueToEnd(0));
            lookAt.SetRow(2, reverseTargetDirection.AddValueToEnd(0));
            lookAt.SetRow(3, new[] {0f, 0f, 0f, 1f});

            var identityMatrix = GetIdentityMatrix(4, 4);
            identityMatrix.SetColumn(3, -cameraPosition.AddValueToEnd(1));

            return lookAt * identityMatrix;
        }

        private static Matrix<float> GetProjectionMatrix(int width, int height, float zNear, float zFar)
        {
            var deltaZ = zNear - zFar;
            var aspect = width / height;

            var projection = Matrix<float>.Build.Dense(4, 4);

            projection[0, 0] = 1f / (aspect * _tanFov);
            projection[1, 1] = 1f / _tanFov;
            projection[2, 2] = zFar / deltaZ;
            projection[3, 2] = zNear * zFar / deltaZ;
            projection[3, 3] = -1f;

            return projection;
        }

        private static Matrix<float> GetViewPortMatrix(float halfOfWidth, float halfOfHeight)
        {
            var viewPortMatrix = GetIdentityMatrix(4, 4);

            viewPortMatrix[0, 0] = halfOfWidth;
            viewPortMatrix[1, 1] = -halfOfHeight;
            viewPortMatrix[0, 3] = halfOfWidth;
            viewPortMatrix[1, 3] = halfOfHeight;

            return viewPortMatrix;
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