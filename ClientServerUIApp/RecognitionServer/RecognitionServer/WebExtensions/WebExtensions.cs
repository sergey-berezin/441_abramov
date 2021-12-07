using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParallelYOLOv4;
using WebClassLib;
using Entities;
using System.Drawing;

namespace RecognitionServer.WebExtensions
{
    public static class ProcessResultExtension
    {
        public static WebProcessResult ToWeb(this ProcessResult processResult)
        {
            string imgName = processResult.ImageName;
            List<KeyValuePair<string, double>> categoriesDict = 
                new List<KeyValuePair<string, double>>(processResult.Categories.Select(obj => 
                    new KeyValuePair<string, double>(obj.ObjName, obj.Confidence)));
            byte[] byteArray = (byte[])new ImageConverter().ConvertTo(processResult.Bitmap, typeof(byte[]));

            return new WebProcessResult(imgName, categoriesDict, byteArray);
        }        
    }

    public static class RecognizedObjectExtension
    {
        public static WebRecognizedObject ToWeb(this RecognizedObject obj) =>
            new WebRecognizedObject(obj.ObjectId, obj.ImageInfoId, obj.CategoryName, obj.Confidence);
    }

    public static class ImageInfoExtension
    {
        public static WebImageInfo ToWeb(this ImageInfo imgInfo) =>
            new WebImageInfo(imgInfo.ImageInfoId, imgInfo.ImageName, imgInfo.ImageHash);
    }
}
