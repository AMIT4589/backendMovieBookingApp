using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieBookingProject.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Token")]
        public string Token { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("ResetPasswordToken")]
        public string ResetPasswordToken { get; set; }
        [BsonElement("ResetPasswordExpiry")]
        public DateTime ResetPasswordExpiry { get; set; }
    }
}
