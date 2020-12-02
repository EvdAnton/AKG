using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    // TODO : twice buffer for image?
    
    public class WireModel
    {
        private readonly Graphics _graphics;
        
        private ObjReader.ObjReader ObjReader { get; }

        public WireModel(ObjReader.ObjReader objReader, Graphics graphics)
        {
            _graphics = graphics;
            ObjReader = objReader;
        }

        public void Draw(Brush brush, Matrix<float> transformMatrix)
        {
            var bresenhemLines = ObjReader.GetVertices(transformMatrix);
            
            foreach (var line in bresenhemLines)
            {
                BresenhemLine.Draw(_graphics, brush, line.FromPoint.X, line.FromPoint.Y, line.ToPoint.X,
                    line.ToPoint.Y);
            }
        }
    }
}