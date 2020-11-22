using System;
using System.IO;
using System.Windows;

namespace Lab._1.design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string FILE_NAME = @"\resources\ship.obj";
        
        public MainWindow()
        {
            InitializeComponent();
            ReadObjFile();
        }

        private void ReadObjFile()
        {
            var path = Directory.GetCurrentDirectory() + FILE_NAME;
            
            var objReader = new ObjReader();
            objReader.ReadObjFile(path);
        }
    }
}