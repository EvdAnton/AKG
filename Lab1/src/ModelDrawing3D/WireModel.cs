using System.Drawing;

namespace Lab1.ModelDrawing3D
{
    // TODO : twice buffer for image?
    
    public class WireModel
    {
        private const int WIDTH  = 400;
        private const int HEIGHT = 400;

        private readonly Graphics _graphics;
        
        private Brush Brush { get; set; }
        private ObjReader.ObjReader ObjReader { get; set; }

        public WireModel(ObjReader.ObjReader objReader, Graphics graphics, Brush brush)
        {
            Brush = brush;
            _graphics = graphics;
            ObjReader = objReader;
        }

        public void Draw()
        {
            foreach (var face in ObjReader.FaceVertices)
            {
                for (var j = 0; j < 3; j++)
                {
                    var v0 = ObjReader.GeometricVertices[face.Vertex[j] - 1];
                    var v1 = ObjReader.GeometricVertices[face.Vertex[(j + 1) % 3] - 1];

                    var x0 = (v0.X + 1) * WIDTH;
                    var y0 = 2 * HEIGHT - (v0.Y + 1) * HEIGHT;
                    var x1 = (v1.X + 1) * WIDTH;
                    var y1 = 2 * HEIGHT - (v1.Y + 1) * HEIGHT;
                    
                    BresenhemLine.Draw(_graphics, Brush, x0, y0, x1, y1);
                }
            }
        }
    }
}