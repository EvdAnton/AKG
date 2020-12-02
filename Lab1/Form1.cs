using System;
using System.Drawing;
using System.Windows.Forms;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private const string OBJ_PATH = "/Resources/Model.obj";

        private static Graphics _graphics;
        private static readonly Brush _brush = new SolidBrush(Color.Black);
        
        public Form1()
        {
            InitializeComponent();

            _graphics = View.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _graphics.Clear(Color.White);
            
            var wireModel = new WireModel(OBJ_PATH, _graphics, View.Width - 300, View.Height - 300);

            wireModel.Draw(_brush);
        }
    }
}