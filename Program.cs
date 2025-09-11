using backendProject.Data.SqlDbContext;
using Microsoft.EntityFrameworkCore;
using P0_ClassLibrary;
using P0_ClassLibrary.Interfaces;
using P0_ClassLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowFrontend", policy =>
policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

var cloudinaryUrl = builder.Configuration["Cloudinary:Url"] ?? throw new InvalidOperationException("Cloudinary url not set !");
var SecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException(" Secret Key not set !");
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<ICloudinaryService>(_ => new CloudinaryService(cloudinaryUrl));
builder.Services.AddSingleton<ITokenService>(_ => new TokenService(SecretKey));
builder.Services.AddSingleton<IMailService, EmailService>();

builder.Services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cloud")));

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();