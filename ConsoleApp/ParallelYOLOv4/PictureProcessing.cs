using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using YOLOv4MLNet.DataStructures;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ParallelYOLOv4
{   
    public class PictureProcessing : IDisposable
    {        
        public string ImageOutputFolder { get; private set; }

        private BlockingCollection<ProcessResult> processResultsBuffer = null;
        private TransformerChain<OnnxTransformer> model = null;

        public PictureProcessing()
        {           
            model = MakePredictionModel(ClassLibConfig.ModelPath);
            processResultsBuffer = new BlockingCollection<ProcessResult>();
        }

        public async IAsyncEnumerable<ProcessResult> ProcessImagesAsync(string imageFolder)
        {            
            ImageOutputFolder = Path.Combine(imageFolder, ClassLibConfig.OutputFolder);
            Directory.CreateDirectory(ImageOutputFolder);

            var images = Directory.GetFiles(imageFolder);
            List<Task<ProcessResult>> processors = new List<Task<ProcessResult>>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            foreach (var imagePath in images)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    Task<ProcessResult> task = ProcessSingleImage(imagePath);
                    processors.Add(task);
                }
                else
                    break;
            }

            for (int i = 0; i < processors.Count; i++)
            {
                if (!cancellationToken.IsCancellationRequested)
                    // the expression is blocked until any element appears in collection
                    yield return processResultsBuffer.Take(); 
                else
                    break;
            }

            await Task.WhenAll(processors);
        }

        public async Task<ProcessResult> ProcessSingleImage(string imagePath)
        {
            return await Task.Factory.StartNew(() =>
            {
                var labels = ObjectsSegmentation(imagePath);
                Dictionary<string, int> uniqueLabels = new Dictionary<string, int>();

                foreach (var label in labels)
                {
                    if (uniqueLabels.ContainsKey(label))
                        uniqueLabels[label] += 1;
                    else
                        uniqueLabels.Add(label, 1);
                }                

                string imageName = imagePath.Substring(imagePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                ProcessResult imageResult;
                imageResult.imageName = imageName;
                imageResult.categoriesCounts = uniqueLabels;
                processResultsBuffer.Add(imageResult);
                return imageResult;                
            });
        }

        private IReadOnlyList<string> ObjectsSegmentation(string imagePath)
        {
            List<string> imageObjectCategories = new List<string>();
            // Create prediction engine
            MLContext mlContext = new MLContext();
            PredictionEngine<YoloV4BitmapData, YoloV4Prediction> predictionEngine =
                mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
            using (var bitmap = new Bitmap(Image.FromFile(imagePath)))
            {
                // predict
                var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                var results = predict.GetResults(ClassLibConfig.ClassesNames, 0.3f, 0.7f);

                using (var g = Graphics.FromImage(bitmap))
                {
                    string imageName = imagePath.Substring(imagePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    foreach (var res in results)
                    {
                        // draw predictions
                        var x1 = res.BBox[0];
                        var y1 = res.BBox[1];
                        var x2 = res.BBox[2];
                        var y2 = res.BBox[3];
                        g.DrawRectangle(Pens.Red, x1, y1, x2 - x1, y2 - y1);
                        using (var brushes = new SolidBrush(Color.FromArgb(50, Color.Red)))
                        {
                            g.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                        }

                        g.DrawString(res.Label + " " + res.Confidence.ToString("0.00"),
                                        new Font("Arial", 12), Brushes.Blue, new PointF(x1, y1));
                        imageObjectCategories.Add(res.Label);
                    }
                    bitmap.Save(Path.Combine(ImageOutputFolder,
                        Path.ChangeExtension(imageName, "_processed" + Path.GetExtension(imageName))));
                }
            }
            return imageObjectCategories;
        }              

        private TransformerChain<OnnxTransformer> MakePredictionModel(string modelPath)
        {
            MLContext mlContext = new MLContext();

            // Define scoring pipeline
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: modelPath, recursionLimit: 50));

            // Fit on empty list to obtain input data schema
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));
            return model;            
        }

        public void Dispose()
        {
            if (processResultsBuffer != null)
                processResultsBuffer.Dispose();
        }
    }
}
