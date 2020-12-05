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
            float scaleValue;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.KeyCode)
            {
                case Keys.Q:
                    scaleValue = 1.1f;
                    break;
                case Keys.E:
                    scaleValue = 0.9f;
                    break;
                default:
                    return;
            }

            var image = new Bitmap(Width, Height);
            
            image = _wireModel.ScaleAndDraw(scaleValue, image);
            
            _view.ClearAndDrawImage(image);
        }
    }
}