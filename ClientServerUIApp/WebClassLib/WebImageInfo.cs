using System;
using System.Collections.Generic;
using System.Text;

// Server and every client should contain classes from this namespace to know how to parse data
namespace WebClassLib
{
    // WebImageInfo is converted from ImageInfo for data transmission by internet.
    public class WebImageInfo
    {
        public WebImageInfo(int id, string name, string hash) 
        {
            ImageInfoId = id;
            ImageName = name;
            ImageHash = hash;
        }

        public int ImageInfoId { get; set; }

        public string ImageName { get; set; }

        public string ImageHash { get; set; }
    }
}
