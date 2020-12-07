using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lab1.Extensions;
using Lab1.ObjReader.Model;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ObjReader
{
    public class ObjReader
    {
        public List<GeometricVertex> GeometricVertices { get; }
        public List<FaceVertex> FaceVertices { get; }
        public List<TextureVertex> TextureVertices { get; }

        public List<NormalVertex> NormalVertices { get; }

        private float _maxValue;

        public ObjReader()
        {
            NormalVertices = new List<NormalVertex>();
            GeometricVertices = new List<GeometricVertex>();
            TextureVertices = new List<TextureVertex>();
            FaceVertices = new List<FaceVertex>();
        }

        public void ReadObjFile(string path)
        {
            ReadObjFile(File.ReadAllLines(path));
        }

        private void ReadObjFile(IEnumerable<string> data)
        {
            var lines = data as string[] ?? data.ToArray();
            
            foreach (var line in lines)
            {
                FindMaxValue(line);
            }
            
            foreach (var line in lines)
            {
                ProcessLine(line);
            }
        }

        public IEnumerable<Line3D> GetLines()
        {
            foreach (var faceVertex in FaceVertices.Select(face => face.Vertex.Select(Convert.ToInt32).ToList()))
            {
                for (var j = 0; j < 3; j++)
                {
                    var v0 = GeometricVertices[faceVertex[j] - 1].Vertex;
                    var v1 = GeometricVertices[faceVertex[(j + 1) % 3] - 1].Vertex;

                    yield return new Line3D(v0, v1);
                }
            }
        }
        
        public IEnumerable<Triangle> GetTriangles()
        {
            return FaceVertices.Select(face => face.Vertex.Select(Convert.ToInt32).ToList())
                .Select(faceVertex => new Triangle(
                    GeometricVertices[faceVertex[0] - 1].Vertex,
                    GeometricVertices[faceVertex[1] - 1].Vertex,
                    GeometricVertices[faceVertex[2] - 1].Vertex));
        }

        private void FindMaxValue(string line)
        {
            var parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 0)
            {
                return;
            }

            if (parts[0] == "v")
            {
                for (var i = 1; i < parts.Length - 1; i++)
                {
                    var value = parts.GetFloatByIndex(i);

                    if (_maxValue < value)
                        _maxValue = value;
                }
            }
        }

        private void ProcessLine(string line)
        {
            var parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 0)
            {
                return;
            }
            
            switch (parts[0])
            {
                case "v":
                    var vertex = new GeometricVertex();
                    vertex.ProcessData(parts, _maxValue);
                    GeometricVertices.Add(vertex);
                    break;
                case "f":
                    var face = new FaceVertex();
                    face.ProcessData(parts);
                    FaceVertices.Add(face);
                    break;
                case "vt":
                    var textureVertex = new TextureVertex();
                    textureVertex.ProcessData(parts);
                    TextureVertices.Add(textureVertex);
                    break;
                case "vn":
                    var normalVertex = new NormalVertex();
                    normalVertex.ProcessData(parts);
                    NormalVertices.Add(normalVertex);
                    break;
            }
        }
    }

    public readonly struct Triangle
    {
        public Vector<float> V0 { get; }
        public Vector<float> V1 { get; }
        public Vector<float> V2 { get; }

        public Triangle(Vector<float> v0, Vector<float> v1, Vector<float> v2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
        }
    }

    public readonly struct Line3D
    {
        public Line3D(Vector<float> startVector, Vector<float> endVector)
        {
            StartVector = startVector;
            EndVector = endVector;
        }

        public Vector<float> StartVector { get; }
        public Vector<float> EndVector { get; }
    }

}