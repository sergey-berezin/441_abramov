using System.Collections.Generic;

// Server and every client should contain classes from this namespace to know how to parse data
namespace WebClassLib
{
    // WebPRocessResult is converted from ProcessResult for data transmission by internet.
    public class WebProcessResult
    {
        public WebProcessResult(string imageName, 
            List<KeyValuePair<string, double>> nameConfidencePairs, byte[] byteBitmap)
        {
            ImageName = imageName;
            RecognitionObjects = nameConfidencePairs;
            ByteBitmap = byteBitmap;
        }

        public string ImageName { get; }

        public List<KeyValuePair<string, double>> RecognitionObjects { get; set; }

        public byte[] ByteBitmap { get; }
    }
}
