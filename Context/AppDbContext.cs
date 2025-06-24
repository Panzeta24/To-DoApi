using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Context
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext>TodoItem): base(TodoItem)
        {
        }
        public DbSet<TodoItem> Tareas { get; set; }
    }
}
