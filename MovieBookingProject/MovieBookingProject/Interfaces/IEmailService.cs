using MovieBookingProject.Models;

namespace MovieBookingProject.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModal);
    }

}
