using System;
using ParallelYOLOv4;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string imageFolder;// = @"A:\4_year\.NET_Technologies\Projects\441_abramov\Images";
            PictureProcessing pictureProcessing = null;
            if (args.Length >= 1)
            {
                imageFolder = args[0];
            }
            else
            {
                Console.WriteLine("Please type path to the image folder");
                imageFolder = Console.ReadLine();
            }
            try
            {
                pictureProcessing = new PictureProcessing(imageFolder);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            
            if (pictureProcessing != null)
                await foreach (var processResult in pictureProcessing.ProcessImagesAsync())
                    Console.WriteLine(processResult);
        }
    }
}
