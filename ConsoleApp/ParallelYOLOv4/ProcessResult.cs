using System.Collections.Generic;
using System.Linq;
using YOLOv4MLNet.DataStructures;


namespace ParallelYOLOv4
{
    public class ProcessResult
    {
        public struct RecognitionObject
        {
            public string objName;
            public double confidence;
            public override string ToString() => $"{objName} with confidence = {confidence}";
        }

        private Dictionary<string, int> categoriesCount = null;

        public ProcessResult(IReadOnlyList<YoloV4Result> results, string imgName)
        {
            ImageName = imgName;
            categoriesCount = new Dictionary<string, int>();
            var categories = new List<RecognitionObject>();
            foreach (var res in results)
            {
                if (!categoriesCount.ContainsKey(res.Label))
                    categoriesCount[res.Label] = 0;
                categoriesCount[res.Label]++;

                RecognitionObject recognitionObject;
                recognitionObject.objName = res.Label;
                recognitionObject.confidence = res.Confidence;
                categories.Add(recognitionObject);
            }

            Categories = categories;
        }

        public string ImageName { get; set; }
        public IReadOnlyList<RecognitionObject> Categories { get; set; }

        public bool IsEmpty() => Categories.Count == 0;

        public override string ToString() =>
            $"'{ImageName}' contains next objects: {string.Join(", ", categoriesCount.ToList().Select(pair => $"{pair.Key} x{pair.Value}"))}";
        
        public string ToLongString() => 
            $"'{ImageName}':\n\t{string.Join("\n\t", Categories.Select(cat => $"{cat}"))}";            
    }
}
