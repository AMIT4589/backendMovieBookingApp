namespace MovieBookingProject.DataTransfer
{
    public class Bookticket
    {

        public string MovieName { get; set; } = string.Empty;



        public string TheatreName { get; set; } = string.Empty;


        public int NumberOfTicketsBooked { get; set; }

        public string TicketStatus { get; set; } = "Approved";
    }
}
