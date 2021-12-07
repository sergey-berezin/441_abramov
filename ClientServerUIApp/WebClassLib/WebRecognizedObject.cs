using System;
using System.Collections.Generic;
using System.Text;

namespace WebClassLib
{
    public class WebRecognizedObject
    {

        public WebRecognizedObject(int objId, int imgInfId, string category, double confidence)
        {
            ObjectId = objId;
            ImageInfoId = imgInfId;
            CategoryName = category;
            Confidence = confidence;
        }

        public int ObjectId { get; set; }

        public int ImageInfoId { get; set; }

        public string CategoryName { get; set; }

        public double Confidence { get; set; }
    }
}
