using Lab1.Extensions;

namespace Lab1.ObjReader.Model
{
    public class NormalVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 4;
        private const string PREFIX = "vn";
        public NormalVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }

        public decimal I { get; private set; }
        public decimal J { get; private set; }
        public decimal K { get; private set; }

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);
            
            I = data.GetDecimalByIndex(1, nameof(I));
            J = data.GetDecimalByIndex(2, nameof(J));
            K = data.GetDecimalByIndex(3, nameof(K));
        }
        
        public override string ToString()
        {
            return $"vn {I} {J} {K}";
        }
    }
}