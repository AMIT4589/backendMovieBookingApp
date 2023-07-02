using MovieBookingProject.Models;

namespace MovieBookingProject.Interfaces
{
    public interface ITicketInterface
    {
        List<Ticket> Get();
        Ticket Get(string id);
        Ticket Create(Ticket ticket);
        void Update(string id, Ticket ticket);
        void Delete(string id);

        Ticket Exists(string movie, string theatre);
        Ticket GetMovie(string id);
    }
}
