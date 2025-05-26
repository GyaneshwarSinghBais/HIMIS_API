using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIMIS_API.Models
{
    //[Table("WorkPhysicalProgressFiles")]
    public class Mongo_WorkPhysicalProgressModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ID { get; set; }

        [BsonElement("WORK_ID")]
        public string? WORK_ID { get; set; }

        [BsonElement("SR")]
        public string? SR { get; set; }

        [BsonElement("IMAGENAME")]
        public string? IMAGENAME { get; set; }

        [BsonElement("IMAGEDATA")]
        public string? IMAGEDATA { get; set; }

        [BsonElement("IMAGENAME2")]
        public string? IMAGENAME2 { get; set; }

        [BsonElement("ImageDatalvl2")]
        public string? IMAGEDATALVL2 { get; set; }
        //public string? ImageName3 { get; set; }
        //public string? ImageDatalvl3 { get; set; }
        //public string? ImageName4 { get; set; }
        //public string? ImageDatalvl4 { get; set; }
        //public string? ImageName5 { get; set; }
        //public string? ImageDatalvl5 { get; set; }

    }
}
