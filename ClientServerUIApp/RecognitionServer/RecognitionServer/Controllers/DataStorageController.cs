using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStorage;
using ParallelYOLOv4;
using WebClassLib;
using RecognitionServer.WebExtensions;

namespace RecognitionServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataStorageController : Controller
    {
        [HttpGet]
        public ActionResult<List<WebImageInfo>> GetImagesInfo()
        {
            List<WebImageInfo> result = null;
            try
            {
                result = new List<WebImageInfo>(DbReaderRecorder.SelectImagesInfo().Select(imgInfo => imgInfo.ToWeb()));
            }
            catch (Exception)
            {
                return StatusCode(503, "Service is unavaivable");
            }
            return result;
        } 

        [HttpGet("{id:int}")]
        public ActionResult<KeyValuePair<byte[], List<WebRecognizedObject>>> Get(int id)
        {            
            var imageInfoDetails = DbReaderRecorder.SelectImageContent(id);
            var recognizedObjects = DbReaderRecorder.SelectRecognizedObjects(id);
            if (imageInfoDetails != null && recognizedObjects != null)
                return new KeyValuePair<byte[], List<WebRecognizedObject>>(imageInfoDetails, 
                        new List<WebRecognizedObject>(recognizedObjects.Select(ro => ro.ToWeb())));
            return NotFound("Image information with given id is not found");
        }

        [HttpGet("{category}")]
        public ActionResult<List<KeyValuePair<byte[], List<WebRecognizedObject>>>> Get(string category)
        {
            if (category == "None")
                category = null;
            var result = new List<KeyValuePair<byte[], List<WebRecognizedObject>>>();
            foreach (var elem in DbReaderRecorder.SelectImagesByCategory(category))
            {
                result.Add(new KeyValuePair<byte[], List<WebRecognizedObject>>(elem.Key, 
                    new List<WebRecognizedObject>(elem.Value.Select(val => 
                        new WebRecognizedObject(val.ObjectId, val.ImageInfoId, val.CategoryName, val.Confidence)))));
            }
            return result;
        }

        [Route("categories")]
        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            List<string> categories = new List<string>(DbReaderRecorder.SelectUniqueCategories());
            return categories;
        }

        [HttpPost]
        public async Task<ActionResult<WebProcessResult>> StartRecognition(KeyValuePair<string, byte[]> image)
        {
            string imageName = image.Key.Substring(image.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            PictureProcessing pictureProcessing = new PictureProcessing();
            var result = await pictureProcessing.ProcessSingleImage(imageName, new Bitmap(new MemoryStream(image.Value)));
            if (result != null)
                DbReaderRecorder.RecordInfo(result);
            return result?.ToWeb();
            
        }

        [HttpDelete("{imageInfoId:int}")]
        public int DeleteImageInfo(int imageInfoId)
        {
            var index = DbReaderRecorder.RemoveItem(imageInfoId);
            return index;
        }
    }
}
