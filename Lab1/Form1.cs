using System.Drawing;
using System.Windows.Forms;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private const string OBJ_PATH = "/Resources/Model.obj";
        
        private readonly WireModel _wireModel;
        private readonly BufferedGraphicsContext _context;
        private BufferedGraphics _buffer;

        public Form1()
        {
            InitializeComponent();
            _context = BufferedGraphicsManager.Current;

            _context.MaximumBuffer = new Size(Width, Height);

            _buffer = _context.Allocate(CreateGraphics(),
                new Rectangle( 0, 0, Width, Height ));

            _wireModel = new WireModel(OBJ_PATH, Width, Height);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Bitmap image; 
            float scaleValue;
            const float moveStep = 0.05f;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.KeyCode)
            {
                case Keys.Q:
                    scaleValue = 1.1f;
                    image = _wireModel.ScaleAndDraw(scaleValue);
                    break;
                case Keys.E:
                    scaleValue = 0.9f;
                    image = _wireModel.ScaleAndDraw(scaleValue);
                    break;
                case Keys.W:
                    image = _wireModel.MoveAndDraw(y: moveStep);
                    break;
                case Keys.S:
                    image = _wireModel.MoveAndDraw(y: -moveStep);
                    break;
                case Keys.A:
                    image = _wireModel.MoveAndDraw(-moveStep);
                    break;
                case Keys.D:
                    image = _wireModel.MoveAndDraw(moveStep);
                    break;
                case Keys.Z:
                    image = _wireModel.MoveAndDraw(z:-moveStep);
                    break;
                case Keys.X:
                    image = _wireModel.MoveAndDraw(z: moveStep);
                    break;
                
                default:
                    return;
            }

            UpdateGraphics(image);
        }
        
        private void UpdateGraphics(Image image)
        {
            _buffer?.Dispose();
            
            _buffer = _context.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));

            _buffer.Graphics.DrawImage(image, new Point(0, 0));
            
            _buffer.Render();
            
            image.Dispose();
        }
    }
}