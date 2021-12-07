using System;
using ClientLib;
using System.Linq;

namespace ClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string imageFolder = @"A:\4_year\.NET_Technologies\Projects\Images";
            string url = "http://localhost:5000/api/datastorage";
            using (var client = new Client())
            {
                foreach (var res in client.PostAsync(url, imageFolder))
                {
                    Console.WriteLine(res.ImageName);
                    Console.WriteLine(string.Join(", ", res.RecognitionObjects?.Select(obj => obj.Key)));
                    client.Cancel(url);
                }
            }

            foreach (var elem in Client.Get(url))
                Console.WriteLine(elem.ImageName);
            Console.WriteLine(Client.Get(url).Count);
            Console.WriteLine($"Deleted: {Client.Delete(url, 1)}");
            Console.WriteLine(Client.Get(url).Count);
        }
    }
}
