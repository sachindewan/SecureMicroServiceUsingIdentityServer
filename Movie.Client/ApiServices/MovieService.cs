using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Movie.Client.ApiServices
{
    public class MovieService: IMovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MovieService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<Models.Movie>> GetMovie()
        {
            var client = _httpClientFactory.CreateClient("MovieAPIClient");

            var request  = new HttpRequestMessage(HttpMethod.Get, "api/Movies");

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movieList = JsonConvert.DeserializeObject<IEnumerable<Models.Movie>>(content);
            return movieList;
        }

        public Task<Models.Movie> GetMovie(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task PutMovie(int id, Models.Movie movie)
        {
            throw new System.NotImplementedException();
        }

        public Task<Models.Movie> PostMovie(Models.Movie movie)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
