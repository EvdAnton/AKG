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
            var lines = ObjReader.GetLines();
            
            foreach (var line in lines)
            {
                var x0 = (line.FromPoint.X + 1) * WIDTH;
                var y0 = 2 * HEIGHT - (line.FromPoint.Y + 1) * HEIGHT;
                var x1 = (line.ToPoint.X + 1) * WIDTH;
                var y1 = 2 * HEIGHT - (line.ToPoint.Y + 1) * HEIGHT;
                
                BresenhemLine.Draw(_graphics, Brush, x0, y0, x1, y1);
            }
        }
    }
}