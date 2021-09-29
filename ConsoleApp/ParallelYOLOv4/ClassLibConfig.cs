using System;
using System.IO;

namespace ParallelYOLOv4
{
    public static class ClassLibConfig
    {
        // this class contain all constants that is used in the class lib project
        private static string gitDirName = "441_abramov";

        public static string ModelPath = 
            @"A:\4_year\.NET_Technologies\Projects\441_abramov\ConsoleApp\ParallelYOLOv4\YOLOv4Model\yolov4.onnx";
        //PathMaker(@"YOLOv4MODEL\yolov4.onnx", gitDirName);
        public static string OutputFolder = "Output";

        public static readonly string[] ClassesNames = new string[] { "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
        
        private static string PathMaker(string filename, string gitDirName)
        {
            string currentDir = Directory.GetCurrentDirectory();            
            int length = currentDir.LastIndexOf(gitDirName);
            string gitDirPath = currentDir.Substring(0, length);

            return Path.Combine(gitDirPath, $@"ConsoleApp\ParallelYOLOv4\{filename}");
        }
    }
}
