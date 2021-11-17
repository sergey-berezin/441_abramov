using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ImageInfoDetails
    {
        [Key]
        public int ImageInfoDetailsId { get; set; }

        public int ImageInfoId { get; set; }

        public byte[] Image { get; set; }
    }
}
