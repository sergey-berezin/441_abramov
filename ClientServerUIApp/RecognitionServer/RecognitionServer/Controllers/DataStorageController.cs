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

        private PictureProcessing _pictureProcessing = new PictureProcessing();
        private Dictionary<uint, PictureProcessing> processors = new Dictionary<uint, PictureProcessing>();

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

        [HttpGet("{id}")]
        public ActionResult<KeyValuePair<byte[], List<WebRecognizedObject>>> Get(int id)
        {            
            var imageInfoDetails = DbReaderRecorder.SelectImageContent(id);
            var recognizedObjects = DbReaderRecorder.SelectRecognizedObjects(id);
            if (imageInfoDetails != null && recognizedObjects != null)
                return new KeyValuePair<byte[], List<WebRecognizedObject>>(imageInfoDetails, 
                        new List<WebRecognizedObject>(recognizedObjects.Select(ro => ro.ToWeb())));
            return NotFound("Image information with given id is not found");
        }

        [HttpPost]
        public async Task<ActionResult<WebProcessResult>> StartRecognition(KeyValuePair<uint, KeyValuePair<string, byte[]>> content)
        {
            var threadID = content.Key;
            var image = content.Value;
            string imageName = image.Key.Substring(image.Key.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            //var result = await _pictureProcessing.ProcessSingleImage(imageName, new Bitmap(new MemoryStream(image.Value)));
            if (!processors.ContainsKey(threadID))
                processors[threadID] = new PictureProcessing();
            var result = await processors[threadID].ProcessSingleImage(imageName, new Bitmap(new MemoryStream(image.Value)));
            if (result != null)
                DbReaderRecorder.RecordInfo(result);
            return result?.ToWeb();
            
        }

        [HttpPut]
        public ActionResult<bool> Cancel(uint threadID)
        {
            processors[threadID].Cancel();
            processors.Remove(threadID);
            return true;
        }

        [HttpDelete("{imageInfoId:int}")]
        public int DeleteImageInfo(int imageInfoId)
        {
            var index = DbReaderRecorder.RemoveItem(imageInfoId);
            return index;
        }
    }
}
