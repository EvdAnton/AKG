using System.Text;
using Lab1.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ObjReader.Model
{
    public class FaceVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 4;
        private const string PREFIX = "f";

        public FaceVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }

        public Vector<float> Vertex { get; private set; }
        public Vector<float> TextureVertex { get; private set; }
        public Vector<float> NormalVertex{ get; private set; }

        public override void ProcessData(string[] data)
        {
            base.ProcessData(data);
            
            var vCount = data.Length - 1;
            
            InitializeVertexes(vCount);
            
            for (var i = 0; i < vCount; i++)
            {
                var parts = data[i + 1].Split('/');

                Vertex.Add(i, parts, 0);
                
                TextureVertex.Add(i, parts, 1);
                
                NormalVertex.Add(i, parts, 2);
            }
        }

        private void InitializeVertexes(int vCount)
        {
            Vertex = Vector<float>.Build.Dense(vCount);
            TextureVertex = Vector<float>.Build.Dense(vCount);
            NormalVertex = Vector<float>.Build.Dense(vCount);
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("f ");

            for (var i = 0; i < Vertex.Count; i++)
            {
                b.AppendFormat( $"{Vertex[i]}");
                
                if (i < TextureVertex.Count)
                {
                    b.AppendFormat($"/{TextureVertex[i]}");
                }
                
                if (i < NormalVertex.Count)
                {
                    b.AppendFormat($"/{NormalVertex[i]} ");
                }
            }

            return b.ToString();
        }
    }
}