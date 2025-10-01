using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.Services;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// --- START CORS CONFIGURATION: 1. Add CORS Services ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            // IMPORTANT: Allowing common local origins used for development, including Live Server (5500)
            builder.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500", "http://localhost:5000") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// --- END CORS CONFIGURATION ---


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BlockchainService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    // Moved Swagger usage outside of the check so it runs in Production environment if needed
    // app.UseSwagger();
    // app.UseSwaggerUI();
}
// Moving Swagger outside of IsDevelopment block for robust local testing
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();

// --- CORS MIDDLEWARE: 2. Use CORS Policy (Must be placed before MapControllers) ---
app.UseCors("AllowLocalhost");

app.MapControllers();
app.Run();