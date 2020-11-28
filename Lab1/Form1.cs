﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lab1.ModelDrawing3D;

namespace Lab1
{
    public sealed partial class Form1 : Form
    {
        private const string OBJ_PATH = "/Resources/Model.obj";

        private static Graphics _graphics;
        private static ObjReader.ObjReader _objReader;
        private static readonly Brush _brush = new SolidBrush(Color.Black);
        
        public Form1()
        {
            InitializeComponent();

            _graphics = View.CreateGraphics();

            _objReader = ReadDataFromObjFile(OBJ_PATH);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var wireModel = new WireModel(_objReader, _graphics, _brush);
            
            wireModel.Draw();
        }
        
        
        private static ObjReader.ObjReader ReadDataFromObjFile(string path)
        {
            var objReader = new ObjReader.ObjReader();

            path = Directory.GetCurrentDirectory() + path;
            
            objReader.ReadObjFile(path);

            return objReader;
        }
    }
}