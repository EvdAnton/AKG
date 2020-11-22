namespace Lab._1.Types
{
    public class TextureVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 3;
        private const string PREFIX = "vt";

        public TextureVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }
        
        public decimal U { get; private set; }
        public decimal V { get; private set; }
        public decimal W { get; private set; }

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);
            
            U = data.GetDecimalByIndex(1, nameof(U));
            V = data.GetDecimalByIndex(2, nameof(V));
            W = data.GetDecimalByIndex(3, nameof(W));
        }
        
        public override string ToString()
        {
            return $"vt {U} {V} {W}";
        }
    }
}