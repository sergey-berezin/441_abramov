using System;
using ClientLib;
using System.Linq;

using System.IO;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebClassLib;

namespace ClientConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string imageFolder = @"A:\4_year\.NET_Technologies\Projects\Images";
            string url = "http://localhost:5000/api/datastorage/categories";
            //var client = new Client();
            /*foreach (var res in client.PostAsync(url, imageFolder))
            {
                Console.WriteLine(res.ImageName);
                Console.WriteLine(string.Join(", ", res.RecognitionObjects?.Select(obj => obj.Key)));
                client.Cancel();
            }*/

            using (var client = new HttpClient())
            {
                var res = client.GetAsync(url);
                Task.WaitAll(res);

                List<string> text = JsonConvert.DeserializeObject<List<string>>(res.Result.Content.ReadAsStringAsync().Result);

                foreach (var elem in text)
                    Console.WriteLine(elem);

            }

/*            Console.WriteLine(Client.Get(url).Count);
            Console.WriteLine($"Deleted: {Client.Delete(url, 1)}");
            Console.WriteLine(Client.Get(url).Count);*/
        }
    }
}
