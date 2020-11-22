using System;
using System.Collections.Generic;
using System.IO;
using Lab._1.Types;

namespace Lab._1
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
}