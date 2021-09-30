using System;
using ParallelYOLOv4;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string imageFolder = @"A:\4_year\.NET_Technologies\Projects\441_abramov\Images";
            PictureProcessing pictureProcessing = new PictureProcessing(imageFolder);
            //pictureProcessing.ProcessImages();
            await foreach (var processResult in pictureProcessing.ProcessImagesAsync())
                Console.WriteLine(processResult);
        }
    }
}
