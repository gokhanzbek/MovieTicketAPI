using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieTicketAPI.Application.Abstractions.Token;
using MovieTicketAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // DİKKAT: Buraya AppUser parametresi ekledik!
        public Application.DTOs.Token CreateAccessToken(int minute, AppUser user)
        {
            Application.DTOs.Token token = new();
            //securitykey'in simetriğini alıyoruz
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            //şifrelenmiş kimliği oluşturuyoruz
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //oluşturulacak token ayarlarını veriyoruz(örnk zaman)
            token.Expiration = DateTime.UtcNow.AddMinutes(minute);

            // 1. TOKEN'IN İÇİNE GİZLEYECEĞİMİZ KİMLİK BİLGİLERİ (CLAIMS)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID'yi basıyoruz
                new Claim(ClaimTypes.Name, user.UserName) // İstersen adını da basabilirsin
            };

            JwtSecurityToken securityToken = new JwtSecurityToken(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                claims: claims, // 2. OLUŞTURDUĞUMUZ KİMLİĞİ TOKEN'A VERİYORUZ
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            token.AccessToken = tokenHandler.WriteToken(securityToken);//Oluşturulan token'ı accestokena yazdık

            return token;
        }

        // Kullanmadığın hatalı interface metodunu şimdilik sildim, kafayı karıştırmasın.
    }
}
