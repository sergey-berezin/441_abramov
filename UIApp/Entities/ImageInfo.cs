using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Entities
{
    public class ImageInfo
    {
        [Key]
        public int ImageInfoId { get; set; }

        public string ImageName { get; set; }

        public string ImageHash { get; set; }

        virtual public ImageInfoDetails ImageInfoDetails { get; set; }

        virtual public ICollection<RecognizedObject> RecognizedObjects { get; set; }
    }
}
