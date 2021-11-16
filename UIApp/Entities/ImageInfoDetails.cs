using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ImageInfoDetails
    {
        [Key]
        public int ImageInfoDetailsId { get; set; }

        public byte[] Image { get; set; }
    }
}
