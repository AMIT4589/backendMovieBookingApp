using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieBookingProject.Models
{
    public class Movie

    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string MovieId { get; set; } = string.Empty;

        [BsonElement("MovieName")]
        public string MovieName { get; set; } = string.Empty;


        [BsonElement("TheatreName")]
        public string TheatreName { get; set; } = string.Empty;

        //TicketsBooked,TicketsAlloted.

        [BsonElement("TotalTickets")]
        public int TotalTicketsAlloted { get; set; }

        [BsonElement("NumberOfTicketsBooked")]
        public int NumberOfTicketsBooked { get; set; }
        [BsonElement("Status")]
        public string Status { get; set; } = "Available";





    }
}
