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
        public float W { get; private set; }

        public override void ProcessData(string[] data, float maxValue = default)
        {
            base.ProcessData(data, maxValue);

            W = data.GetFloatByIndex(4, nameof(W), 1);
            
            X = data.GetFloatByIndex(1, nameof(X)) / W / maxValue;
            Y = data.GetFloatByIndex(2, nameof(Y)) / W / maxValue;
            Z = data.GetFloatByIndex(3, nameof(Z)) / W / maxValue;
            
            Vertex = Vector<float>.Build.Dense(new[] {X, Y, Z, W});
        }

        public override string ToString()
        {
            return $"v {X} {Y} {Z} {W}";
        }
    }
}