using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

public class TodoContext : DbContext
{

    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.Entity<TodoItem>().HasData(
            new TodoItem{ Id = 1, Name = "Task 1", IsComplete = false, Secret = "Secret 1" },
            new TodoItem{ Id = 2, Name = "Task 2", IsComplete = false, Secret = "Secret 2" },
            new TodoItem{ Id = 3, Name = "Task 3", IsComplete = true, Secret = "Secret 3" },
            new TodoItem{ Id = 4, Name = "Task 4", IsComplete = true, Secret = "Secret 4" }
        );
    }

}