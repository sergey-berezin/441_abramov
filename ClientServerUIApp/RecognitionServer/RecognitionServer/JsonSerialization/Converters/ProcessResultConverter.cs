using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using ParallelYOLOv4;
using RecognitionServer.JsonSerialization.SerializableClasses;

namespace RecognitionServer.JsonSerialization.Converters
{
    public class ProcessResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SerializableProcessResult);        

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string imgName = null;
            Bitmap bitmap = null;
            IReadOnlyList<ProcessResult.RecognitionObject> categories = null;
            SerializableProcessResult objectDescription = null;
            while (reader.Read())
            {
                switch ((string)reader.Value)
                {
                    case nameof(objectDescription.ImageName):
                        reader.Read();
                        imgName = (string)reader.Value;
                        break;
                    case nameof(objectDescription.Bitmap):
                        reader.Read();
                        var byteArray = Convert.FromBase64String((string)reader.Value);
                        bitmap = new Bitmap(new MemoryStream(byteArray));
                        break;
                    case nameof(objectDescription.Categories):
                        reader.Read();
                        categories = JsonConvert.DeserializeObject<IReadOnlyList<ProcessResult.RecognitionObject>>((string)reader.Value);
                        foreach (var cat in categories)
                            Console.WriteLine(cat);
                        break;
                }
            }            

            return new SerializableProcessResult(categories, imgName, bitmap);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (ProcessResult)value;
            var imgConverter = new ImageConverter();
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(item.ImageName));
            writer.WriteValue(item.ImageName);
            writer.WritePropertyName(nameof(item.Bitmap));
            writer.WriteValue((byte[])imgConverter.ConvertTo(item.Bitmap, typeof(byte[])));
            writer.WritePropertyName(nameof(item.Categories));
            writer.WriteValue(JsonConvert.SerializeObject(item.Categories));
            writer.WriteEndObject();
            writer.Flush();
        }
    }
}
