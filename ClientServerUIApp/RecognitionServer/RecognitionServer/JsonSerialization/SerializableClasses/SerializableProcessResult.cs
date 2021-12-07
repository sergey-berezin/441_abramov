using System.Drawing;
using ParallelYOLOv4;
using System.Collections.Generic;
using Newtonsoft.Json;
using RecognitionServer.JsonSerialization.Converters;

namespace RecognitionServer.JsonSerialization.SerializableClasses
{
    [JsonConverter(typeof(ProcessResultConverter))]
    public class SerializableProcessResult : ProcessResult
    {
        [JsonConstructor]
        public SerializableProcessResult(IReadOnlyList<RecognitionObject> categories, 
            string imgName, Bitmap bitmap) : base(categories, imgName, bitmap)
        {

        }

        public SerializableProcessResult(ProcessResult processResult) : 
            this(processResult.Categories, processResult.ImageName, processResult.Bitmap)
        {

        }
    }
}
