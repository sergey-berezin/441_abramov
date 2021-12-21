using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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

        public static IEnumerable<RecognizedObject> SelectRecognizedObjects(int imageInfoId)
        {
            using (var db = new ImagesLibraryContext())
            {
                var query = db.ImagesInfo.Include(imgInfo => imgInfo.RecognizedObjects)
                                         .Where(imgInfo => imgInfo.ImageInfoId == imageInfoId)
                                         .FirstOrDefault()?.RecognizedObjects;
                foreach (var recognizedObject in query)
                    yield return recognizedObject;
            }
        }

        public static byte[] SelectImageContent(int imageInfoId)
        {
            byte[] byteArray;
            using (var db = new ImagesLibraryContext())
            {
                byteArray = db.ImagesInfo.Include(imgInfo => imgInfo.ImageInfoDetails)
                                         .Where(imgInfo => imgInfo.ImageInfoId == imageInfoId)
                                         .FirstOrDefault()?.ImageInfoDetails.Image;
            }
            return byteArray;
        }        

        public static IEnumerable<KeyValuePair<byte[], List<RecognizedObject>>> SelectImagesByCategory(string category)
        {
            IEnumerable<ImageInfo> query;
            using (var db = new ImagesLibraryContext())
            {
                if (category != null)
                {
                    query = db.ImagesInfo.Include(imgInfo => imgInfo.RecognizedObjects)
                                         .Where(imgInfo => imgInfo.RecognizedObjects
                                                                  .Select(obj => obj.CategoryName.ToLower())
                                                                  .Contains(category.ToLower()))
                                         .Include(imgInfo => imgInfo.ImageInfoDetails)
                                         .AsEnumerable();
                }
                else
                {
                    query = db.ImagesInfo.Include(imgInfo => imgInfo.RecognizedObjects)
                                         .Include(imgInfo => imgInfo.ImageInfoDetails)
                                         .AsEnumerable();
                }

                foreach (var imgInfo in query)
                {
                    yield return new KeyValuePair<byte[], List<RecognizedObject>>
                        (
                            imgInfo.ImageInfoDetails.Image, 
                            new List<RecognizedObject>(imgInfo.RecognizedObjects)
                        );
                }
            }

        }

        public static IEnumerable<string> SelectUniqueCategories()
        {
            List<string> categories = new List<string>();
            using (var db = new ImagesLibraryContext())
            {
                var query = db.ImagesInfo.Include(imgInfo => imgInfo.RecognizedObjects)
                                         .Select(imgInfo => imgInfo.RecognizedObjects);
                foreach (var range in query)
                {
                    categories.AddRange(range.Select(r => r.CategoryName));
                }
            }
            return categories.Distinct();
        }

        public static void RemoveItem(ImageInfo imageInfo)
        {
            using (var db = new ImagesLibraryContext())
            {
                db.Attach(imageInfo);
                db.Entry(imageInfo).Collection(imgInfo => imgInfo.RecognizedObjects).Load();
                db.Entry(imageInfo).Reference(imgInfo => imgInfo.ImageInfoDetails).Load();
                db.Remove(imageInfo);
                db.SaveChanges();
            }
        }

        public static int RemoveItem(int imageInfoId)
        {
            using (var db = new ImagesLibraryContext())
            {
                var imageInfo = db.ImagesInfo.Include(imgInfo => imgInfo.RecognizedObjects)
                                             .Include(imgInfo => imgInfo.ImageInfoDetails)
                                             .Where(imgInfo => imgInfo.ImageInfoId == imageInfoId)
                                             .FirstOrDefault();
                if (imageInfo != null)
                {
                    db.Remove(imageInfo);
                    db.SaveChanges();
                    return imageInfoId;
                }
                return -1;
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
                    db.Entry(img).Reference(imgInfo => imgInfo.ImageInfoDetails).Load();
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
