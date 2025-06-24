
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Context;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Connection");
Console.WriteLine($" Conexión utilizada: {connectionString}");


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "v1",
        Description = "API para gestionar tareas",
        Contact = new OpenApiContact { Name = "Tu Nombre", Email = "tu@email.com" }
    });
});

builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // Esto publica los endpoints de tus controladores (ej: TodoController)

// Si no querés el endpoint de ejemplo, borrá esto:
app.Run();

