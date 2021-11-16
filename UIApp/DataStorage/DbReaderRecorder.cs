using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using ParallelYOLOv4;
using Entities;

namespace DataStorage
{
    public static class DbReaderRecorder
    {
        #region PublicMethods

        public static void RecordInfo(ProcessResult info)
        {
            // make ImageByteArray
            ImageConverter imgConv = new ImageConverter();
            byte[] imgByteArray = (byte[])imgConv.ConvertTo(info.Bitmap, typeof(byte[]));
            var imgHashCodeStr = new HashCode(imgByteArray).ToString();

            using (var db = new ImagesLibraryContext())
            {
                if (!IsDuplicateExist(db, imgByteArray, imgHashCodeStr)) // check if db contains a similar image
                {
                    // make ImageInfoDetails record
                    db.Add(new ImageInfoDetails() { Image = imgByteArray });

                    // make RecognizedObjects record
                    List<RecognizedObject> recognizedObjects = new List<RecognizedObject>();
                    foreach (var category in info.Categories)
                    {
                        var recObj = new RecognizedObject()
                        {
                            CategoryName = category.ObjName,
                            Confidence = category.Confidence
                        };
                        recognizedObjects.Add(recObj);
                        db.Add(recObj);
                    }

                    // make ImageInfo record
                    ImageInfo imageInfo = new ImageInfo()
                    {
                        ImageName = info.ImageName,
                        ImageHash = imgHashCodeStr,
                        ImageInfoDetails = new ImageInfoDetails() { Image = imgByteArray },
                        RecognizedObjects = recognizedObjects
                    };
                    db.Add(imageInfo);

                    db.SaveChanges();
                }
            }
        }

        public static IEnumerable<ImageInfo> SelectImagesInfo()
        {            
            using (var db = new ImagesLibraryContext())
            {
                foreach (var imgInfo in db.ImagesInfo.AsEnumerable())
                    yield return imgInfo;
            }            
        }

        public static IEnumerable<RecognizedObject> SelectRecognizedObjects(ImageInfo imageInfo)
        {
            using (var db = new ImagesLibraryContext())
            {
                var query = db.ImagesInfo.Where(imgInfo => imgInfo.ImageInfoId == imageInfo.ImageInfoId)
                                         .FirstOrDefault()?.RecognizedObjects;
                foreach (var recognizedObject in query)
                    yield return recognizedObject;
            }
        }

        public static Bitmap SelectImageContent(ImageInfo imageInfo)
        {
            Bitmap result;
            using (var db = new ImagesLibraryContext())
            {
                byte[] byteArray = db.ImagesInfo.Where(imgInfo => imgInfo.ImageInfoId == imageInfo.ImageInfoId)
                                                .FirstOrDefault()?.ImageInfoDetails.Image;
                result = new Bitmap(new MemoryStream(byteArray));
            }
            return result;
        }

        public static void RemoveItem(ImageInfo imageInfo)
        {
            using (var db = new ImagesLibraryContext())
            {
                db.Remove(imageInfo);
                db.SaveChanges();
            }

        }

        #endregion

        #region PrivateMethods

        private static bool IsDuplicateExist(ImagesLibraryContext db, byte[] imgByteArray, string imgHashCodeStr)
        {
            bool repeated = false;
            var similarImgs = db.ImagesInfo.Where(info => info.ImageHash.Equals(imgHashCodeStr));
            if (similarImgs != null)
            {
                foreach (var img in similarImgs)
                {
                    repeated = Enumerable.SequenceEqual(img.ImageInfoDetails.Image, imgByteArray);
                    if (repeated)
                        break;
                }
            }
            return repeated;
        }

        #endregion

        #region RelationClasses

        sealed internal class HashCode
        {
            private byte[] _hashCode;

            public HashCode(byte[] byteArray) =>
                _hashCode = new MD5CryptoServiceProvider().ComputeHash(byteArray);                

            public override string ToString() => 
                string.Join("", _hashCode.Select(byteEl => byteEl.ToString("X2")));                
        }

        #endregion
    }
}
