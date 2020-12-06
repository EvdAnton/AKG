﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            foreach (var line in data)
            {
                ProcessLine(line);
            }
        }

        public IEnumerable<Line3D> GetVertices()
        {
            foreach (var face in FaceVertices)
            {
                var faceVertex = face.Vertex.Select(Convert.ToInt32).ToList();
                
                for (var j = 0; j < 3; j++)
                {
                    var v0 = GeometricVertices[faceVertex[j] - 1].Vertex;
                    var v1 = GeometricVertices[faceVertex[(j + 1) % 3] - 1].Vertex;

                    yield return new Line3D(v0, v1);
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
                    vertex.ProcessData(parts);
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