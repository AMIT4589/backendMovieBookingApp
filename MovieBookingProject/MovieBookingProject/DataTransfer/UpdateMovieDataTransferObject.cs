namespace MovieBookingProject.DataTransfer
{
    public class UpdateMovieDataTransferObject
    {
        public string MovieName { get; set; } = string.Empty;

        public string TheatreName { get; set; } = string.Empty;


        public int TotalTicketsAlloted { get; set; }

        // public int NumberOfTicketsBooked { get; set; }

        public string Status { get; set; } = "Available";
    }
}
