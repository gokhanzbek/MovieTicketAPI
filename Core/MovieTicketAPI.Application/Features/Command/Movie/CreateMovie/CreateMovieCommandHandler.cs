using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using System.Threading.Tasks;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Domain.Entities;
using System.Reflection;

namespace MovieTicketAPI.Application.Features.Command.Movie.CreateMovie
{
    public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommandRequest, CreateMovieCommandResponse>
    {
        private readonly IMovieWriteRepository _movieWriteRepository;

        public CreateMovieCommandHandler(IMovieWriteRepository movieWriteRepository)
        {
            _movieWriteRepository = movieWriteRepository;
        }

        public async Task<CreateMovieCommandResponse> Handle(CreateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Movie newMovie = new Domain.Entities.Movie
            {
                Title = request.Title,
                DurationInMinutes = request.DurationInMinutes,
                Genre = request.Genre,
                Director = request.Director,
                ReleaseYear = request.ReleaseYear,
                Description = request.Description
            };
            await _movieWriteRepository.AddAsync(newMovie);
            await _movieWriteRepository.SaveAsync();

            return new CreateMovieCommandResponse
            {
                Id = newMovie.Id,
                IsSuccess = true,
                Message = "Görev başarıyla eklendi!"
                };
        }
    }
}
