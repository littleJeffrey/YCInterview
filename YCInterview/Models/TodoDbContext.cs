using Microsoft.EntityFrameworkCore;
using System;

namespace YCInterview.Models;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<User> Users { get; set; }
}
