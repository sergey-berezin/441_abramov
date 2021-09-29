using System;
using ParallelYOLOv4;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string imageFolder = @"A:\4_year\.NET_Technologies\Projects\441_abramov\Images";
            PictureProcessing pictureProcessing = new PictureProcessing(imageFolder);
            pictureProcessing.ProcessImages();
        }
    }
}
