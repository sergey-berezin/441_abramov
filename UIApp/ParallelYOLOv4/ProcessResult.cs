using System.Collections.Generic;
using System.Linq;
using YOLOv4MLNet.DataStructures;
using System.Drawing;


namespace ParallelYOLOv4
{
    public class ProcessResult : System.IDisposable
    {
        public struct RecognitionObject
        {
            public string ObjName { get; set; }
            public double Confidence { get; set; }
            public override string ToString() => $"{ObjName} with confidence = {Confidence}";
        }

        private Dictionary<string, int> _categoriesCount = null;

        public ProcessResult(IReadOnlyList<YoloV4Result> results, string imgName, Bitmap bitmap)
        {
            ImageName = imgName;
            _categoriesCount = new Dictionary<string, int>();
            var categories = new List<RecognitionObject>();
            foreach (var res in results)
            {
                if (!_categoriesCount.ContainsKey(res.Label))
                    _categoriesCount[res.Label] = 0;
                _categoriesCount[res.Label]++;

                RecognitionObject recognitionObject = new RecognitionObject();
                recognitionObject.ObjName = res.Label;
                recognitionObject.Confidence = res.Confidence;
                categories.Add(recognitionObject);
            }

            Categories = categories;
            Bitmap = bitmap;
        }

        public string ImageName { get; }
        public IReadOnlyList<RecognitionObject> Categories { get; }
        public Bitmap Bitmap { get; }

        public bool IsEmpty() => Categories.Count == 0;

        public override string ToString() =>
            $"'{ImageName}' contains next objects: {string.Join(", ", _categoriesCount.ToList().Select(pair => $"{pair.Key} x{pair.Value}"))}";
        
        public string ToLongString() => 
            $"'{ImageName}':\n\t{string.Join("\n\t", Categories.Select(cat => $"{cat}"))}";    
        
        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}
