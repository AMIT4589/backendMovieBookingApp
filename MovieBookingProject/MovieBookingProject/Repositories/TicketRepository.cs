using MongoDB.Driver;
using MovieBookingProject.Interfaces;
using MovieBookingProject.Models;

namespace MovieBookingProject.Repositories
{
    public class TicketRepository : ITicketInterface
    {
        private readonly IMongoCollection<Ticket> Tickets;

        public TicketRepository(IConnectionWithMongoDb connectionSettings, IMongoClient mongoDBClient)
        {
            var database = mongoDBClient.GetDatabase(connectionSettings.DatabaseName);
            Tickets = database.GetCollection<Ticket>(connectionSettings.CollectionName[2]);
        }
        public Ticket Create(Ticket ticket)
        {
            Tickets.InsertOne(ticket);
            return ticket;
        }

        public Ticket Exists(string movie, string theatre)
        {
            return Tickets.Find(m => m.MovieName == movie && m.TheatreName == theatre).SingleOrDefault();

        }
        public List<Ticket> Get()
        {
            return Tickets.Find(movie => true).ToList();
        }

        public Ticket Get(string movieName)
        {
            return Tickets.Find(movie => movie.MovieName == movieName).FirstOrDefault();
        }
        public Ticket GetMovie(string movieId)
        {
            return Tickets.Find(movie => movie.TicketId == movieId).FirstOrDefault();
        }

        public void Update(string id, Ticket movie)
        {
            Tickets.ReplaceOne(s => s.TicketId == id, movie);
        }

        public void Delete(string id)
        {
            Tickets.DeleteOne(student => student.TicketId == id);
        }
    }
}
