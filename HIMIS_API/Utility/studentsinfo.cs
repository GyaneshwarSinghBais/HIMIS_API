using MongoDB.Bson;

namespace MongowithAsp
{
    public class studentsinfo
    {
        public ObjectId _id { get; set; }
        public string studentId { get; set; }
        public string ReceiptID { get; set; }
        
        //It Is For Chunks//public ObjectId files_id { get; set; }
        public string Name { get; set; }
        //It Is For Chunks//public int n { get; set; }
        //It Is For Chunks// public BsonBinaryData data { get; set; }
        public BsonBinaryData file { get; set; }
        public string filename { get; set; }
        public string length { get; set; }
        public string Registration { get; set; }
	public string ext { get; set; }
        public static void Insert(BsonDocument state)
        {
            throw new System.NotImplementedException();
        }
    }
}