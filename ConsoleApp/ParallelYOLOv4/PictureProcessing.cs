using Microsoft.ML;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using YOLOv4MLNet.DataStructures;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelYOLOv4
{   
    public class PictureProcessing
    {        
        static string modelPath;
        public string ImageFolder { get; private set; }
        public string ImageOutputFolder { get; private set; }

        public PictureProcessing(string imageFolder)
        {
            try
            {
                if (!Directory.Exists(imageFolder))
                    throw new DirectoryNotFoundException("This directory doesn't exists. Please try again.");
            }
            catch (DirectoryNotFoundException ex)
            {
                //Console.WriteLine(ex.Message);
                throw ex;
            }

            modelPath = ClassLibConfig.ModelPath;
            ImageFolder = imageFolder;
            ImageOutputFolder = Path.Combine(ImageFolder, ClassLibConfig.OutputFolder);

            Directory.CreateDirectory(ImageOutputFolder);
        }

        public async IAsyncEnumerable<ProcessResult> ProcessImagesAsync()
        {
            var images = Directory.GetFiles(ImageFolder);
            List<Task<ProcessResult>> processors = new List<Task<ProcessResult>>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            string imageName;
            Console.WriteLine("Processing...");
            foreach (var imagePath in images)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    imageName = imagePath.Substring(imagePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    Task<ProcessResult> task = ProcessSingleImage(imageName);
                    processors.Add(task);
                }
                else
                    break;
            }

            List<int> checkTasks = new List<int>();
            while (processors.Count > checkTasks.Count)
                for (int i = 0; i < processors.Count; i++)
                    if (processors[i].IsCompleted && !checkTasks.Contains(i))
                    {
                        checkTasks.Add(i);
                        yield return processors[i].Result;
                    }

            await Task.WhenAll(processors);
        }

        public async Task<ProcessResult> ProcessSingleImage(string imageName)
        {
            return await Task.Factory.StartNew(() =>
            {
                var labels = ObjectsSegmentation(imageName);
                Dictionary<string, int> uniqueLabels = new Dictionary<string, int>();

                foreach (var label in labels)
                {
                    if (uniqueLabels.ContainsKey(label))
                        uniqueLabels[label] += 1;
                    else
                        uniqueLabels.Add(label, 1);
                }                

                ProcessResult imageResult;
                imageResult.imageName = imageName;
                imageResult.categoriesCounts = uniqueLabels;
                return imageResult;                
            });
        }

        private IReadOnlyList<string> ObjectsSegmentation(string imageName)
        {
            string imagePath = Path.Combine(ImageFolder, imageName);
            List<string> imageObjectCategories = new List<string>();
            PredictionEngine<YoloV4BitmapData, YoloV4Prediction> predictionEngine = MakePredictionModel();
            using (var bitmap = new Bitmap(Image.FromFile(imagePath)))
            {
                // predict
                var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                var results = predict.GetResults(ClassLibConfig.ClassesNames, 0.3f, 0.7f);

                using (var g = Graphics.FromImage(bitmap))
                {
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

        private PredictionEngine<YoloV4BitmapData, YoloV4Prediction> MakePredictionModel()
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

            // Create prediction engine
            PredictionEngine<YoloV4BitmapData, YoloV4Prediction> predictionEngine =
                mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);

            // save model
            //mlContext.Model.Save(model, predictionEngine.OutputSchema, Path.ChangeExtension(modelPath, "zip"));
            return predictionEngine;
        }
    }
}
