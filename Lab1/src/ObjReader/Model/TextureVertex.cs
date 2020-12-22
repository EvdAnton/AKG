using Lab1.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ObjReader.Model
{
    public class TextureVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 3;
        private const string PREFIX = "vt";

        public TextureVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }
        
        public float U { get; private set; }
        public float V { get; private set; }
        public float W { get; private set; }
        public Vector<float> Vertex { get; private set; }

        public override void ProcessData(string[] data, float maxValue = default)
        {
            base.ProcessData(data, maxValue);
            
            U = data.GetFloatByIndex(1, nameof(U));
            V = data.GetFloatByIndex(2, nameof(V));
            W = data.GetFloatByIndex(3, nameof(W));

            Vertex = Vector<float>.Build.Dense(new[] {U, V});
        }
        
        public override string ToString()
        {
            return $"vt {U} {V} {W}";
        }
    }
}