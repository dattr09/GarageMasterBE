using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GarageMasterBE.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        [BsonElement("emailConfirmed")]
        public bool EmailConfirmed { get; set; } = false;

        [BsonElement("emailConfirmationCode")]
        public string? EmailConfirmationCode { get; set; }

        [BsonElement("emailConfirmationCodeExpiry")]
        public DateTime? EmailConfirmationCodeExpiry { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } = "Customer"; // Customer | Employee

        [BsonElement("linkedEntityId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? LinkedEntityId { get; set; }    // CustomerId hoặc EmployeeId
    }
    
}
