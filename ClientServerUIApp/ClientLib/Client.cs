using System;
using System.IO;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebClassLib;

namespace ClientLib
{
    public class Client : IDisposable
    {
        #region Fields

        private static uint _clientsCount = 0;

        #endregion

        #region Constructor

        public Client()
        {
            Cancelled = false;
            ClientID = _clientsCount;
            _clientsCount++;
        }

        #endregion

        #region Properties

        public uint ClientID { get; }

        public bool Cancelled { get; private set; }

        #endregion

        #region PublicStaticMethods

        public static List<WebImageInfo> Get(string url)
        {
            string answer;            
            using (HttpClient client = new HttpClient())
            {
                answer = client.GetStringAsync(url).Result;
            }
            return JsonConvert.DeserializeObject<List<WebImageInfo>>(answer);
        }

        public static KeyValuePair<byte[], List<WebRecognizedObject>>? Get(string url, int imgInfoId)
        {
            // byte[] obj is bitmap source. If you want to get System.Drawing.Bitmap
            // you need to read this byte array from memory stream
            #nullable enable
            string? answer = null;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    answer = client.GetStringAsync(url + '/' + imgInfoId.ToString()).Result;
                }
                catch (Exception) { }
            }
            if (answer != null)
                return JsonConvert.DeserializeObject<KeyValuePair<byte[], List<WebRecognizedObject>>>(answer);
            else
                return null;
        }
    
        public static int? Delete(string url, int index)
        {
            string answer;
            using (var client = new HttpClient())
            {
                try
                {
                    answer = client.DeleteAsync($"{url}/{index}").Result.Content.ReadAsStringAsync().Result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return Convert.ToInt32(answer);
        }

        #endregion

        #region PublicMethods

        public IEnumerable<WebProcessResult?> PostAsync(string url, string imageFolder)
        {
            using (var client = new HttpClient())
            {
                Bitmap bitmap;
                byte[] byteArray;
                KeyValuePair<string, byte[]> image;
                List<Task<HttpResponseMessage>> answers = new List<Task<HttpResponseMessage>>();
                var imageFiles = Directory.GetFiles(imageFolder);
                foreach (var imagePath in imageFiles)
                {
                    bitmap = new Bitmap(Image.FromFile(imagePath));
                    byteArray = (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]));
                    image = new KeyValuePair<string, byte[]>(imagePath, byteArray);
                    var serializedImages = JsonConvert.SerializeObject(
                            new KeyValuePair<uint, KeyValuePair<string, byte[]>>(ClientID, image));
                    var content = new StringContent(serializedImages);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    answers.Add(client.PostAsync(url, content));
                }
                while (answers.Count > 0 && !Cancelled)
                {
                    int taskId = Task.WaitAny(answers.ToArray());
                    HttpResponseMessage response = answers[taskId].Result;
                    string content = response.Content.ReadAsStringAsync().Result;
                    yield return JsonConvert.DeserializeObject<WebProcessResult>(content);
                    answers.RemoveAt(taskId);
                }
                Cancel(url);
            }
        }

        public void Cancel(string url)
        {
            if (!Cancelled)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(ClientID));
                        client.PutAsync(url, content);
                    }
                    catch (Exception) { }
                }
                Cancelled = true;
            }
        }

        public void Dispose()
        {
            _clientsCount--;
        }

        #endregion
    }
}
