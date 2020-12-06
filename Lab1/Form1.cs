using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private const string OBJ_PATH = "/Resources/Model.obj";
        private const float ANGEL_ROTATION = 5f;
        private const float MOVE_STEP = 0.05f;

        private readonly WireModel _wireModel;
        private readonly BufferedGraphicsContext _context;
        private BufferedGraphics _buffer;
        
        private int _lastXPos;
        private int _lastYPos;
        private bool _isFirst = true;

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
                    image = _wireModel.MoveAndDraw(y: MOVE_STEP);
                    break;
                case Keys.S:
                    image = _wireModel.MoveAndDraw(y: -MOVE_STEP);
                    break;
                case Keys.A:
                    image = _wireModel.MoveAndDraw(-MOVE_STEP);
                    break;
                case Keys.D:
                    image = _wireModel.MoveAndDraw(MOVE_STEP);
                    break;
                case Keys.R:
                    image = _wireModel.MoveAndDraw(z:-MOVE_STEP);
                    break;
                case Keys.T:
                    image = _wireModel.MoveAndDraw(z: MOVE_STEP);
                    break;
                
                case Keys.Y:
                    image = _wireModel.YRotationAndDraw(ANGEL_ROTATION);
                    break;
                case Keys.Z:
                    image = _wireModel.ZRotationAndDraw(ANGEL_ROTATION);
                    break;
                case Keys.X:
                    image = _wireModel.XRotationAndDraw(ANGEL_ROTATION);
                    break;
                
                case Keys.Up:
                    image = _wireModel.CameraMovement(CameraMovement.Forward);
                    break;
                case Keys.Down:
                    image = _wireModel.CameraMovement(CameraMovement.Backward);
                    break;
                case Keys.Right:
                    image = _wireModel.CameraMovement(CameraMovement.Left);
                    break;
                case Keys.Left:
                    image = _wireModel.CameraMovement(CameraMovement.Right);
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

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isFirst)
            {
                _lastXPos = e.X;
                _lastYPos = e.Y;
                _isFirst = false;
            }

            var xOffset = e.X - _lastXPos;
            var yOffset = _lastYPos - e.Y;
            
            if(xOffset < 1 && yOffset < 1)
                return;
            
            var image = _wireModel.ProcessMouseMovement(xOffset, yOffset);
            
            UpdateGraphics(image);
        }
    }
}