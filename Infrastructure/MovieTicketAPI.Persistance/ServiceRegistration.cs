using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieTicketAPI.Application.Repositories.Halls;
using MovieTicketAPI.Application.Repositories.Movies;
using MovieTicketAPI.Application.Repositories.Movies.MovieTicketAPI.Domain.Repositories;
using MovieTicketAPI.Application.Repositories.Showtimes;
using MovieTicketAPI.Domain.Entities.Identity;
using MovieTicketAPI.Persistence.Contexts;
using MovieTicketAPI.Persistence.Repositories.Halls;
using MovieTicketAPI.Persistence.Repositories.Movies;
using MovieTicketAPI.Persistence.Repositories.Showtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Persistence
{
    public static class ServiceRegistration
    {
        // 'this IServiceCollection' ifadesi bunun bir Extension Method olmasını sağlar
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. DbContext Kaydı
            services.AddDbContext<MovieTicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Identity Kaydı (Hazır buradayken bunu da aradan çıkaralım)
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // İleride şifre kurallarını vb. buraya yazabiliriz
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<MovieTicketDbContext>();

            // --- MOVIE ---
            services.AddScoped<IMovieReadRepository, MovieReadRepository>();
            services.AddScoped<IMovieWriteRepository, MovieWriteRepository>();

            // --- HALL ---
            services.AddScoped<IHallReadRepository, HallReadRepository>();
            services.AddScoped<IHallWriteRepository, HallWriteRepository>();

            // --- SHOWTIME ---
            services.AddScoped<IShowTimeReadRepository, ShowTimeReadRepository>();
            services.AddScoped<IShowTimeWriteRepository, ShowTimeWriteRepository>();
        }
    }
    }
}
