using System;
using ParallelYOLOv4;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string imageFolder;
            if (args.Length >= 1)
            {
                imageFolder = args[0];
            }
            else
            {
                Console.WriteLine("Please type path to the image folder");
                imageFolder = Console.ReadLine();
            }
                
            using (var pictureProcessing = new PictureProcessing())
            {
                Console.WriteLine("Processing...");
                await foreach (var processResult in pictureProcessing.ProcessImagesAsync(imageFolder))
                    Console.WriteLine(processResult);
            }
        }
    }
}