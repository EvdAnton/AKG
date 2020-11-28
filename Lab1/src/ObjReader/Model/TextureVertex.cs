using Lab1.Extensions;

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

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);
            
            U = data.GetFloatByIndex(1, nameof(U));
            V = data.GetFloatByIndex(2, nameof(V));
            W = data.GetFloatByIndex(3, nameof(W));
        }
        
        public override string ToString()
        {
            return $"vt {U} {V} {W}";
        }
    }
}