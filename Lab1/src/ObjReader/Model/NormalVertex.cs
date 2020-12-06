﻿using Lab1.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ObjReader.Model
{
    public class NormalVertex : VertexType
    {
        private const int MINIMUM_VERTEX_LENGTH = 3;
        private const string PREFIX = "vn";
        
        public Vector<float> Vertex { get; private set; }

        public float I { get; private set; }
        public float J { get; private set; }
        public float K { get; private set; }

        public NormalVertex() : base(MINIMUM_VERTEX_LENGTH, PREFIX) { }
        
        public override void ProcessData(string[] data, float maxValue = default)
        {
            base.ProcessData(data, maxValue);
            
            I = data.GetFloatByIndex(1, nameof(I));
            J = data.GetFloatByIndex(2, nameof(J));
            K = data.GetFloatByIndex(3, nameof(K));

            Vertex = Vector<float>.Build.Dense(new[] {I,J,K});
        }
        
        public override string ToString()
        {
            return $"vn {I} {J} {K}";
        }
    }
}