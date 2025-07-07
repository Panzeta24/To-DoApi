
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Context;
using TodoApi.Services;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Connection");
Console.WriteLine($" Conexión utilizada: {connectionString}");

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<TodoServices>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        Policy =>
        {
            // direccion frontend.
            // Usa * por ahora para desarrollo, pero en profuccion usar URL exacta.
            Policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


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
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers(); // Esto publica los endpoints de tus controladores (ej: TodoController)

// Si no querés el endpoint de ejemplo, borrá esto:
app.Run();

