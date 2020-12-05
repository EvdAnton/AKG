using System;
using System.Drawing;
using System.Windows.Forms;
using Lab1.Extensions;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private const string OBJ_PATH = "/Resources/Model.obj";
        
        private readonly WireModel _wireModel;
        private readonly Graphics _view;

        public Form1()
        {
            InitializeComponent();
            _view = CreateGraphics();

            _wireModel = new WireModel(OBJ_PATH, 600, 600);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var image = new Bitmap(Width, Height);
            
            image = _wireModel.Draw(image);
            
            _view.ClearAndDrawImage(image);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var image = new Bitmap(Width, Height);
            float scaleValue;
            const float moveStep = 0.05f;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.KeyCode)
            {
                case Keys.Q:
                    scaleValue = 1.1f;
                    image = _wireModel.ScaleAndDraw(scaleValue, image);
                    break;
                case Keys.E:
                    scaleValue = 0.9f;
                    image = _wireModel.ScaleAndDraw(scaleValue, image);
                    break;
                case Keys.W:
                    image = _wireModel.MoveAndDraw(image, y: moveStep);
                    break;
                case Keys.S:
                    image = _wireModel.MoveAndDraw(image, y: -moveStep);
                    break;
                case Keys.A:
                    image = _wireModel.MoveAndDraw(image, -moveStep);
                    break;
                case Keys.D:
                    image = _wireModel.MoveAndDraw(image, moveStep);
                    break;
                case Keys.Z:
                    image = _wireModel.MoveAndDraw(image, z:-moveStep);
                    break;
                case Keys.X:
                    image = _wireModel.MoveAndDraw(image, z: moveStep);
                    break;
                
                default:
                    return;
            }

            _view.ClearAndDrawImage(image);
        }
    }
}