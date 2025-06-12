using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GarageMasterBE.Models
{
    public class Moto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // MongoDB sẽ tự sinh ObjectId

        [BsonElement("licensePlate")]
        public string LicensePlate { get; set; } = null!;

        [BsonElement("brandId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BrandId { get; set; } = null!; // Tham chiếu bảng Brand (_id)

        [BsonElement("model")]
        public string Model { get; set; } = null!;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = null!; // Tham chiếu bảng Customer (_id)

        [BsonElement("dateOfSent")]
        public DateTime DateOfSent { get; set; } = DateTime.Now;
    }
}
