using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieBookingProject.Models
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string TicketId { get; set; } = string.Empty;

        [BsonElement("MovieName")]
        public string MovieName { get; set; } = string.Empty;


        [BsonElement("TheatreName")]
        public string TheatreName { get; set; } = string.Empty;

        //TicketsBooked,TicketsAlloted.

        [BsonElement("NumberOfTicketsBooked")]
        public int NumberOfTicketsBooked { get; set; }

        [BsonElement("TicketStatus")]
        public string TicketStatus { get; set; } = "Approved";
    }
}
