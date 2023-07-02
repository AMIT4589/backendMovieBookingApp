using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingProject.DataTransfer;
using MovieBookingProject.Interfaces;
using MovieBookingProject.Models;

namespace MovieBookingProject.Controllers
{
    [Route("api/v1.0/moviebooking")]
    [ApiController]
    [Authorize]
    public class MovieController
    : ControllerBase
    {
        private readonly IMovieInterface _MovieRepository;
        private readonly ITicketInterface _TicketRepository;
        // private readonly ILogger<MoviesController> _logger;
        public MovieController(IMovieInterface movieRepository, ITicketInterface ticketRepository)
        {
            _MovieRepository = movieRepository;
            _TicketRepository = ticketRepository;
            // _logger = logger;



        }
        [HttpGet("all")]
        public ActionResult<List<Movie>> Get()
        {
            //_logger.LogInformation("Retreiving the list of movies.");
            return Ok(_MovieRepository.Get());
        }
        [HttpGet("tickets/all")]

        public ActionResult<List<Movie>> GetAllTickets()
        {
            //_logger.LogInformation("Retreiving the list of movies.");
            return Ok(_TicketRepository.Get());
        }
        [HttpGet("movies/search/{moviename}")]
        public ActionResult<Movie> GetMovieInfo(string moviename)
        {
            // _logger.LogInformation("Searching for a give movie.");
            var student = _MovieRepository.Get(moviename);
            if (student == null)
            {
                // _logger.LogInformation("Movie was not found.");
                return NotFound($"Movie with Name {moviename} not found!");
            }
            return student;
        }
        [HttpPost("add")]
        // [Authorize(Roles = "Admin")]
        public ActionResult<Movie> AdminAddsMoive([FromBody] MovieDataTransferObject2 movie)
        {
            //_logger.LogInformation("An admin adding a movie to the database");
            Movie newMovie = new Movie()
            {
                NumberOfTicketsBooked = 0,
                MovieName = movie.MovieName,
                TheatreName = movie.TheatreName,
                TotalTicketsAlloted = movie.TotalTicketsAlloted,
                Status = "Available"


            };
            if (newMovie.TotalTicketsAlloted <= 0)
            {
                // _logger.LogInformation("No seats were allocated which is not allowed by our backend.");
                return Content("Need to Allocate atleast 1 seat.");
            }
            var movieExistsOrNot = _MovieRepository.Exists(movie.MovieName, movie.TheatreName);
            if (movieExistsOrNot != null)
            {
                //_logger.LogInformation("Admin tried to add an already existing movie.");
                return Content("Such an entry already exists.");
            }

            _MovieRepository.Create(newMovie);
            //_logger.LogInformation("Admin has successfully added a new movie.");
            return CreatedAtAction(nameof(Get), new { id = newMovie.MovieId }, newMovie);
        }
        /*
          [HttpPost("add/{movieName}")]
         public ActionResult<Movie> Post(string movieName, string theatreName, int numberOfTickets)
         {
             //_logger.LogInformation("User has begun booking a ticket.");
             var movie = _MovieRepository.Exists(movieName, theatreName);
             if (movie == null) return NotFound();
             movie.NumberOfTicketsBooked = movie.NumberOfTicketsBooked + numberOfTickets;
             movie.TotalTicketsAlloted = movie.TotalTicketsAlloted - numberOfTickets;
             if (movie.TotalTicketsAlloted < 0)
             {
                 //_logger.LogInformation("User tried to book more tickets than are available for the given movie.");
                 return Content("Housefull...cannot book these many tickets.");
             }
             Ticket ticket = new Ticket()
             {
                 MovieName = movieName,
                 TheatreName = theatreName,
                 NumberOfTicketsBooked = numberOfTickets

             };
             _TicketRepository.Create(ticket);
             _MovieRepository.Update(movie.MovieId, movie);
             // _logger.LogInformation("User has successfully booked a ticket.");
             return Ok();
         }
         */
        [HttpPost("addTicket")]
        public ActionResult<Movie> Post(Bookticket newTicket)
        {
            //_logger.LogInformation("User has begun booking a ticket.");
            var movie = _MovieRepository.Exists(newTicket.MovieName, newTicket.TheatreName);
            if (movie == null) return NotFound();
            movie.NumberOfTicketsBooked = movie.NumberOfTicketsBooked + newTicket.NumberOfTicketsBooked;
            movie.TotalTicketsAlloted = movie.TotalTicketsAlloted - newTicket.NumberOfTicketsBooked;
            if (movie.TotalTicketsAlloted < 0)
            {
                //_logger.LogInformation("User tried to book more tickets than are available for the given movie.");
                return Content("Housefull.." +
                    ".cannot book these many tickets.");
            }
            Ticket ticket = new Ticket()
            {
                MovieName = newTicket.MovieName,
                TheatreName = newTicket.TheatreName,
                NumberOfTicketsBooked = newTicket.NumberOfTicketsBooked

            };
            _TicketRepository.Create(ticket);
            _MovieRepository.Update(movie.MovieId, movie);
            // _logger.LogInformation("User has successfully booked a ticket.");
            return Ok(new { Message = "Ticket Booked Successfully" });
        }
        [HttpPut("updatemoviebyadmin")]
        public ActionResult UpdateTheMovieTable([FromBody] UpdateMovieDataTransferObject movie)
        {
            var existingMovie = _MovieRepository.Get(movie.MovieName);
            if (existingMovie == null)
            {
                // _logger.LogInformation("Admin tried to update a movie that doesn't exist in the database.");
                return NotFound($"{movie.MovieName} not found!");

            }
            Movie movieResult = new Movie()
            {
                MovieId = existingMovie.MovieId,
                MovieName = movie.MovieName,
                TheatreName = movie.TheatreName,
                NumberOfTicketsBooked = existingMovie.NumberOfTicketsBooked,
                TotalTicketsAlloted = movie.TotalTicketsAlloted,
                Status = movie.Status

            };
            _MovieRepository.Update(existingMovie.MovieId, movieResult);
            // _logger.LogInformation("Admin has successfuly updated an existing movie in the database.");
            return CreatedAtAction(nameof(Get), new { id = existingMovie.MovieId }, movieResult);
        }


        [HttpPut("update")]
        public ActionResult Put([FromBody] Ticket ticket)
        {
            // _logger.LogInformation("Admin has started updating an existing ticket.");
            var existingTicket = _TicketRepository.GetMovie(ticket.TicketId);
            if ((existingTicket.MovieName != ticket.MovieName))
            {
                //   _logger.LogInformation("Admin tried to update dicket for a movie that doesn't exist.");
                return Content("No such movie found");

            }
            if (existingTicket == null)
            {
                //  _logger.LogInformation("Admin tried to update a ticket that doesn't exist in the database.");
                return Content("No such ticket found.");
            }

            Ticket ticketResult = new Ticket()
            {
                TicketId = ticket.TicketId,
                MovieName = ticket.MovieName,
                TheatreName = ticket.TheatreName,
                NumberOfTicketsBooked = ticket.NumberOfTicketsBooked,
                //NumberOfTicketsBooked = ticket.NumberOfTicketsBooked,
                TicketStatus = ticket.TicketStatus

            };
            if (ticket.TicketStatus == "cancelled")
            {
                //_logger.LogInformation("Admin is cancelling a ticket.");
                ticketResult.NumberOfTicketsBooked = 0;
                var movieUpdate = _MovieRepository.Exists(ticket.MovieName, ticket.TheatreName);
                int tickets = ticket.NumberOfTicketsBooked;
                movieUpdate.NumberOfTicketsBooked = movieUpdate.NumberOfTicketsBooked - tickets;
                movieUpdate.TotalTicketsAlloted = movieUpdate.TotalTicketsAlloted + tickets;

                _MovieRepository.Update(movieUpdate.MovieId, movieUpdate);

            }
            _TicketRepository.Update(ticket.TicketId, ticketResult);
            //  _logger.LogInformation("A ticket has successfully been added by the admin.");
            return CreatedAtAction(nameof(Get), new { id = ticket.TicketId }, ticketResult);
            //return Ok();
        }

        /*
          [HttpPut("{moviename}/update/{ticketId}")]
         public ActionResult Put(string moviename, string ticketId, [FromBody] UpdateTicket ticket)
         {
             // _logger.LogInformation("Admin has started updating an existing ticket.");
             var existingTicket = _TicketRepository.GetMovie(ticketId);
             if ((existingTicket.MovieName != moviename))
             {
                 //   _logger.LogInformation("Admin tried to update dicket for a movie that doesn't exist.");
                 return Content("No such movie found");

             }
             if (existingTicket == null)
             {
                 //  _logger.LogInformation("Admin tried to update a ticket that doesn't exist in the database.");
                 return Content("No such ticket found.");
             }

             Ticket ticketResult = new Ticket()
             {
                 TicketId = existingTicket.TicketId,
                 MovieName = existingTicket.MovieName,
                 TheatreName = existingTicket.TheatreName,
                 NumberOfTicketsBooked = existingTicket.NumberOfTicketsBooked,
                 //NumberOfTicketsBooked = ticket.NumberOfTicketsBooked,
                 TicketStatus = ticket.TicketStatus

             };
             if (ticket.TicketStatus == "cancelled")
             {
                 //_logger.LogInformation("Admin is cancelling a ticket.");
                 ticketResult.NumberOfTicketsBooked = 0;
                 var movieUpdate = _MovieRepository.Exists(existingTicket.MovieName, existingTicket.TheatreName);
                 int tickets = existingTicket.NumberOfTicketsBooked;
                 movieUpdate.NumberOfTicketsBooked = movieUpdate.NumberOfTicketsBooked - tickets;
                 movieUpdate.TotalTicketsAlloted = movieUpdate.TotalTicketsAlloted + tickets;

                 _MovieRepository.Update(movieUpdate.MovieId, movieUpdate);

             }
             _TicketRepository.Update(ticketId, ticketResult);
             //  _logger.LogInformation("A ticket has successfully been added by the admin.");
             return CreatedAtAction(nameof(Get), new { id = existingTicket.TicketId }, ticketResult);
             //return Ok();
         }
         */

        [HttpDelete("delete/{id}")]
        public ActionResult Delete(string id)
        {
            // _logger.LogInformation("Admin has begun deleting a movie from the database.");
            var existingMovie = _MovieRepository.GetMovie(id);
            if (existingMovie == null) return NotFound($"Movie with ID '${id}' not found!");
            /*
                         if (existingMovie.NumberOfTicketsBooked != 0)
                        {
                            return BadRequest();
                        }
             */
            //if (existingMovie.MovieName != moviename) return Content("No such movie found");

            _MovieRepository.Delete(id);
            return Ok(new { Message = "Deleted Successfuly" });
        }
        [HttpDelete("deleteticket/{id}")]
        public ActionResult DeleteTicket(string id)
        {
            // _logger.LogInformation("Admin has begun deleting a movie from the database.");
            var existingMovie = _TicketRepository.GetMovie(id);
            if (existingMovie == null) return NotFound($"Ticket ID '${id}' not found!");
            /*
                         if (existingMovie.NumberOfTicketsBooked != 0)
                        {
                            return BadRequest();
                        }
             */
            //if (existingMovie.MovieName != moviename) return Content("No such movie found");
            Movie movieName = _MovieRepository.GetMovieByName(existingMovie.MovieName);
            int tickets = existingMovie.NumberOfTicketsBooked;
            Movie newMovie = new Movie()
            {
                MovieId = movieName.MovieId,
                MovieName = movieName.MovieName,
                TheatreName = movieName.TheatreName,
                TotalTicketsAlloted = movieName.TotalTicketsAlloted + tickets,
                NumberOfTicketsBooked = movieName.NumberOfTicketsBooked - tickets,
                Status = movieName.Status

            };


            _MovieRepository.Update(movieName.MovieId, newMovie);
            _TicketRepository.Delete(id);
            //_MovieRepository.Update(movieName.MovieId, movieName);
            return Ok(new { Message = $"Deleted Successfuly" });
        }
    }
}
