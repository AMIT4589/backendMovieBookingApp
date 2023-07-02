using MongoDB.Driver;
using MovieBookingProject.Interfaces;
using MovieBookingProject.Models;

namespace MovieBookingProject.Repositories
{
    public class MovieRepository : IMovieInterface
    {
        private readonly IMongoCollection<Movie> Movies;

        public MovieRepository(IConnectionWithMongoDb connectionSettings, IMongoClient mongoDBClient)
        {
            var database = mongoDBClient.GetDatabase(connectionSettings.DatabaseName);
            Movies = database.GetCollection<Movie>(connectionSettings.CollectionName[0]);
        }
        public Movie Create(Movie movie)
        {
            Movies.InsertOne(movie);
            return movie;
        }

        public Movie Exists(string movie, string theatre)
        {
            return Movies.Find(m => m.MovieName == movie && m.TheatreName == theatre).SingleOrDefault();

        }
        public List<Movie> Get()
        {
            return Movies.Find(movie => true).ToList();
        }

        public Movie Get(string movieName)
        {
            return Movies.Find(movie => movie.MovieName == movieName).FirstOrDefault();
        }
        public Movie GetMovieByName(string movieName)
        {
            return Movies.Find(movie => movie.MovieName == movieName).FirstOrDefault();
        }
        public Movie GetMovie(string movieId)
        {
            return Movies.Find(movie => movie.MovieId == movieId).FirstOrDefault();
        }

        public void Update(string id, Movie movie)
        {
            Movies.ReplaceOne(s => s.MovieId == id, movie);
        }

        public void Delete(string id)
        {
            Movies.DeleteOne(student => student.MovieId == id);
        }
    }
}
