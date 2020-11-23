﻿using System.Text;
using Lab1.Extensions;

namespace Lab1.ObjReader.Model
{
    public class FaceVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 4;
        private const string PREFIX = "f";

        public FaceVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }
        
        public int[] Vertex { get; private set; }
        public int[] TextureVertex { get; private set; }
        public int[] NormalVertex { get; private set; }

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
            Vertex = new int[vCount];
            TextureVertex = new int[vCount];
            NormalVertex = new int[vCount];
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("f ");

            for (var i = 0; i < Vertex.Length; i++)
            {
                b.AppendFormat( $"{Vertex[i]}");
                
                if (i < TextureVertex.Length)
                {
                    b.AppendFormat($"/{TextureVertex[i]}");
                }
                
                if (i < NormalVertex.Length)
                {
                    b.AppendFormat($"/{NormalVertex[i]} ");
                }
            }

            return b.ToString();
        }
    }
}