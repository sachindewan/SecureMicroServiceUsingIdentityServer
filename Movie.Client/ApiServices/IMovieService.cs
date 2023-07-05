using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movie.Client.ApiServices
{
    public interface IMovieService
    {
        Task<IEnumerable<Models.Movie>> GetMovie();
        Task<Models.Movie> GetMovie(int id);
        Task PutMovie(int id, Models.Movie movie);
        Task<Models.Movie> PostMovie(Models.Movie movie);
        Task DeleteMovie(int id);
    }

}
