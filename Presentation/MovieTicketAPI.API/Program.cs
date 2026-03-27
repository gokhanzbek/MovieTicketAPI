using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieTicketAPI.Application;
using MovieTicketAPI.Infrastructure; 
using MovieTicketAPI.Persistence;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaret API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'�n�z� 'Bearer {token}' format�nda girin."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- KATMANLI MİMARİ SERVİSLERİ ---
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
// Eğer Infrastructure katmanın varsa (Token servisleri vb. için) bu satırı da eklemelisin:
builder.Services.AddInfrastructureServices();

// CurrentUser gibi işlemlerde HttpContext'e erişebilmek için:
builder.Services.AddHttpContextAccessor();

// --- 1. CORS POLİTİKASINI TANIMLA ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

// --- JWT AYARLARI ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Token:Audience"],
        ValidIssuer = builder.Configuration["Token:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 2. CORS KULLANIMI (Sıralama önemli!) ---
app.UseCors("AllowAll");

// --- 3. KİMLİK VE YETKİ KONTROLÜ (Sıralama önemli!) ---
app.UseAuthentication(); // Önce kimlik doğrulanır (Sen kimsin?)
app.UseAuthorization();  // Sonra yetki kontrol edilir (Bunu yapmaya iznin var mı?)

app.MapControllers();

app.Run();