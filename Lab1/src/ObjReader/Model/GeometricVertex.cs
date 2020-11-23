using Lab1.Extensions;

namespace Lab1.ObjReader.Model
{
    public class GeometricVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 4;
        private const string PREFIX = "v";
        
        public GeometricVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }
        
        public decimal X { get; private set; }

        public decimal Y { get; private set; }

        public decimal Z { get; private set; }

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);

            X = data.GetDecimalByIndex(1, nameof(X));
            Y = data.GetDecimalByIndex(2, nameof(Y));
            Z = data.GetDecimalByIndex(3, nameof(Z));
        }

        public override string ToString()
        {
            return $"v {X} {Y} {Z}";
        }
    }
}