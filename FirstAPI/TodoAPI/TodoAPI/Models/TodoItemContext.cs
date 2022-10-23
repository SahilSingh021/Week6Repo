using Microsoft.EntityFrameworkCore;
namespace TodoAPI.Models;

public class TodoItemContext : DbContext
{
    protected TodoItemContext()
    {
    }

    public TodoItemContext(DbContextOptions<TodoItemContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
