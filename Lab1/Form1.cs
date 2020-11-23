using System;
using System.Drawing;
using System.Windows.Forms;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private static Graphics _graphics;
        
        public Form1()
        {
            InitializeComponent();

            _graphics = View.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BresenhemLine.Draw(_graphics, 10, 10, View.ClientSize.Width - 10, View.ClientSize.Height - 10);
        }
    }
}