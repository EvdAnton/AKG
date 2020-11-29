using Lab1.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ObjReader.Model
{
    public class GeometricVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 4;
        private const string PREFIX = "v";
        
        public GeometricVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }

        public Vector<float> Vertex { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);

            X = data.GetFloatByIndex(1, nameof(X));
            Y = data.GetFloatByIndex(2, nameof(Y));
            Z = data.GetFloatByIndex(3, nameof(Z));

            Vertex = Vector<float>.Build.Dense(new[] {X, Y, Z});
        }

        public override string ToString()
        {
            return $"v {X} {Y} {Z}";
        }
    }
}