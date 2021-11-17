using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class RecognizedObject
    {
        [Key]
        public int ObjectId { get; set; }

        public int ImageInfoId { get; set; }

        public string CategoryName { get; set; }

        public double Confidence { get; set; }
    }
}
